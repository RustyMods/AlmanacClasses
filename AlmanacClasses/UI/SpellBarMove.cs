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
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftAlt)) return;
        updateElement = !updateElement;
    }
}