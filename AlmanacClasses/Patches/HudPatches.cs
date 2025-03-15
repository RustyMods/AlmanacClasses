using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class HudPatches
{
    // [HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
    // private static class Hud_Awake_Patch
    // {
    //     private static void Postfix(Hud __instance)
    //     {
    //         if (!__instance) return;
    //         LoadUI.InitHud(__instance);
    //     }
    // }
}