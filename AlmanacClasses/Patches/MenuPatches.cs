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
            if (SpellInfo.m_instance != null)
                SpellInfo.m_instance.Hide();
            
            if (PassiveBar.m_instance != null)
                PassiveBar.m_instance.Hide();
        }
    }

    [HarmonyPatch(typeof(Menu), nameof(Menu.Show))]
    private static class Menu_Show_Patch
    {
        private static void Postfix()
        {
            if (PassiveBar.m_instance != null)
                PassiveBar.m_instance.Show();
        }
    }
}