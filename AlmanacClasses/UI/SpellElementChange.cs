using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;
public class SpellElementChange : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static int? selectedSpellIndex;
    private static GameObject? selectedSpellObject;
    private static GameObject? draggedSpellImage;
    public static GameObject? title;

    public AbilityData data = null!;
    public int index;
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
                SpellBarMove.updateElement = true;
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

        if (LoadUI.MenuInfoPanel) LoadUI.MenuInfoPanel.SetActive(false);
        if (title) Destroy(title);
    }

    private void ShowSpellInfo()
    {
        try
        {
            if (!TalentBook.IsTalentBookVisible())
            {
                LoadUI.MenuInfoPanel.SetActive(true);
                var nameText = Utils.FindChild(LoadUI.MenuInfoPanel.transform, "$text_name").GetComponent<Text>();
                var descriptionText = Utils.FindChild(LoadUI.MenuInfoPanel.transform, "$text_description").GetComponent<Text>();
                if (nameText != null && descriptionText != null)
                {
                    nameText.text = Localization.instance.Localize($"<color=orange>{data.m_data.GetName()}</color>");
                    descriptionText.text = data.m_data.GetTooltip();
                }
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
        if (LoadUI.MenuInfoPanel) LoadUI.MenuInfoPanel.SetActive(false);
        if (title) Destroy(title);
    }

    private void Update()
    {
        if (draggedSpellImage != null)
        {
            draggedSpellImage.transform.position = Input.mousePosition + new Vector3(35f, 35f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelSpellExchange();
        }
    }

    private void CancelSpellExchange()
    {
        selectedSpellIndex = null;
        selectedSpellObject = null;

        if (draggedSpellImage != null)
        {
            Destroy(draggedSpellImage);
            draggedSpellImage = null;
        }

        if (LoadUI.MenuInfoPanel) LoadUI.MenuInfoPanel.SetActive(false);
        if (title) Destroy(title);
    }
}

