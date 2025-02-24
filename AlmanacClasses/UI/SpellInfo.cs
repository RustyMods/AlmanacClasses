using System;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class SpellInfo : MonoBehaviour
{
    /// <summary>
    /// The element over spells when hovering.
    /// </summary>
    public static SpellInfo m_instance = null!;
    public Text[] m_texts = null!;

    public Text m_name = null!;
    public Text m_description = null!;

    public void Init()
    {
        m_instance = this;
        m_texts = GetComponentsInChildren<Text>();
        transform.SetAsFirstSibling();
        SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + AlmanacClassesPlugin._MenuTooltipPosition.Value);
        SetMenuVisible(false);
        m_name = Utils.FindChild(transform, "$text_name").GetComponent<Text>();
        m_description = Utils.FindChild(transform, "$text_description").GetComponent<Text>();
    }

    public void SetPosition(Vector3 pos) => transform.position = pos;
    public void SetMenuVisible(bool enable) => gameObject.SetActive(enable);
    public bool IsVisible() => gameObject.activeInHierarchy;
    public void SetName(string text) => m_name.text = text;
    public void SetDescription(string text) => m_description.text = text;
    public static void OnSpellInfoPositionChange(object sender, EventArgs e)
    {
        m_instance.SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + AlmanacClassesPlugin._MenuTooltipPosition.Value);
    }
}