using System;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AlmanacClasses.UI;

public class PassiveBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static PassiveBar m_instance = null!;
    public static GameObject m_element = null!;
    public RectTransform m_rect = null!;
    public RectTransform m_contentList = null!;
    
    private static bool m_isDragging;
    
    public void Init()
    {
        m_instance = this;
        m_rect = GetComponent<RectTransform>();
        m_contentList = transform.Find("$part_content").GetComponent<RectTransform>();
        m_element = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("PassiveBar_element");
        m_element.AddComponent<PassiveButton>();
        m_rect.anchoredPosition = AlmanacClassesPlugin._PassiveBarPos.Value;
        m_element.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
        Hide();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!(Menu.IsVisible() ^ SkillTree.IsPanelVisible())) return;
        if (eventData.button is not PointerEventData.InputButton.Left) return;
        
        m_isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_rect.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AlmanacClassesPlugin._PassiveBarPos.Value = m_rect.position;
        m_isDragging = false;
    }
    
    public static void ResetUI()
    {
        var defaultPos = (Vector2)AlmanacClassesPlugin._PassiveBarPos.DefaultValue;
        m_instance.m_rect.position = defaultPos;
        
        AlmanacClassesPlugin._PassiveBarPos.Value = defaultPos;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    public static void OnPassiveBarPosChange(object sender, EventArgs e)
    {
        if (!m_instance) return;
        m_instance.m_rect.position = AlmanacClassesPlugin._PassiveBarPos.Value;
    }
    
    public void Clear()
    {
        foreach (Transform instance in m_contentList)
        {
            Destroy(instance.gameObject);
        }
    }

    public void Add(Talent talent)
    {
        var component = Instantiate(m_element, m_contentList).GetComponent<PassiveButton>();
        component.Init();
        component.m_talent = talent;
        component.SetIcon(talent.m_sprite);
        component.SetBorder(talent.m_passiveActive);
        component.SetName(Localization.m_instance.Localize(talent.GetName()));
        component.OnClick(() =>
        {
            if (m_isDragging)
                return;

            if (talent.m_onClickPassive?.Invoke() ?? false)
            {
                component.SetBorder(talent.m_passiveActive);
            }
        });
        component.ShouldUpdate(talent.m_onClickPassive is null);
    }
}