using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.Classes.Abilities.Warrior;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class CharacterPatches
{
    [HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
    private static class Character_OnDeath_Patch
    {
        private static void Prefix(Character __instance)
        {
            if (!__instance || !Player.m_localPlayer) return;
            if (__instance.IsPlayer()) return;
            if (!__instance.m_nview) return;
            if (!__instance.m_nview.IsValid()) return;
            if (__instance.m_nview.GetZDO().GetBool(Classes.Abilities.SpawnSystem.FriendlyKey)) return;
            ExperienceManager.AddExperience(__instance);
            ExperienceManager.DropOrb(__instance);
            Pickpocket.CheckDoubleLoot(__instance);
            BattleFury.CheckBattleFury(__instance);
        }
    }
}