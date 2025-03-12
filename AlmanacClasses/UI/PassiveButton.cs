using System.Collections.Generic;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class PassiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static readonly List<PassiveButton> m_instances = new();
    public Image m_border = null!;
    public Image m_icon = null!;
    public Text m_name = null!;
    public Button m_button = null!;
    public Talent m_talent = null!;
    public bool m_shouldUpdate = true;
    private float m_timer;
    public void Init()
    {
        m_button = GetComponent<Button>();
        m_border = transform.Find("$image_border").GetComponent<Image>();
        m_icon = transform.Find("$image_icon").GetComponent<Image>();
        m_name = transform.Find("$text_name").GetComponent<Text>();
        gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
        m_name.gameObject.SetActive(false);
        m_instances.Add(this);
    }

    public void OnDestroy()
    {
        m_instances.Remove(this);
    }

    // public void Update()
    // {
    //     if (!m_shouldUpdate) return;
    //     m_timer += Time.deltaTime;
    //     if (m_timer < 1f) return;
    //     m_timer = 0.0f;
    //     SetBorder(m_talent.m_passiveActive);
    // }

    public void SetIcon(Sprite? sprite) => m_icon.sprite = sprite;
    public void SetName(string text) => m_name.text = text;
    public void SetBorder(bool enable) => m_border.fillAmount = enable ? 1f : 0f;
    public void ShouldUpdate(bool enable) => m_shouldUpdate = enable;
    public void OnClick(UnityAction action) => m_button.onClick.AddListener(action);
    public void OnPointerEnter(PointerEventData eventData) => m_name.gameObject.SetActive(true);
    public void OnPointerExit(PointerEventData eventData) => m_name.gameObject.SetActive(false);
    public static void OnFontChange(Font? font)
    {
        foreach (var instance in m_instances)
        {
            instance.m_name.font = font;
        }
    }
}