using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.Classes.Abilities.Rogue;
using AlmanacClasses.Classes.Abilities.Warrior;
using BepInEx;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Patches;

public static class CharacterPatches
{
    [HarmonyWrapSafe]
    [HarmonyPatch(typeof(Character), nameof(Character.GetHoverName))]
    private static class Character_GetHoverName_Patch
    {
        private static void Postfix(Character __instance, ref string __result)
        {
            if (!__instance || __result.IsNullOrWhiteSpace()) return;
            if (__instance.IsBoss() || __instance.IsPlayer()) return;
            if (__instance.name.IsNullOrWhiteSpace()) return;
            if (AlmanacClassesPlugin._DisplayExperience.Value is AlmanacClassesPlugin.Toggle.Off) return;
            if (!__instance.m_nview) return;
            if (!__instance.m_nview.IsValid()) return;
            if (__instance.m_nview.GetZDO().GetBool(Classes.Abilities.SpawnSystem.FriendlyKey)) return;
            int exp = ExperienceManager.GetExperienceAmount(__instance);
            __result += $" [<color=orange>{exp}</color>]";
        }
    }
    
    [HarmonyPatch(typeof(Character), nameof(Character.SetMaxHealth))]
    private static class Character_SetMaxHealth_Patch
    {
        private static void Prefix(Character __instance, ref float health)
        {
            if (!__instance) return;
            if (!__instance.IsPlayer()) return;
            if (__instance != Player.m_localPlayer) return;
            health += PlayerManager.GetTotalAddedHealth();
        }
    }

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