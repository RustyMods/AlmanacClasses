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

    public static readonly Dictionary<int, SpellSlot> m_slots = new();
    
    public RectTransform m_rect = null!;
    public List<Text> m_elementTexts = new();
    private Transform m_contentList = null!;
    
    public void Init()
    {
        m_instance = this;
        m_rect = GetComponent<RectTransform>();
        m_rect.SetAsFirstSibling();
        m_rect.position = AlmanacClassesPlugin._SpellBookPos.Value;
        m_contentList = transform.Find("$part_content");
        for (int i = 0; i <= 7; ++i)
        {
            if (m_contentList.GetChild(i) is { } child)
            {
                var slot = child.gameObject.AddComponent<SpellSlot>();
                slot.Init();
                slot.m_index = i;
                m_slots[i] = slot;
                if (slot.m_texts != null) m_elementTexts.AddRange(slot.m_texts);
            }
            else
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogError($"Failed to find AbilitySlot {i+1}");
            }
        }
        UpdateFontSize();
    }
    
    public void Update()
    {
        if (!m_instance || !Player.m_localPlayer || Minimap.IsOpen()) return;
        if (Player.m_localPlayer.IsTeleporting() || Player.m_localPlayer.IsDead() || Player.m_localPlayer.IsSleeping()) return;
        if (SpellSlot.m_draggedSpellImage is null) return;
        
        SpellSlot.m_draggedSpellImage.transform.position = Input.mousePosition;
        if ((!Input.GetKeyDown(KeyCode.Mouse0) || SpellSlot.m_hoveringSlot) && !Input.GetKeyDown(KeyCode.Escape)) return;
        SpellSlot.m_selectedSlot = null;
        Destroy(SpellSlot.m_draggedSpellImage);
        SpellSlot.m_draggedSpellImage = null;
    }
    
    private Vector3 mouseDifference = Vector3.zero;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        mouseDifference = m_rect.position - new Vector3(pos.x, pos.y, 0);
    }
    public void OnDrag(PointerEventData eventData)
    {
        // I feel like this may be a redundant keybind, since moving spell slots relies on IPointer, this on IDrag interfaces.
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
    public static bool IsVisible() => m_instance.m_rect.position.x < Screen.width;
    
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
    public static void OnSpellBarPosChange(object sender, EventArgs e)
    {
        m_instance.m_rect.position = AlmanacClassesPlugin._SpellBookPos.Value;
        SpellInfo.m_instance.SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + new Vector2(0f, 150f));
    }
    public static void OnLogout() => ClearSpellBook();
    public static void ClearSpellBook()
    {
        foreach (var slot in m_slots.Values)
        {
            slot.SetAbility(null);
        }
    }
    
    public static bool IsAbilityInBook(Talent ability) => m_slots.Values.Any(slot => slot.m_talent == ability);
    
    public static void Remove(Talent ability)
    {
        foreach (var slot in m_slots.Values)
        {
            if (slot.m_talent != ability) continue;
            slot.SetAbility(null);
            return;
        }
    }

    /// <summary>
    /// Attempts to find a slot that is empty and can be used
    /// </summary>
    /// <param name="slotIndex">Index of the empty slot, -1 if none was found</param>
    /// <returns>true if a slot was successfully found, otherwise false</returns>
    private static bool TryFindFirstAvailableSlot(out int slotIndex)
    {
        slotIndex = -1;

        foreach (var slot in m_slots.Values)
        {
            if (slot.m_talent is null)
            {
                slotIndex = slot.m_index;
                return true;
            }
        }
        return false;
    }

    private static SpellSlot? TryFindAvailableSlot()
    {
        foreach (var slot in m_slots.Values)
        {
            if (slot.m_talent is null) return slot;
        }

        return null;
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

    public static int ActiveSlotCount()
    {
        int count = 0;
        foreach (var slot in m_slots.Values)
        {
            if (slot.m_talent != null) ++count;
        }

        return count;
    }
    
    public static bool Add(Talent ability)
    {
        if (IsAbilityInBook(ability))
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_in_book");
            return false;
        }
        
        if (ActiveSlotCount() > 7)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_book_full");
            return false;
        }

        if (TryFindAvailableSlot() is { } slot)
        {
            slot.SetAbility(ability);
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_added_spell");
            return true;
        }

        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_book_full");
        return false;
    }
}

