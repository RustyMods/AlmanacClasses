using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class GamePatches
{
    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    private static class Game_Logout_Patches
    {
        private static void Postfix()
        {
            PlayerManager.m_tempPlayerData = new();
            PlayerManager.m_playerTalents.Clear();
            SpellBook.m_abilities.Clear();
            PlayerManager.OnLogout();
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Game logout: Clearing temporary player talent data");
        }
        private static void Prefix()
        {
            LoadUI.OnLogout();
        }
    }
}