using HarmonyLib;
using AlmanacClasses.UI;

namespace AlmanacClasses.Patches;

public static class PlayerPatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    private static class Player_OnSpawned_Patch
    {
        private static void Postfix(Character __instance)
        {
            if (!__instance || !Hud.m_instance) return;
                LoadUI.InitHud(Hud.m_instance);
        }
    }
}