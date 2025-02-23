using System;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class SpellBook : MonoBehaviour
{
    public static SpellBook m_instance = null!;
    public static Dictionary<int, AbilityData> m_abilities = new();
    // Element base, that gets instantiated when new spell is added to book
    public static GameObject m_element = null!;
    
    public RectTransform m_rect = null!;
    public Text[] m_elementTexts = null!;
    // Root parent that contains instantiated elements
    private Transform m_contentList = null!;
    public void Init()
    {
        m_instance = this;
        m_rect = GetComponent<RectTransform>();
        m_rect.anchoredPosition = AlmanacClassesPlugin._SpellBookPos.Value;
        m_element = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_element");
        m_element.AddComponent<SpellElement>();
        m_element.AddComponent<SpellElementChange>();
        m_elementTexts = m_element.GetComponentsInChildren<Text>();
        m_contentList = transform.Find("$part_content");
        UpdateFontSize();
    }

    public static void OnSpellBarPosChange(object sender, EventArgs e)
    {
        m_instance.m_rect.anchoredPosition = AlmanacClassesPlugin._SpellBookPos.Value;
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
            if (!child.TryGetComponent(out SpellElementChange component)) continue;
            existingElements[component.index] = child.gameObject;
        }

        return existingElements;
    }

    public static void UpdateAbilities()
    {
        if (!Player.m_localPlayer || Player.m_localPlayer.IsDead() || Minimap.IsOpen()) return;

        Dictionary<int, GameObject> existingElements = GetExistingElements();

        foreach (KeyValuePair<int, AbilityData> kvp in m_abilities)
        {
            if (existingElements.TryGetValue(kvp.Key, out GameObject element))
            {
                if (!element.TryGetComponent(out SpellElementChange component)) continue;
                component.data = kvp.Value;
                component.index = kvp.Key;
            }
            else
            {
                element = Instantiate(m_element, m_instance.m_contentList);
                if (!element.TryGetComponent(out SpellElementChange component)) continue;
                component.data = kvp.Value;
                component.index = kvp.Key;
            }

            Sprite? icon = kvp.Value.m_data.GetSprite();
            var spellElement = element.GetComponent<SpellElement>();
            spellElement.SetIcon(icon);
            spellElement.SetHotkey(GetKeyCode(kvp.Key));
            kvp.Value.m_go = element;
            UpdateCooldownDisplay(kvp.Value);
        }

        foreach (int key in existingElements.Keys)
        {
            if (m_abilities.ContainsKey(key)) continue;
            Destroy(existingElements[key]);
        }
    }

    private static void UpdateCooldownDisplay(AbilityData abilityData)
    {
        // if (abilityData.m_go == null || !Player.m_localPlayer) return;
        if (!Player.m_localPlayer) return;
        try
        {
            if (!abilityData.m_go.TryGetComponent(out SpellElement component)) return;
            if (AbilityManager.m_cooldownMap.TryGetValue(abilityData.m_data.m_key, out float ratio))
            {
                if (abilityData.m_data.m_statusEffectHash == 0)
                {
                    component.SetGrayAmount(0f);
                }
                else
                {
                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(abilityData.m_data.m_statusEffectHash))
                    {
                        StatusEffect? effect = Player.m_localPlayer.GetSEMan()
                            .GetStatusEffect(abilityData.m_data.m_statusEffectHash);
                        float time = effect.GetRemaningTime();
                        float normal = Mathf.Clamp01(time / effect.m_ttl);
                        component.SetGrayAmount(time > 0 ? normal : 0f);
                    }
                    else
                    {
                        component.SetGrayAmount(0f);
                    }
                }

                component.SetFillAmount(ratio);
                int cooldownTime = (int)(abilityData.m_data.GetCooldown(abilityData.m_data.GetLevel()) * ratio);
                component.SetTimer(GetCooldownColored(cooldownTime));
                if (cooldownTime <= 0)
                {
                    component.SetTimer(Localization.instance.Localize("$info_ready"));
                }
            }
            else
            {
                component.SetGrayAmount(0f);
                component.SetFillAmount(0f);
                component.SetTimer(Localization.instance.Localize("$info_ready"));
            }
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to update ability cooldown");
        }
    }

    public static void UpdateCooldownDisplayForAbility(Talent ability)
    {
        if (m_abilities.Values.FirstOrDefault(data => data.m_data == ability) is not { } abilityData) return;
        UpdateCooldownDisplay(abilityData);
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
    
    public class AbilityData
    {
        public Talent m_data = null!;
        public GameObject m_go = null!;
    }
}

