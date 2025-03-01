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
            ExperienceBar.m_updatePosition = false;
            SpellBook.m_updatePosition = false;
            PassiveBar.m_updatePosition = false;
            SpellInfo.m_instance.Hide();
            PassiveBar.m_instance.Hide();
        }
    }

    [HarmonyPatch(typeof(Menu), nameof(Menu.Show))]
    private static class Menu_Show_Patch
    {
        private static void Postfix()
        {
            PassiveBar.m_instance.Show();
        }
    }
}