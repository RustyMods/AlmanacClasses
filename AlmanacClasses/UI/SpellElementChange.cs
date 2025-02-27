﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;
/// <summary>
/// Allows user to manipulate spell book spell indexes
/// To place the spells in the slots of their choice
/// Additionally, since it records its index, it is used when updating the cooldowns
/// to make sure only the casted spells are updating their visuals
/// </summary>
public class SpellElementChange : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static int? selectedSpellIndex;
    private static GameObject? selectedSpellObject;
    private static GameObject? draggedSpellImage;
    public static GameObject? title;

    public SpellBook.AbilityData data = null!;
    public int index;
    
    private void Update()
    {
        if (draggedSpellImage is not null)
        {
            draggedSpellImage.transform.position = Input.mousePosition + new Vector3(35f, 35f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSpellExchange();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectedSpellIndex == null)
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                SelectSpell();
            } 
            else
            {
                SpellBarMove.updateElement = !SpellBarMove.updateElement;
            }
        }
        else
        {
            MoveSpell();
        }
    }
    private void SelectSpell()
    {
        if (selectedSpellObject != null)
        {
            var previousImage = selectedSpellObject.GetComponent<Image>();
            if (previousImage != null)
            {
                previousImage.color = Color.white;
            }
        }

        selectedSpellIndex = index;
        selectedSpellObject = gameObject;

        draggedSpellImage = new GameObject("DraggedSpell");
        draggedSpellImage.transform.SetParent(Hud.instance.transform, false);
        var image = draggedSpellImage.AddComponent<Image>();
        if (data.m_data.GetSprite() == null) return;
        image.sprite = data.m_data.GetSprite();
        image.raycastTarget = false;

        RectTransform rectTransform = draggedSpellImage.GetComponent<RectTransform>();
        var originalRectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = originalRectTransform.sizeDelta;
    }
    private void MoveSpell()
    {
        if (selectedSpellIndex == null || selectedSpellObject == null) return;

        int fromIndex = (int)selectedSpellIndex;
        int toIndex = index;

        (SpellBook.m_abilities[fromIndex], SpellBook.m_abilities[toIndex]) = (SpellBook.m_abilities[toIndex], SpellBook.m_abilities[fromIndex]);

        selectedSpellIndex = null;
        selectedSpellObject = null;

        if (draggedSpellImage != null)
        {
            Destroy(draggedSpellImage);
            draggedSpellImage = null;
        }

        SpellBook.UpdateAbilities();

        SpellInfo.m_instance.SetMenuVisible(false);
        if (title) Destroy(title);
    }
    private void ShowSpellInfo()
    {
        try
        {
            if (!TalentBook.IsTalentBookVisible())
            {
                SpellInfo.m_instance.SetMenuVisible(true);
                SpellInfo.m_instance.SetName(Localization.instance.Localize($"<color=orange>{data.m_data.GetName()}</color>"));
                SpellInfo.m_instance.SetDescription(data.m_data.GetTooltip());
            }
            else
            {
                if (title) Destroy(title);
                title = Instantiate(LoadUI.SpellBarHoverName, Hud.instance.transform, false);
                var titleText = title.GetComponent<Text>();
                if (titleText != null)
                {
                    titleText.text = Localization.instance.Localize($"<color=orange>{data.m_data.GetName()}</color>");
                    title.transform.position = transform.position + new Vector3(0f, 60f);
                }
            }
        }
        catch
        {
            // ignored
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftAlt) && draggedSpellImage == null)
        {
            ShowSpellInfo();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SpellInfo.m_instance.SetMenuVisible(false);
        if (title) Destroy(title);
    }
    private static void CancelSpellExchange()
    {
        selectedSpellIndex = null;
        selectedSpellObject = null;

        if (draggedSpellImage != null)
        {
            Destroy(draggedSpellImage);
            draggedSpellImage = null;
        }

        SpellInfo.m_instance.SetMenuVisible(false);
        if (title) Destroy(title);
    }
}

