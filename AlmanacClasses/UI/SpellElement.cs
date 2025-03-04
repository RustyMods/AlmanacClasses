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
    private static readonly List<SpellElement> m_instances = new();
    private static int? m_selectedSpellIndex;
    private static GameObject? m_selectedSpellObject;
    private static GameObject? m_draggedSpellImage;
    
    public Image m_icon = null!;
    public Text m_hotkey = null!;
    public Image m_gray = null!;
    public Image m_fill = null!;
    public Text m_timer = null!;
    public Text m_title = null!;
    public Text[] m_texts = null!;
    public SpellBook.AbilityData m_data = null!;
    public int m_index;
    public void Awake()
    {
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
        if (m_draggedSpellImage is not null)
        {
            m_draggedSpellImage.transform.position = Input.mousePosition + new Vector3(35f, 35f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSpellExchange();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_selectedSpellIndex == null)
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                SelectSpell();
            } 
        }
        else
        {
            MoveSpell();
        }
    }
    private void SelectSpell()
    {
        if (m_selectedSpellObject != null)
        {
            var previousImage = m_selectedSpellObject.GetComponent<Image>();
            if (previousImage != null)
            {
                previousImage.color = Color.white;
            }
        }

        m_selectedSpellIndex = m_index;
        m_selectedSpellObject = gameObject;

        m_draggedSpellImage = new GameObject("DraggedSpell");
        m_draggedSpellImage.transform.SetParent(Hud.instance.transform, false);
        var image = m_draggedSpellImage.AddComponent<Image>();
        if (m_data.m_data.GetSprite() == null) return;
        image.sprite = m_data.m_data.GetSprite();
        image.raycastTarget = false;

        RectTransform rectTransform = m_draggedSpellImage.GetComponent<RectTransform>();
        var originalRectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = originalRectTransform.sizeDelta;
    }
    private void MoveSpell()
    {
        if (m_selectedSpellIndex == null || m_selectedSpellObject == null) return;

        int fromIndex = (int)m_selectedSpellIndex;
        int toIndex = m_index;

        (SpellBook.m_abilities[fromIndex], SpellBook.m_abilities[toIndex]) = (SpellBook.m_abilities[toIndex], SpellBook.m_abilities[fromIndex]);

        m_selectedSpellIndex = null;
        m_selectedSpellObject = null;

        if (m_draggedSpellImage != null)
        {
            Destroy(m_draggedSpellImage);
            m_draggedSpellImage = null;
        }

        SpellBook.UpdateAbilities();
        SpellInfo.m_instance.Hide();
    }
    private void ShowSpellInfo()
    {
        if (!SkillTree.IsPanelVisible())
        {
            SpellInfo.m_instance.Show();
            SpellInfo.m_instance.SetName(Localization.instance.Localize($"<color=orange>{m_data.m_data.GetName()}</color>"));
            SpellInfo.m_instance.SetDescription(m_data.m_data.GetTooltip());
        }
        else
        {
            ShowName();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftAlt) && m_draggedSpellImage == null)
        {
            ShowSpellInfo();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SpellInfo.m_instance.Hide();
        HideName();
    }
    private static void CancelSpellExchange()
    {
        m_selectedSpellIndex = null;
        m_selectedSpellObject = null;

        if (m_draggedSpellImage is not null)
        {
            Destroy(m_draggedSpellImage);
            m_draggedSpellImage = null;
        }

        SpellInfo.m_instance.Hide();
    }

    public static bool IsMovingSpell() => m_draggedSpellImage is not null;
    public void SetIcon(Sprite? sprite) => m_icon.sprite = sprite;
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