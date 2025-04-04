﻿using System;
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
        m_rect.SetAsFirstSibling();
        m_rect.position = AlmanacClassesPlugin._ExperienceBarPos.Value;

        SetScale(AlmanacClassesPlugin._ExperienceBarScale.Value);
        m_fillBar = transform.Find("FillBar").GetComponent<Image>();
        m_text = transform.Find("$text_experience").GetComponent<Text>();
        SetFill(0f);
        SetText("");
        m_texts = GetComponentsInChildren<Text>();
    }
    
    private Vector3 mouseDifference = Vector3.zero;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        mouseDifference = m_rect.position - new Vector3(pos.x, pos.y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_rect.position = Input.mousePosition + mouseDifference;
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
        m_instance.SetScale(defaultScale);
        
        AlmanacClassesPlugin._ExperienceBarPos.Value = defaultPos;
        AlmanacClassesPlugin._ExperienceBarScale.Value = defaultScale;
    }

    public void SetFill(float amount) => m_fillBar.fillAmount = amount;
    public void SetText(string text) => m_text.text = text;
    public void SetScale(float scale) => m_rect.localScale = (new Vector3(scale, scale, scale) / 100f);
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
        m_instance.SetScale(AlmanacClassesPlugin._ExperienceBarScale.Value);
    }
    
    /// <summary>
    /// Checks if the UI is visible, if it has been moved off-screen
    /// </summary>
    public static bool IsVisible()
    {
        return m_instance.m_rect.position.x < Screen.width;
    }
    
    /// <summary>
    /// Sets 'visibility' by moving it off-screen, like Valheim does. Thus it can keep ticking.
    /// </summary>
    public static void SetVisible(bool shouldBeVisible)
    {
        if (IsVisible() == shouldBeVisible)
            return;
        
        if (shouldBeVisible)
            m_instance.m_rect.position = AlmanacClassesPlugin._ExperienceBarPos.Value;
        else
            m_instance.m_rect.position = Hud.s_notVisiblePosition;
    }
}