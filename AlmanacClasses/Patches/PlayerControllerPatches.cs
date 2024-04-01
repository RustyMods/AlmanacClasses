using AlmanacClasses.UI;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Patches;

public static class PlayerControllerPatches
{
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.FixedUpdate))]
    private static class PlayerController_FixedUpdate_Patch
    {
        private static bool Prefix(PlayerController __instance)
        {
            if (!TalentBook.IsTalentBookVisible()) return true;
            __instance.m_character.SetControls(Vector3.zero, false, false, false, false, false, false, false, false, false, false);
            return false;
        }
    }
}