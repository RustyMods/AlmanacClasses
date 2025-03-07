using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;
using UnityEngine.UI;

namespace AlmanacClasses.Patches;

public static class ButtonPatches
{
    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnSelect))]
    private static class Button_OnSelect_Patch
    {
        private static bool Prefix(Selectable __instance) => !TalentButton.IsTalentButton(__instance);
        
    }
}