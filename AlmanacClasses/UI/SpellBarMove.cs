using UnityEngine;

namespace AlmanacClasses.UI;

/// <summary>
/// Allows to move the spell bar
/// </summary>
public class SpellBarMove : MonoBehaviour
{
    public static bool updateElement;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) updateElement = false;

        if (!updateElement) return;
        AlmanacClassesPlugin._SpellBookPos.Value = Input.mousePosition;
        SpellInfo.m_instance.SetPosition(AlmanacClassesPlugin._SpellBookPos.Value + AlmanacClassesPlugin._MenuTooltipPosition.Value);
        if (SpellInfo.m_instance.IsVisible()) SpellInfo.m_instance.SetMenuVisible(false);
        if (Input.GetKeyDown(KeyCode.Mouse0)) updateElement = false;
    }
}