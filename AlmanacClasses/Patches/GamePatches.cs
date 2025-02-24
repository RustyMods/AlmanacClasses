using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class GamePatches
{
    /// <summary>
    /// Make sure the class system cleans up so when user re-logs, the system is refreshed to initial states
    /// </summary>
    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    private static class Game_Logout_Patches
    {
        private static void Postfix()
        {
            CharacteristicManager.OnLogout();
            SpellBook.OnLogout();
            PlayerManager.OnLogout();
            LoadUI.OnLogout();
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Game logout: Clearing temporary player talent data");
        }
    }
}