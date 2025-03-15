using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class Berzerker
{
    [HarmonyPatch(typeof(Player), nameof(Player.GetBodyArmor))]
    private static class Player_GetBodyArmor_Postfix
    {
        private static void Postfix(ref float __result)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("Berzerk", out Talent talent)) return;
            __result += talent.GetArmor(talent.GetLevel());
        }
    }
}