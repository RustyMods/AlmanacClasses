using System;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

/// <summary>
/// Container which controls the experience bar
/// </summary>
public class ExperienceBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static ExperienceBar m_instance = null!;
    
    public RectTransform m_rect = null!;
    public Image m_fillBar = null!;
    public Text m_text = null!;
    public Text[] m_texts = null!;
    
    public void Init()
    {
        m_instance = this;
        SetHUDVisibility(false);
        m_rect = GetComponent<RectTransform>();
        m_rect.SetAsLastSibling();
        m_rect.position = AlmanacClassesPlugin._ExperienceBarPos.Value;

        SetScale(AlmanacClassesPlugin._ExperienceBarScale.Value / 100f);
        m_fillBar = transform.Find("FillBar").GetComponent<Image>();
        m_text = transform.Find("$text_experience").GetComponent<Text>();
        SetFill(0f);
        SetText("");
        m_texts = GetComponentsInChildren<Text>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!(Menu.IsVisible() ^ SkillTree.IsPanelVisible())) return;
        if (eventData.button is not PointerEventData.InputButton.Left) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_rect.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AlmanacClassesPlugin._ExperienceBarPos.Value = m_rect.position;
    }
    
    public static void ResetUI()
    {
        var defaultPos = (Vector2)AlmanacClassesPlugin._ExperienceBarPos.DefaultValue;
        var defaultScale = (float)AlmanacClassesPlugin._ExperienceBarScale.DefaultValue;
        m_instance.m_rect.position = defaultPos;
        m_instance.SetScale(defaultScale / 100f);
        
        AlmanacClassesPlugin._PassiveBarPos.Value = defaultPos;
        AlmanacClassesPlugin._ExperienceBarScale.Value = defaultScale;
    }

    public void SetFill(float amount) => m_fillBar.fillAmount = amount;
    public void SetText(string text) => m_text.text = text;
    public void SetScale(float scale) => m_rect.localScale = new Vector3(scale, scale, scale);

    public static void SetHUDVisibility(bool enable)
    {
        if (m_instance == null) return;
        m_instance.gameObject.SetActive(enable);
    }
    
    public static void UpdateExperienceBar()
    {
        // Called whenever experience changes, instead of updating in the background all the time.
        if (m_instance == null) return;
        int experience = PlayerManager.GetExperience();
        int level = PlayerManager.GetPlayerLevel(experience);
        int nxtLvlExp = PlayerManager.GetRequiredExperience(level + 1);
        int previousLvlExp = PlayerManager.GetRequiredExperience(level);
        float difference = nxtLvlExp - previousLvlExp;
        float current = experience - previousLvlExp;
        float percentage = current / difference;
        m_instance.SetText($"{experience} / {nxtLvlExp}");
        m_instance.SetFill(percentage);
        SkillTree.m_instance.ExperienceBarFill.fillAmount = percentage;
    }
    
    public static void OnChangeExperienceBarVisibility(object sender, EventArgs e)
    {
        SetHUDVisibility(AlmanacClassesPlugin._HudVisible.Value is AlmanacClassesPlugin.Toggle.On);
    }
    
    public static void OnChangeExperienceBarPosition(object sender, EventArgs e)
    {
        if (m_instance == null) return;
        m_instance.m_rect.position = AlmanacClassesPlugin._ExperienceBarPos.Value;
    }
    
    public static void OnExperienceBarScaleChange(object sender, EventArgs e)
    {
        if (m_instance == null) return;
        m_instance.SetScale(AlmanacClassesPlugin._ExperienceBarScale.Value / 100f);
    }
}