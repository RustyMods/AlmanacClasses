using AlmanacClasses.Classes;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Patches;

public static class SE_Rested_Patches
{
    [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.CalculateComfortLevel),typeof(Player))]
    [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.CalculateComfortLevel),typeof(bool),typeof(Vector3))]
    private static class SE_Rested_CalculateComfortLevel_Patch
    {
        private static void Postfix(ref int __result)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("Comfort", out Talent talent)) return;
            __result += (int)talent.GetAddedComfort(talent.GetLevel());
        }
    }
}