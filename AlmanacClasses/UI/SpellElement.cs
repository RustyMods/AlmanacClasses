using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

/// <summary>
/// Container to hold the spell elements
/// In order to change their info
/// Instantiated in the spell book content list
/// </summary>
public class SpellElement : MonoBehaviour
{
    private static readonly List<SpellElement> m_instances = new();
    public Image m_icon = null!;
    public Text m_hotkey = null!;
    public Image m_gray = null!;
    public Image m_fill = null!;
    public Text m_timer = null!;

    public Text[] m_texts = null!;
    public void Awake()
    {
        m_icon = Utils.FindChild(transform, "$image_icon").GetComponent<Image>();
        m_hotkey = Utils.FindChild(transform, "$text_hotkey").GetComponent<Text>();
        m_gray = Utils.FindChild(transform, "$image_gray").GetComponent<Image>();
        m_fill = Utils.FindChild(transform, "$image_fill").GetComponent<Image>();
        m_timer = Utils.FindChild(transform, "$text_timer").GetComponent<Text>();
        m_texts = GetComponentsInChildren<Text>();
        m_instances.Add(this);
    }

    public void SetIcon(Sprite? sprite) => m_icon.sprite = sprite;
    public void SetHotkey(string text) => m_hotkey.text = text;
    public void SetGrayAmount(float amount) => m_gray.fillAmount = amount;
    public void SetFillAmount(float amount) => m_fill.fillAmount = amount;
    public void SetTimer(string text) => m_timer.text = text;

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