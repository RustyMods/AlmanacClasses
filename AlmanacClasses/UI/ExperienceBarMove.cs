using UnityEngine;
using UnityEngine.EventSystems;

namespace AlmanacClasses.UI;

public class ExperienceBarMove : MonoBehaviour, IPointerClickHandler
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
            AlmanacClassesPlugin._ExperienceBarPos.Value = Input.mousePosition;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        updateElement = !updateElement;
    }
}