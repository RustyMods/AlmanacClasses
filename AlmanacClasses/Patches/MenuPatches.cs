using AlmanacClasses.UI;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Patches;

public static class MenuPatches
{
    [HarmonyPatch(typeof(Menu), nameof(Menu.Hide))]
    private static class Menu_Hide_Patch
    {
        private static void Postfix()
        {
            // SpellElementChange.DestroyElement();
            ExperienceBarMove.updateElement = false;
            SpellBarMove.updateElement = false;
            if (LoadUI.MenuInfoPanel) LoadUI.MenuInfoPanel.SetActive(false);
            if (SpellElementChange.title) Object.Destroy(SpellElementChange.title);
        }
    }
}