using AlmanacClasses.Classes;
using AlmanacClasses.Data;
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
            int amount = 0;
            if (PlayerManager.m_playerTalents.TryGetValue("CoreComfort2", out Talent comfort2))
            {
                amount += (comfort2.m_comfortAmount?.Value ?? 2) * comfort2.m_level;
            }
            __result += amount;
        }
    }
}