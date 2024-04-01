using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class GamePatches
{
    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    private static class Game_Logout_Prefix_Patch
    {
        private static void Prefix()
        {
            LoadUI.m_initLineFillSet = false;
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    private static class Game_Logout_Postfix_Patch
    {
        private static void Postfix()
        {
            PlayerManager.m_tempPlayerData = new();
            PlayerManager.m_playerTalents.Clear();
            SpellBook.m_abilities.Clear();
            PlayerPatches.initiated = false;
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Game logout: Clearing temporary player talent data");
        }
    }
}