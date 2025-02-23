using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class HudPatches
{
    [HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
    private static class Hud_Awake_Patch
    {
        private static void Postfix(Hud __instance)
        {
            if (!__instance) return;
            LoadUI.InitHud(__instance);
        }
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.Update))]
    private static class Hud_Update_Patch
    {
        private static void Postfix(Hud __instance)
        {
            if (!__instance || ! Player.m_localPlayer) return;
            if (Player.m_localPlayer.IsTeleporting() || Player.m_localPlayer.IsDead() || Player.m_localPlayer.IsSleeping()) return;
            // LoadUI.UpdateExperienceBarHud();
            SpellBook.UpdateAbilities();
        }
    }
}