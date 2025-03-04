using System;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

/// <summary>
/// Component that controls the spell bar
/// </summary>
public class SpellBook : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static SpellBook m_instance = null!;
    public static Dictionary<int, AbilityData> m_abilities = new();
    // Element base, that gets instantiated when new spell is added to book
    public static GameObject m_element = null!;
    
    public RectTransform m_rect = null!;
    public Text[] m_elementTexts = null!;
    // Root parent that contains instantiated elements
    private Transform m_contentList = null!;
    private float m_timer;
    
    public void Init()
    {
        m_instance = this;
        m_rect = GetComponent<RectTransform>();
        m_rect.position = AlmanacClassesPlugin._SpellBookPos.Value;
        m_element = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_element");
        GameObject HoverName = new GameObject("$text_title");
        var rect = HoverName.AddComponent<RectTransform>();
        Text text = HoverName.AddComponent<Text>();
        text.fontSize = 25;
        text.alignment = TextAnchor.MiddleCenter;
        text.supportRichText = true;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        rect.SetParent(m_element.transform.Find("$part_image/$part_image_text"));
        rect.anchoredPosition = new Vector2(0f, 60f);
        m_element.AddComponent<SpellElement>();
        m_elementTexts = m_element.GetComponentsInChildren<Text>();
        m_contentList = transform.Find("$part_content");
        UpdateFontSize();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!(Menu.IsVisible() ^ SkillTree.IsPanelVisible())) return;
        if (eventData.button is not PointerEventData.InputButton.Left) return;
        if (SpellElement.IsMovingSpell()) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_rect.position = Input.mousePosition;
        if (SpellInfo.m_instance.IsVisible())
            SpellInfo.m_instance.Hide();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AlmanacClassesPlugin._SpellBookPos.Value = m_rect.position;
        SpellInfo.m_instance.SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + AlmanacClassesPlugin._MenuTooltipPosition.Value);
    }

    public static void ResetUI()
    {
        var defaultSpellBarPos = (Vector2)AlmanacClassesPlugin._SpellBookPos.DefaultValue;
        var defaultSpellInfoPos = (Vector2)AlmanacClassesPlugin._MenuTooltipPosition.DefaultValue;
        m_instance.m_rect.position = defaultSpellBarPos;
        SpellInfo.m_instance.SetPosition(defaultSpellBarPos + defaultSpellInfoPos);
        
        AlmanacClassesPlugin._SpellBookPos.Value = defaultSpellBarPos;
        AlmanacClassesPlugin._MenuTooltipPosition.Value = defaultSpellInfoPos;
    }
    
    public void Update()
    {
        if (!m_instance || !Player.m_localPlayer || Minimap.IsOpen()) return;
        if (Player.m_localPlayer.IsTeleporting() || Player.m_localPlayer.IsDead() || Player.m_localPlayer.IsSleeping()) return;
        m_timer += Time.deltaTime;
        if (m_timer < 1f) return;
        m_timer = 0.0f;

        UpdateAbilities();
    }
    public static void OnSpellBarPosChange(object sender, EventArgs e)
    {
        m_instance.m_rect.position = AlmanacClassesPlugin._SpellBookPos.Value;
        SpellInfo.m_instance.SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + new Vector2(0f, 150f));
    }
    public static void OnLogout() => ClearSpellBook();
    public static void ClearSpellBook()
    {
        m_abilities.Clear();
        UpdateAbilities();
    }
    public static bool IsAbilityInBook(Talent ability) => m_abilities.Values.Any(talent => ability == talent.m_data);
    public static void RemoveAbility(Talent ability)
    {
        if (!IsAbilityInBook(ability)) return;

        KeyValuePair<int, AbilityData> kvp = default;
        foreach (KeyValuePair<int, AbilityData> item in m_abilities)
        {
            if (item.Value.m_data != ability) continue;
            kvp = item;
            break;
        }

        if (!m_abilities.Remove(kvp.Key)) return;
        NormalizeBook();
        UpdateAbilities();
    }
    private static void NormalizeBook()
    {
        Dictionary<int, AbilityData> newAbilities = new Dictionary<int, AbilityData>();
        int newKey = 0;
        foreach (KeyValuePair<int, AbilityData> item in m_abilities)
        {
            newAbilities[newKey] = item.Value;
            ++newKey;
        }

        m_abilities = newAbilities;
    }
    public void UpdateFontSize()
    {
        foreach (Text component in m_elementTexts)
        {
            component.fontSize = 14;
            component.resizeTextMinSize = 10;
            component.resizeTextForBestFit = true;
        }
    }
    private static Dictionary<int, GameObject> GetExistingElements()
    {
        Dictionary<int, GameObject> existingElements = new Dictionary<int, GameObject>();
        foreach (Transform child in m_instance.m_contentList)
        {
            if (!child.TryGetComponent(out SpellElement component)) continue;
            existingElements[component.m_index] = child.gameObject;
        }

        return existingElements;
    }
    public static void UpdateAbilities()
    {
        if (!Player.m_localPlayer || Player.m_localPlayer.IsDead()) return;

        Dictionary<int, GameObject> existingElements = GetExistingElements();

        foreach (KeyValuePair<int, AbilityData> kvp in m_abilities)
        {
            SpellElement component;
            if (existingElements.TryGetValue(kvp.Key, out GameObject element))
            {
                if (!element.TryGetComponent(out component)) continue;
                component.m_data = kvp.Value;
                component.m_index = kvp.Key;
            }
            else
            {
                element = Instantiate(m_element, m_instance.m_contentList);
                if (!element.TryGetComponent(out component)) continue;
                component.m_data = kvp.Value;
                component.m_index = kvp.Key;
            }

            Sprite? icon = kvp.Value.m_data.GetSprite();
            component.SetIcon(icon);
            component.SetName($"<color=orange>{Localization.instance.Localize(kvp.Value.m_data.GetName())}</color>");
            component.SetHotkey(GetKeyCode(kvp.Key));
            kvp.Value.m_go = element;
            kvp.Value.Update();
        }

        foreach (int key in existingElements.Keys)
        {
            if (m_abilities.ContainsKey(key)) continue;
            Destroy(existingElements[key]);
        }
    }
    private static string GetKeyCode(int index)
    {
        return AddAltKey(RemoveAlpha(index switch
        {
            0 => AlmanacClassesPlugin._Spell1.Value.ToString(),
            1 => AlmanacClassesPlugin._Spell2.Value.ToString(),
            2 => AlmanacClassesPlugin._Spell3.Value.ToString(),
            3 => AlmanacClassesPlugin._Spell4.Value.ToString(),
            4 => AlmanacClassesPlugin._Spell5.Value.ToString(),
            5 => AlmanacClassesPlugin._Spell6.Value.ToString(),
            6 => AlmanacClassesPlugin._Spell7.Value.ToString(),
            7 => AlmanacClassesPlugin._Spell8.Value.ToString(),
            _ => ""
        }));
    }
    private static string AddAltKey(string input) => AlmanacClassesPlugin._SpellAlt.Value is KeyCode.None
        ? input
        : $"{AlmanacClassesPlugin._SpellAlt.Value} + {input}";

    private static string RemoveAlpha(string input) => input.Replace("Alpha", string.Empty);

    private static string GetCooldownColored(int time)
    {
        return time switch
        {
            > 60 => $"<color=#FF5349>{time}</color>",
            > 30 => $"<color=#FFAA33>{time}</color>",
            > 10 => $"<color=#FFAA33>{time}</color>",
            _ => time.ToString()
        };
    }
    public static bool Add(Talent ability)
    {
        if (IsAbilityInBook(ability))
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_in_book");
            return false;
        }
        if (m_abilities.Count > 7)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_book_full");
            return false;
        }

        m_abilities.Add(m_abilities.Count, new AbilityData(ability));
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_added_spell");
        UpdateAbilities();
        return true;
    }
    public class AbilityData
    {
        public readonly Talent m_data;
        public GameObject? m_go;

        public void Update()
        {
            if (!Player.m_localPlayer) return;
            try
            {
                if (m_go is null) return;
                if (!m_go.TryGetComponent(out SpellElement component)) return;
                if (AbilityManager.m_cooldownMap.TryGetValue(m_data.m_key, out float ratio))
                {
                    if (m_data.m_status is not { } status)
                    {
                        component.SetBorder(0f);
                    }
                    else
                    {
                        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(status.NameHash()))
                        {
                            StatusEffect? effect = Player.m_localPlayer.GetSEMan().GetStatusEffect(status.NameHash());
                            float time = effect.GetRemaningTime();
                            float normal = Mathf.Clamp01(time / effect.m_ttl);
                            component.SetBorder(time > 0 ? normal : 0f);
                        }
                        else
                        {
                            component.SetBorder(0f);
                        }
                    }

                    int cooldownTime = (int)(m_data.GetCooldown(m_data.GetLevel()) * ratio);
                    component.SetTimer(GetCooldownColored(cooldownTime));
                    component.SetFillAmount(ratio);
                    if (cooldownTime <= 0)
                    {
                        component.SetTimer(Localization.instance.Localize("$info_ready"));
                    }
                }
                else
                {
                    component.SetBorder(0f);
                    component.SetFillAmount(0f);
                    component.SetTimer(Localization.instance.Localize("$info_ready"));
                }
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to update ability cooldown");
            }
        }
        public AbilityData(Talent talent)
        {
            m_data = talent;
            talent.m_abilityData = this;
        }
    }
}

