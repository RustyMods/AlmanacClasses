using AlmanacClasses.Managers;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes;

public static class StaticExperience
{
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.RPC_Damage))]
    private static class Destructible_RPC_Damage_Patch
    {
        private static void Postfix(Destructible __instance, HitData hit)
        {
            var attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return; 
            if (__instance.m_destructibleType is DestructibleType.Tree) return;
            float exp = Mathf.Max((__instance.m_minToolTier + 1) * AlmanacClassesPlugin._ExperienceMultiplier.Value, 1);
            if (__instance.m_nview.IsOwner())
            {
                AddExperience(player, (int)exp, __instance.transform.position);
            }
        }
    }

    [HarmonyPatch(typeof(MineRock), nameof(MineRock.RPC_Hit))]
    private static class MineRock_RPC_Hit_Patch
    {
        private static void Postfix(MineRock __instance, HitData hit)
        {
            var attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return; 
            float exp = Mathf.Max((__instance.m_minToolTier + 1) * AlmanacClassesPlugin._ExperienceMultiplier.Value, 1);
            if (__instance.m_nview.IsOwner())
            {
                AddExperience(player, (int)exp, __instance.transform.position);
            }
        }
    }

    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.RPC_Damage))]
    private static class MineRock5_RPC_Damage_Patch
    {
        private static void Postfix(MineRock5 __instance, HitData hit)
        {
            var attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return; 
            float exp = Mathf.Max((__instance.m_minToolTier + 1) * AlmanacClassesPlugin._ExperienceMultiplier.Value, 1);
            if (__instance.m_nview.IsOwner())
            {
                AddExperience(player, (int)exp, __instance.transform.position);
            }
        }
    }

    [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))]
    private static class TreeBase_RPC_Damage_Patch
    {
        private static void Postfix(TreeBase __instance, HitData hit)
        {
            var attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return;
            float exp = Mathf.Max((__instance.m_minToolTier + 1) * AlmanacClassesPlugin._ExperienceMultiplier.Value, 1);
            if (__instance.m_nview.IsOwner())
            {
                AddExperience(player, (int)exp, __instance.transform.position);
            }
        }
    }

    [HarmonyPatch(typeof(TreeLog), nameof(TreeLog.RPC_Damage))]
    private static class TreeLog_RPC_Damage_Patch
    {
        private static void Postfix(TreeLog __instance, HitData hit)
        {
            var attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return; 
            float exp = Mathf.Max((__instance.m_minToolTier + 1) * AlmanacClassesPlugin._ExperienceMultiplier.Value, 1);
            if (__instance.m_nview.IsOwner())
            {
                AddExperience(player, (int)exp, __instance.transform.position);
            }
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Tame))]
    private static class Tameable_Tame_Patch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance.m_nview.IsOwner()) return;
            Player closestPlayer = Player.GetClosestPlayer(__instance.transform.position, 30f);
            if (!closestPlayer) return;
            AddExperience(closestPlayer, 10 * __instance.m_character.m_level, __instance.transform.position);
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
    private static class Player_PlacePiece_Patch
    {
        private static void Postfix(Player __instance, Piece piece)
        {
            if (!piece.m_cultivatedGroundOnly) return;
            AddExperience(__instance, 1, __instance.transform.position);
        }
    }

    private static void AddExperience(Player player, int amount, Vector3 position)
    {
        if (player.m_nview.IsOwner())
        {
            PlayerManager.m_tempPlayerData.m_experience += amount;
            DisplayText.ShowText(new Color(0.1f, 0.7f, 0.8f, 1f), player.transform.position, $"+{amount} $text_xp");
        }
        else
        {
            player.m_nview.InvokeRPC(nameof(ExperienceManager.RPC_AddExperience), amount, position);
        }
    }
}