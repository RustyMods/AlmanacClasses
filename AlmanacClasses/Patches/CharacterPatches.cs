using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.Classes.Abilities.Rogue;
using AlmanacClasses.Data;
using BepInEx;
using HarmonyLib;
using UnityEngine;
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

    [HarmonyPatch(typeof(Character),nameof(Character.Damage))]
    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    private static class Character_RPC_Damage_Patch
    {
        private static void Prefix(Character __instance, HitData hit)
        {
            if (!__instance || __instance.IsPlayer() || !hit.HaveAttacker()) return;
            if (!hit.GetAttacker().IsPlayer()) return;
            if (hit.GetAttacker() != Player.m_localPlayer) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("RogueBleed", out Talent ability)) return;
            if (!hit.GetAttacker().GetSEMan().HaveStatusEffect(ability.m_statusEffectHash)) return;
            if (__instance.m_nview.IsValid()) __instance.m_nview.ClaimOwnership();
            BleedTrigger.TriggerBleeding(__instance);
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
            
            // ExperienceManager.AddExperienceRPCAll(__instance);
            ExperienceManager.AddExperience(__instance);
            ExperienceManager.DropOrb(__instance);
            Pickpocket.CheckDoubleLoot(__instance);
            CheckBattleFury(__instance);
        }
    }

    private static void CheckBattleFury(Character instance)
    {
        if (!PlayerManager.m_playerTalents.TryGetValue("BattleFury", out Talent talent)) return;
        if (instance.m_lastHit is null or { m_ranged: true }) return;
        float chance = talent.GetChance(talent.GetLevel());
        float random = Random.Range(0, 101);
        if (random > chance) return;
        Player.m_localPlayer.AddStamina(random);
        // if (AlmanacClassesPlugin._BattleFuryFX.Value is AlmanacClassesPlugin.Toggle.Off) return;
        Player.m_localPlayer.GetSEMan().AddStatusEffect("SE_BattleFury".GetStableHashCode());
    }
    
    [HarmonyPatch(typeof(Character), nameof(Character.GetHealth))]
    private static class Player_GetHealth_Patch
    {
        private static void Postfix(Character __instance, ref float __result)
        {
            if (!__instance) return;
            if (__instance != Player.m_localPlayer) return;
            if (__result > 0.0) return;
            if (PlayerManager.m_playerTalents.TryGetValue("Survivor", out Talent talent))
            {
                if (__instance.GetSEMan().HaveStatusEffect("SE_Survivor".GetStableHashCode())) return;
                float chance = talent.GetChance(talent.GetLevel());
                float random = Random.Range(0, 101f);
                if (random < chance)
                {
                    float quarter = __instance.GetMaxHealth() / 4f;
                    __instance.Heal(quarter);
                    __result = quarter;
                    // if (AlmanacClassesPlugin._SurvivorFX.Value is AlmanacClassesPlugin.Toggle.Off) return;
                    __instance.GetSEMan().AddStatusEffect("SE_Survivor".GetStableHashCode());
                }
            }
        }
    }
}