using System.Collections.Generic;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class InventoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static readonly List<InventoryButton> m_instances = new();
    public Image m_border = null!;
    public Image m_icon = null!;
    public Text m_name = null!;
    public Button m_button = null!;
    public Talent m_talent = null!;
    
    public void Init()
    {
        m_button = GetComponent<Button>();
        m_border = transform.Find("$image_border").GetComponent<Image>();
        m_icon = transform.Find("$image_icon").GetComponent<Image>();
        m_name = transform.Find("$text_name").GetComponent<Text>();
        m_name.maskable = false;
        m_name.gameObject.SetActive(false);
        m_instances.Add(this);
    }
    
    public void OnDestroy()
    {
        m_instances.Remove(this);
    }
    public void SetIcon(Sprite? sprite) => m_icon.sprite = sprite;
    public void SetName(string text) => m_name.text = Localization.instance.Localize(text);
    public void SetBorder(bool enable) => m_border.fillAmount = enable ? 1f : 0f;
    public void OnClick(UnityAction action) => m_button.onClick.AddListener(action);

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_name.gameObject.SetActive(true);
        SkillTree.m_instance.SetSelectedName(m_talent.GetName() + " $text_lvl " + m_talent.m_level);
        SkillTree.m_instance.SetSelectedDescription(m_talent.GetTooltip());
        SkillTree.m_instance.SetSelectedCost($"$almanac_cost: <color=orange>{m_talent.GetCost()}</color>");
        SkillTree.m_instance.SetSelectedType(m_talent.GetTalentType());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_name.gameObject.SetActive(false);
        SkillTree.m_instance.SetDefaultTooltip();
    }
    public static void OnFontChange(Font? font)
    {
        foreach (var instance in m_instances)
        {
            instance.m_name.font = font;
        }
    }
}