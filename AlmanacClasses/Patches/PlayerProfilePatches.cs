using System.Collections.Generic;
using AlmanacClasses.Classes;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class PlayerProfilePatches
{
    [HarmonyPatch(typeof(PlayerProfile), nameof(PlayerProfile.IncrementStat))]
    private static class PlayerProfile_IncrementStat_Patch
    {
        private static void Postfix(PlayerStatType stat, float amount) => ExperienceManager.AddIncrementExperience(stat, amount);
    }
}