using UnityEngine;
using UnityEngine.EventSystems;

namespace AlmanacClasses.UI;

public static class SpellBarMove
{
    public static bool updateElement;

    public static void UpdateElement()
    {
        if (updateElement)
        {
            AlmanacClassesPlugin._SpellBookPos.Value = Input.mousePosition;
            SpellInfo.m_instance.SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + AlmanacClassesPlugin._MenuTooltipPosition.Value);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            updateElement = false;
        }
    }
}