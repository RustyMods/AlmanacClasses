using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlmanacClasses.UI;

/// <summary>
/// Allows to move the experience bar
/// </summary>
public class ExperienceBarMove : MonoBehaviour, IPointerClickHandler
{
    public static bool updateElement;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) updateElement = false;

        if (!updateElement) return;
        AlmanacClassesPlugin._ExperienceBarPos.Value = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        updateElement = !updateElement;
    }
}