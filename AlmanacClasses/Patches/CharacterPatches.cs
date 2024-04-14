using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Data;
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
            if (__instance.m_nview.IsValid()) __instance.m_nview.ClaimOwnership();
            Spells.TriggerBleeding(__instance);
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
            CheckDoubleLoot(__instance);
        }
    }

    private static void CheckDoubleLoot(Character instance)
    {
        if (instance.m_lastHit == null || !instance.m_localPlayerHasHit) return;
        if (!PlayerManager.m_playerTalents.TryGetValue("CoreMerchant", out Talent ability)) return;
        if (instance.m_baseAI == null || instance.m_baseAI.CanSeeTarget(instance.m_lastHit.GetAttacker())) return;
        if (!instance.TryGetComponent(out CharacterDrop characterDrop)) return;
        int percentage = (int)((ability.m_chance?.Value ?? 20f) * ability.m_level);
        int random = Random.Range(0, 101);
        if (random <= percentage)
        {
            CharacterDrop.DropItems(characterDrop.GenerateDropList(), instance.GetCenterPoint() + characterDrop.transform.TransformVector(characterDrop.m_spawnOffset), 0.5f);
        }
    }
}