using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

/// <summary>
/// Container to hold the spell elements
/// In order to change their info
/// Instantiated in the spell book content list
/// </summary>
public class SpellElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // List of instantiated objects
    // TODO: This is barely used, set font during Awake() instead?
    private static readonly List<SpellElement> m_instances = new();
    // Variables used for UI manipulation
    private static int? m_selectedSpellIndex;
    private static bool m_isMovingSpell;
    private static SpellElement? m_targetSpellElement;
    private static GameObject? m_selectedSpellObject;
    private static GameObject? m_draggedSpellImage;

    // Prefab GameObjects
    public RectTransform m_rect = null!;
    public Image m_icon = null!;
    public Text m_hotkey = null!;
    public Image m_gray = null!;
    public Image m_fill = null!;
    public Text m_timer = null!;
    public Text m_title = null!;
    public Text[] m_texts = null!;
    public SpellBook.AbilityData? m_AbilityData = null!;
    
    // Position in the spellbook
    public int m_index;
    
    public void Awake()
    {
        m_rect = GetComponent<RectTransform>();
        m_icon = Utils.FindChild(transform, "$image_icon").GetComponent<Image>();
        m_hotkey = Utils.FindChild(transform, "$text_hotkey").GetComponent<Text>();
        m_gray = Utils.FindChild(transform, "$image_gray").GetComponent<Image>();
        m_fill = Utils.FindChild(transform, "$image_fill").GetComponent<Image>();
        m_timer = Utils.FindChild(transform, "$text_timer").GetComponent<Text>();
        m_title = Utils.FindChild(transform, "$text_title").GetComponent<Text>();
        m_texts = GetComponentsInChildren<Text>();
        HideName();
        m_instances.Add(this);
    }
    private void Update()
    {
        if (m_isMovingSpell && m_draggedSpellImage != null)
            m_draggedSpellImage.transform.position = Input.mousePosition;

        if (m_isMovingSpell && m_targetSpellElement == null && Input.GetMouseButtonUp(0)|| m_isMovingSpell && Input.GetKeyDown(KeyCode.Escape))
            CancelSpellExchange();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO: Right-click to remove from spellbook?
        if (eventData.button != PointerEventData.InputButton.Left || Input.GetKey(KeyCode.LeftAlt))
            return;
        
        // User tried to drop the spell on its original position, cancel it
        if (m_isMovingSpell && m_selectedSpellObject != null && m_selectedSpellObject.name == gameObject.name)
        {
            CancelSpellExchange();
            ShowSpellInfo();
            m_targetSpellElement = this;
            return;
        }
        
        // User clicked a spell slot
        if (!m_isMovingSpell && m_targetSpellElement != null)
        {
            // Prevent user from trying to select an empty spell slot
            if (!SpellBook.TryGetSpellElement(eventData, out var spellElement)) return;
            if (!SpellBook.IsAbilitySlotInUse(spellElement)) return;
            
            SelectSpell();
        }
        
        // User clicked a different spell slot, whilst dragging one - move them
        if (m_isMovingSpell && m_selectedSpellObject != null && m_selectedSpellObject.name != gameObject.name)
        {
            SwitchSpellSlots();
            ShowSpellInfo();
            m_targetSpellElement = this;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftAlt) && !m_isMovingSpell)
            ShowSpellInfo();

        if (SpellBook.TryGetSpellElement(eventData, out var spellElement))
        {
            m_targetSpellElement = spellElement;
            m_icon.rectTransform.sizeDelta *= 1.15f;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        SpellInfo.m_instance.Hide();
        m_targetSpellElement = null;
        m_icon.rectTransform.sizeDelta /= 1.15f;
        HideName();
    }
    
    private void SelectSpell()
    {
        if (m_AbilityData?.m_talentData.GetSprite() == null)
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogError($"[SpellElement.SelectSpell()]: Unable to fetch sprite for ability!");
            return;
        }

        m_selectedSpellIndex = m_index;
        m_selectedSpellObject = gameObject;

        m_draggedSpellImage = new GameObject("SpellDummyIcon");
        m_draggedSpellImage.transform.SetParent(Hud.instance.transform, false);
        var image = m_draggedSpellImage.AddComponent<Image>();
        image.sprite = m_AbilityData.m_talentData.GetSprite();
        image.raycastTarget = false;

        var rectTransform = m_draggedSpellImage.GetComponent<RectTransform>();
        var originalRectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = (originalRectTransform.sizeDelta * 0.75f);
        
        m_isMovingSpell = true;
    }
    
    private void SwitchSpellSlots()
    {
        if (m_selectedSpellIndex == null || m_selectedSpellObject == null) return;

        var fromIndex = m_selectedSpellIndex.Value;
        var toIndex = m_index;

        // Move the spells around
        if (SpellBook.m_abilities.ContainsKey(toIndex))
        {
            (SpellBook.m_abilities[fromIndex], SpellBook.m_abilities[toIndex]) = (SpellBook.m_abilities[toIndex], SpellBook.m_abilities[fromIndex]);
        }
        else
        {
            SpellBook.m_abilities[toIndex] = SpellBook.m_abilities[fromIndex];
            SpellBook.m_abilities.Remove(fromIndex);
            SpellBook.ResetAbilitySlot(fromIndex);
        }
        
        // Reset variables
        CancelSpellExchange();
        SpellBook.UpdateAbilities();
        SpellInfo.m_instance.Hide();
    }
    
    private static void CancelSpellExchange()
    {
        if (m_draggedSpellImage != null)
        {
            Destroy(m_draggedSpellImage);
            m_draggedSpellImage = null;
        }
        
        m_selectedSpellIndex = null;
        m_selectedSpellObject = null;
        m_isMovingSpell = false;
        m_targetSpellElement = null;
        SpellInfo.m_instance.Hide();
    }
    
    private void ShowSpellInfo()
    {
        if (m_AbilityData == null) return;
        
        if (!SkillTree.IsPanelVisible())
        {
            SpellInfo.m_instance.Show();
            SpellInfo.m_instance.SetName(Localization.instance.Localize($"<color=orange>{m_AbilityData.m_talentData.GetName()}</color>"));
            SpellInfo.m_instance.SetDescription(m_AbilityData.m_talentData.GetTooltip());
        }
        else
        {
            ShowName();
        }
    }
    
    public void SetIcon(Sprite? sprite) => m_icon.sprite = sprite;
    public void SetIconVisibility(bool isEnabled) => m_icon.enabled = isEnabled;
    public void SetHotkey(string text) => m_hotkey.text = text;
    public void SetBorder(float amount) => m_gray.fillAmount = amount;
    public void SetFillAmount(float amount) => m_fill.fillAmount = amount;
    public void SetTimer(string text) => m_timer.text = text;
    public void SetName(string text) => m_title.text = text;
    public void ShowName() => m_title.gameObject.SetActive(true);
    public void HideName() => m_title.gameObject.SetActive(false);
    public void SetFont(Font? font)
    {
        foreach (var text in m_texts) text.font = font;
    }
    public static void UpdateFont(Font? font)
    {
        foreach (var instance in m_instances)
        {
            instance.SetFont(font);
        }
    }
}