using System;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class PassiveBar : MonoBehaviour
{
    public static PassiveBar m_instance = null!;
    public static bool m_updatePosition;
    public static GameObject m_element = null!;

    public RectTransform m_rect = null!;
    public RectTransform m_contentList = null!;
    
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
    public void Update()
    {
        if (!m_instance) return;
        if (Input.GetKeyDown(KeyCode.Escape)) m_updatePosition = false;
        if (!m_updatePosition) return;
        AlmanacClassesPlugin._PassiveBarPos.Value = Input.mousePosition;
        if (Input.GetKeyDown(KeyCode.Mouse0)) m_updatePosition = false;
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
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                m_updatePosition = !m_updatePosition;
            }
            else
            {
                if (talent.m_onClickPassive?.Invoke() ?? false)
                {
                    component.SetBorder(talent.m_passiveActive);
                }
            }
        });
        component.ShouldUpdate(talent.m_onClickPassive is null);
    }
}