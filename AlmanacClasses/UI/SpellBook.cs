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
    private static readonly Dictionary<int, Transform> m_abilitySlots = new();
    
    public RectTransform m_rect = null!;
    public List<Text> m_elementTexts = new();
    private Transform m_contentList = null!;
    private float m_timer;
    
    public void Init()
    {
        m_instance = this;
        m_rect = GetComponent<RectTransform>();
        m_rect.SetAsFirstSibling();
        m_rect.position = AlmanacClassesPlugin._SpellBookPos.Value;
        m_contentList = transform.Find("$part_content");
        SetupAbilitySlots();
        UpdateFontSize();
    }

    private void SetupAbilitySlots()
    {
        for (int i = 0; i <= 7; i++)
        {
            if (m_contentList.GetChild(i))
            {
                // System works a bit differently this time around, m_index shouldn't be altered,
                // it's pre-determined for each slot.
                m_abilitySlots[i] = m_contentList.GetChild(i);
                var spellElement = m_abilitySlots[i].gameObject.AddComponent<SpellElement>();
                m_abilitySlots[i].gameObject.GetComponent<SpellElement>().m_index = i;
                m_elementTexts.AddRange(spellElement.m_texts);
            }
            else
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogError($"Failed to find AbilitySlot {i+1}");
            }
        }
    }

    private void SetupAbilityFonts()
    {
        var currentFont = Managers.FontManager.GetFont(AlmanacClassesPlugin._Font.Value);
        for (int i = 0; i <= 7; i++)
        {
            var textElements = GetAbilitySlotSpellElement(i)?.GetComponentsInChildren<Text>();
            if (textElements != null && textElements.Length > 0)
            {
                foreach (var textElement in textElements)
                {
                    textElement.font = currentFont;
                }
            }
        }
    }

    private static SpellElement? GetAbilitySlotSpellElement(int index)
    {
        if (m_abilitySlots.Count < 8 || index < 0 || index > 8) return null;
        
        return m_abilitySlots[index].gameObject.GetComponent<SpellElement>();
    }

    public static bool TryGetSpellElement(PointerEventData eventData, out SpellElement spellElement)
    {
        spellElement = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SpellElement>();
        if (spellElement)
            return true;

        return false;
    }

    public static void ResetAbilitySlot(int index)
    {
        var spellElement = GetAbilitySlotSpellElement(index);

        if (spellElement == null) return;
        
        spellElement.m_AbilityData = null;
        spellElement.m_icon.sprite = null;
        spellElement.m_icon.enabled = false;
        var elementTexts = m_abilitySlots[index].gameObject.GetComponentsInChildren<Text>();
        foreach (var elementText in elementTexts)
        {
            elementText.text = "";
        }
        spellElement.SetBorder(0);
        spellElement.SetTimer("");
        spellElement.SetFillAmount(0);
        spellElement.SetName("");
    }

    private static void ResetAllAbilitySlots()
    {
        for (int i = 0; i <= 7; i++)
        {
            var spellElement = GetAbilitySlotSpellElement(i);
            if (spellElement == null) return;
        
            spellElement.m_AbilityData = null;
            spellElement.m_icon.sprite = null;
            spellElement.m_icon.enabled = false;
            
            var elementTexts = m_abilitySlots[i].gameObject.GetComponentsInChildren<Text>();
            foreach (var elementText in elementTexts)
            {
                elementText.text = "";
            }
        }
    }

    public static bool IsAbilitySlotInUse(SpellElement spellElement)
    {
        return (spellElement.m_AbilityData != null);
    }

    public static bool IsAbilitySlotInUse(int slotIndex)
    {
        var spellElement = m_abilitySlots[slotIndex].gameObject.GetComponent<SpellElement>();
        if (spellElement != null)
            return (spellElement.m_AbilityData == null);

        return false;
    }

    private Vector3 mouseDifference = Vector3.zero;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        mouseDifference = m_rect.position - new Vector3(pos.x, pos.y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // TODO: I feel like this may be a redundant keybind, since moving spell slots relies on IPointer, this on IDrag interfaces.
        // left-mouse needs to be pressed down and released for a click to register, dragging won't trigger it - shouldn't cause any conflicts.
        if (!Input.GetKey(KeyCode.LeftAlt)) return; // so it follows previous system, where you need to hold L.Alt to move spell bar
        m_rect.position = Input.mousePosition + mouseDifference;
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
    
    /// <summary>
    /// Checks if the UI is visible, if it has been moved off-screen
    /// </summary>
    public static bool IsVisible()
    {
        return m_instance.m_rect.position.x < Screen.width;
    }
    
    /// <summary>
    /// Sets 'visibility' by moving it off-screen, like Valheim does, thus it can keep ticking.
    /// </summary>
    public static void SetVisible(bool shouldBeVisible)
    {
        if (IsVisible() == shouldBeVisible)
            return;
        
        if (shouldBeVisible)
            m_instance.m_rect.position = AlmanacClassesPlugin._SpellBookPos.Value;
        else
            m_instance.m_rect.position = Hud.s_notVisiblePosition;
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
        ResetAllAbilitySlots();
        UpdateAbilities();
    }
    
    public static bool IsAbilityInBook(Talent ability)
    {
        foreach (var abilitySlot in m_abilitySlots)
        {
            var spellElement = abilitySlot.Value.GetComponent<SpellElement>();
            var abilityData = spellElement.m_AbilityData;
            if (abilityData != null && abilityData.m_talentData == ability)
                return true;
        }

        return false;
    }

    public static void Remove(Talent ability)
    {
        if (!IsAbilityInBook(ability))
            return;
        
        KeyValuePair<int, AbilityData> kvp = default;
        foreach (KeyValuePair<int, AbilityData> item in m_abilities)
        {
            if (item.Value.m_talentData != ability)
                continue;
            
            kvp = item;
            break;
        }
        
        if (!m_abilities.Remove(kvp.Key))
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogError($"[SpellBook::Remove] Failed to remove ability {ability.GetName()}");
            return;
        }
        
        ResetAbilitySlot(kvp.Key);
        UpdateAbilities();
    }

    /// <summary>
    /// Attempts to find a slot that is empty and can be used
    /// </summary>
    /// <param name="slotIndex">Index of the empty slot, -1 if none was found</param>
    /// <returns>true if a slot was successfully found, otherwise false</returns>
    public static bool TryFindFirstAvailableSlot(out int slotIndex)
    {
        slotIndex = -1;
        
        for (var i = 0; i <= 7; i++)
        {
            if (IsAbilitySlotInUse(i))
            {
                slotIndex = i;
                return true;
            }
        }

        return false;
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
        if (!Player.m_localPlayer || Player.m_localPlayer.IsDead())
            return;
        
        foreach (KeyValuePair<int, AbilityData> kvp in m_abilities)
        {
            if (!m_abilitySlots[kvp.Key].gameObject.TryGetComponent(out SpellElement component))
                continue;
            
            var talent = kvp.Value.m_talentData;
            Sprite? icon = talent.GetSprite();
            component.m_AbilityData = kvp.Value;
            component.SetIcon(icon);
            component.SetIconVisibility(true);
            component.SetName(TalentManager.GetLocalizedTalentName(talent));
            component.SetHotkey(GetKeyCode(kvp.Key));
            
            kvp.Value.m_gameObject = m_abilitySlots[kvp.Key].gameObject;
            kvp.Value.Update();
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

    private static string AddAltKey(string key)
    {
        var spellModifier = AlmanacClassesPlugin._SpellAlt.Value;
        if (spellModifier is KeyCode.None)
            return key;

        switch (spellModifier)
        {
            case KeyCode.LeftAlt:
                return $"L.Alt + {key}";
            case KeyCode.LeftControl:
                return $"L.Ctrl + {key}";
            case KeyCode.RightAlt:
                return $"R.Alt + {key}";
            case KeyCode.RightControl:
                return $"R.Ctrl + {key}";
            case KeyCode.LeftShift:
                return $"L.Shift + {key}";
            case KeyCode.RightShift:
                return $"R.Shift + {key}";
            default:
                return $"{spellModifier} + {key}";
        }  
    }

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
        
        var emptySlot = TryFindFirstAvailableSlot(out var slotIndex);
        if (emptySlot)
        {
            m_abilities[slotIndex] = new AbilityData(ability);
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_added_spell");
            UpdateAbilities();

            return true;
        }

        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_book_full");
        return false;
    }
    public class AbilityData
    {
        public readonly Talent m_talentData;
        public GameObject? m_gameObject;

        public void Update()
        {
            if (!Player.m_localPlayer) return;
            try
            {
                if (m_gameObject is null) return;
                if (!m_gameObject.TryGetComponent(out SpellElement component)) return;
                if (AbilityManager.m_cooldownMap.TryGetValue(m_talentData.m_key, out float ratio))
                {
                    if (m_talentData.m_status is not { } status)
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

                    int cooldownTime = (int)(m_talentData.GetCooldown(m_talentData.GetLevel()) * ratio);
                    //component.SetTimer(GetCooldownColored(cooldownTime));
                    component.SetFillAmount(ratio);
                    if (cooldownTime <= 0)
                    {
                        //component.SetTimer("");
                        //component.SetTimer(Localization.instance.Localize("$info_ready"));
                    }
                }
                else
                {
                    component.SetBorder(0f);
                    component.SetFillAmount(0f);
                    //component.SetTimer(Localization.instance.Localize("$info_ready"));
                    //component.SetTimer("");
                }
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to update ability cooldown");
            }
        }
        public AbilityData(Talent talent)
        {
            m_talentData = talent;
            talent.m_abilityData = this;
        }
    }
}

