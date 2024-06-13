using System;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace AlmanacClasses.UI;

public class SpellElementChange : MonoBehaviour, IPointerEnterHandler
{
    private static GameObject? title;
    private static GameObject? element;
    private static int fromIndex;
    
    public AbilityData data = null!;
    public int index;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            if (!TalentBook.IsTalentBookVisible())
            {
                LoadUI.MenuInfoPanel.SetActive(!element);
                Utils.FindChild(LoadUI.MenuInfoPanel.transform, "$text_name").GetComponent<Text>().text = Localization.instance.Localize($"<color=orange>{data.m_data.GetName()}</color>");
                Utils.FindChild(LoadUI.MenuInfoPanel.transform, "$text_description").GetComponent<Text>().text = data.m_data.GetTooltip();
            }
            else
            {
                if (title) Destroy(title);
                title = Instantiate(LoadUI.SpellBarHoverName, Hud.instance.transform, false);
                title.GetComponent<Text>().text = Localization.instance.Localize($"<color=orange>{data.m_data.GetName()}</color>");
                title.transform.position = transform.position + new Vector3(0f, 60f);
            }

            if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
            if (element != null)
            {
                Destroy(element);
                (SpellBook.m_abilities[fromIndex], SpellBook.m_abilities[index]) = (SpellBook.m_abilities[index], SpellBook.m_abilities[fromIndex]);
                if (title) Destroy(title);
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftAlt)) return;
                element = Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_element"), Hud.instance.transform, false);
                element.transform.SetAsFirstSibling();
                fromIndex = index;
                Utils.FindChild(element.transform, "$image_icon").GetComponent<Image>().sprite = data.m_data.GetSprite();
                if (title) Destroy(title);
            }
        }
        catch
        {
            // ignored
        }
    }

    public static void UpdateSpellMouseElement()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) DestroyElement();
        if (element != null) element.transform.position = Input.mousePosition + new Vector3(35f, 35f);
    }

    public static void DestroyElement()
    {
        if (element) Destroy(element);
        if (LoadUI.MenuInfoPanel) LoadUI.MenuInfoPanel.SetActive(false);
        if (title) Destroy(title);
    }
}