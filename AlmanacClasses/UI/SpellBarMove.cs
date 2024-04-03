using UnityEngine;
using UnityEngine.EventSystems;

namespace AlmanacClasses.UI;

public class SpellBarMove : MonoBehaviour, IPointerClickHandler
{
    public static bool updateElement = false;

    public static void UpdateElement()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            updateElement = false;
        }

        if (updateElement)
        {
            AlmanacClassesPlugin._SpellBookPos.Value = Input.mousePosition;
            LoadUI.MenuInfoPanel.transform.position = AlmanacClassesPlugin._SpellBookPos.Value +
                                                      AlmanacClassesPlugin._MenuTooltipPosition.Value;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftAlt)) return;
        updateElement = !updateElement;
    }
}