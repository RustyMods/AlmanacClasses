using System.Collections.Generic;
using System.IO;
using AlmanacClasses.FileSystem;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AlmanacClasses.Classes;

public static class StaticExperience
{
    private static readonly CustomSyncedValue<string> m_serverStaticExperienceMap = new(AlmanacClassesPlugin.ConfigSync, "ServerStaticExperienceMap", "");
    private static Dictionary<string, ExperienceManager.ExperienceData> m_staticExperienceMap = new();

    public static void LoadServerStaticExperienceWatcher()
    {
        m_serverStaticExperienceMap.ValueChanged += () =>
        {
            if (!ZNet.instance || ZNet.instance.IsServer()) return;
            if (m_serverStaticExperienceMap.Value.IsNullOrWhiteSpace()) return;
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                var data =
                    deserializer.Deserialize<Dictionary<string, ExperienceManager.ExperienceData>>(
                        m_serverStaticExperienceMap.Value);
                m_staticExperienceMap = data;
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to parse server static experience map");
            }
        };
    }

    public static void UpdateServerStaticExperience()
    {
        if (!ZNet.instance || !ZNet.instance.IsServer()) return;
        var serializer = new SerializerBuilder().Build();
        m_serverStaticExperienceMap.Value = serializer.Serialize(m_staticExperienceMap);
    }
    public static void LoadStaticMap()
    {
        m_staticExperienceMap = GetDefaultStaticMap();
        var serializer = new SerializerBuilder().Build();
        if (File.Exists(FilePaths.StaticExperienceFilePath))
        {
            var deserializer = new DeserializerBuilder().Build();
            var data = deserializer.Deserialize<Dictionary<string, ExperienceManager.ExperienceData>>(File.ReadAllText(FilePaths.StaticExperienceFilePath));
            if (m_staticExperienceMap.Count > data.Count)
            {
                foreach (var kvp in m_staticExperienceMap)
                {
                    if (data.ContainsKey(kvp.Key)) continue;
                    data[kvp.Key] = kvp.Value;
                }
                File.WriteAllText(FilePaths.StaticExperienceFilePath, serializer.Serialize(data));
            }
            m_staticExperienceMap = data;
        }
        else
        {
            var serial = serializer.Serialize(m_staticExperienceMap);
            File.WriteAllText(FilePaths.StaticExperienceFilePath, serial);
        }
    }
    
    private static Dictionary<string, ExperienceManager.ExperienceData> GetDefaultStaticMap()
    {
        return new Dictionary<string, ExperienceManager.ExperienceData>()
        {
            ["Beech1"] = ExperienceManager.CreateData(1, 1, 100),
            ["Beech_Stub"] = ExperienceManager.CreateData(10, 1, 100),
            ["beech_log"] = ExperienceManager.CreateData(1, 1, 100),
            ["beech_log_half"] = ExperienceManager.CreateData(1, 1, 100),
            ["Birch1"] = ExperienceManager.CreateData(5, 5, 100),
            ["BirchStub"] = ExperienceManager.CreateData(15, 1, 100),
            ["Birch1_aut"] = ExperienceManager.CreateData(5, 5, 100),
            ["Birch2"] = ExperienceManager.CreateData(5, 5, 100),
            ["Birch2_aut"] = ExperienceManager.CreateData(5, 5, 100),
            ["Birch_log"] = ExperienceManager.CreateData(5, 5, 100),
            ["Birch_log_half"] = ExperienceManager.CreateData(5, 5, 100),
            ["FirTree"] = ExperienceManager.CreateData(1, 1, 100),
            ["FirTree_log"] = ExperienceManager.CreateData(1, 1, 100),
            ["FirTree_log_half"] = ExperienceManager.CreateData(1, 1, 100),
            ["Oak1"] = ExperienceManager.CreateData(10, 5, 100),
            ["Oak_log"] = ExperienceManager.CreateData(5, 5, 100),
            ["Oak_log_half"] = ExperienceManager.CreateData(5, 5, 100),
            ["SwampTree1"] = ExperienceManager.CreateData(5, 5, 100),
            ["SwampTree1_log"] = ExperienceManager.CreateData(5, 5, 100),
            ["YggaShoot1"] = ExperienceManager.CreateData(10, 20, 100),
            ["YggaShoot2"] = ExperienceManager.CreateData(10, 20, 100),
            ["YggaShoot3"] = ExperienceManager.CreateData(10, 20, 100),
            ["yggashoot_log"] = ExperienceManager.CreateData(10, 20, 100),
            ["yggashoot_log_half"] = ExperienceManager.CreateData(10, 20, 100),
            ["OakStub"] = ExperienceManager.CreateData(20, 1, 100),
            ["PineTree"] = ExperienceManager.CreateData(10, 1, 100),
            ["Pinetree_01"] = ExperienceManager.CreateData(10, 1, 100),
            ["PineTree_log"] = ExperienceManager.CreateData(10, 1, 100),
            ["PineTree_log_half"] = ExperienceManager.CreateData(10, 1, 100),
            ["Pinetree_01_Stub"] = ExperienceManager.CreateData(10, 1, 100),
            ["FirTree_Stub"] = ExperienceManager.CreateData(10, 1, 100),
            ["SwampTree1_Stub"] = ExperienceManager.CreateData(15, 1, 100),
            ["MineRock_Tin"] = ExperienceManager.CreateData(2, 1, 20),
            ["$piece_deposit_copper"] = ExperienceManager.CreateData(2, 1, 20),
            ["$piece_mudpile"] = ExperienceManager.CreateData(3, 10, 40),
            ["$piece_deposit_silvervein"] = ExperienceManager.CreateData(3, 10, 50),
            ["MineRock_Obsidian"] = ExperienceManager.CreateData(5, 5, 50),
            ["$piece_giant_brain"] = ExperienceManager.CreateData(10, 20, 70),
            ["sapling_seedturnip"] = ExperienceManager.CreateData(1, 5, 40),
            ["sapling_turnip"] = ExperienceManager.CreateData(1, 5, 40),
            ["sapling_seedcarrot"] = ExperienceManager.CreateData(1, 5, 40),
            ["sapling_carrot"] = ExperienceManager.CreateData(1, 5, 40),
            ["sapling_onion"] = ExperienceManager.CreateData(2, 5, 50),
            ["sapling_seedonion"] = ExperienceManager.CreateData(2, 5, 50),
            ["sapling_barley"] = ExperienceManager.CreateData(2, 5, 100),
            ["sapling_flax"] = ExperienceManager.CreateData(2, 5, 100),
            ["sapling_magecap"] = ExperienceManager.CreateData(3, 5, 100),
            ["sapling_jotunpuffs"] = ExperienceManager.CreateData(3, 5, 100),
            ["Fish1"] = ExperienceManager.CreateData(10, 1, 100),
            ["Fish2"] = ExperienceManager.CreateData(20, 1, 100),
            ["Fish3"] = ExperienceManager.CreateData(30, 1, 100),
            ["Fish4_cave"] = ExperienceManager.CreateData(40, 1, 100),
            ["Fish5"] = ExperienceManager.CreateData(50, 1, 100),
            ["Fish6"] = ExperienceManager.CreateData(60, 1, 100),
            ["Fish7"] = ExperienceManager.CreateData(70, 1, 100),
            ["Fish8"] = ExperienceManager.CreateData(80, 1, 100),
            ["Fish9"] = ExperienceManager.CreateData(90, 1, 100),
            ["Fish10"] = ExperienceManager.CreateData(100, 1, 100),
            ["Fish11"] = ExperienceManager.CreateData(110, 1, 100),
            ["Fish12"] = ExperienceManager.CreateData(120, 1, 100),
        };
    }

    private static void CheckExperienceMap(string prefabName, Vector3 position, Player player)
    {
        if (!m_staticExperienceMap.TryGetValue(prefabName.Replace("(Clone)", string.Empty), out ExperienceManager.ExperienceData data)) return;
        int playerLevel = PlayerManager.GetPlayerLevel(PlayerManager.GetExperience());
        if (AlmanacClassesPlugin._UseExperienceLevelCap.Value is AlmanacClassesPlugin.Toggle.On 
            && (playerLevel > data.MaximumLevel || playerLevel < data.MinimumLevel)) return;
        float exp = data.Experience * AlmanacClassesPlugin._ExperienceMultiplier.Value;
        AddExperience(player, (int)exp, position);
    }

    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
    private static class Pickable_Interact_Patch
    {
        private static void Postfix(Pickable __instance, Humanoid character, bool __result)
        {
            if (!__result) return;
            if (character is not Player player) return;
            if (__instance.m_tarPreventsPicking)
            {
                if (__instance.m_floating && __instance.m_floating.IsInTar()) return;
            }
            CheckExperienceMap(__instance.name, __instance.transform.position, player);
        }
    }

    [HarmonyPatch(typeof(Destructible), nameof(Destructible.RPC_Damage))]
    private static class Destructible_RPC_Damage_Patch
    {
        private static void Postfix(Destructible __instance, HitData hit)
        {
            // if (!__instance.m_nview.IsOwner()) return;
            Character attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return;
            CheckExperienceMap(__instance.name, __instance.transform.position, player);
        }
    }

    [HarmonyPatch(typeof(MineRock), nameof(MineRock.RPC_Hit))]
    private static class MineRock_RPC_Hit_Patch
    {
        private static void Postfix(MineRock __instance, HitData hit)
        {
            Character attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return; 
            CheckExperienceMap(__instance.name, __instance.transform.position, player);
        }
    }

    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.RPC_Damage))]
    private static class MineRock5_RPC_Damage_Patch
    {
        private static void Postfix(MineRock5 __instance, HitData hit)
        {
            Character attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return;
            CheckExperienceMap(__instance.GetHoverName(), __instance.transform.position, player);
        }
    }

    [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))]
    private static class TreeBase_RPC_Damage_Patch
    {
        private static void Postfix(TreeBase __instance, HitData hit)
        {
            Character attacker = hit.GetAttacker();
            if (attacker == null) return;
            if (attacker is not Player player) return;
            if (!hit.CheckToolTier(__instance.m_minToolTier)) return;
            if (hit.GetTotalDamage() < 1) return;
            CheckExperienceMap(__instance.name, __instance.transform.position, player);
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
            CheckExperienceMap(__instance.name, __instance.transform.position, player);
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
            CheckExperienceMap(piece.name, __instance.transform.position, __instance);
        }
    }

    [HarmonyPatch(typeof(Fish), nameof(Fish.OnHooked))]
    private static class Fish_OnHooked_Patch
    {
        private static void Postfix(Fish __instance)
        {
            if (!__instance) return;
            if (__instance.m_fishingFloat == null) return;
            Character owner = __instance.m_fishingFloat.GetOwner();
            if (owner == null) return;
            if (owner is not Player player) return;
            CheckExperienceMap(__instance.name, __instance.m_fishingFloat.transform.position, player);
        }
    }

    private static void AddExperience(Player player, int amount, Vector3 position)
    {
        if (player.m_nview.IsOwner())
        {
            PlayerManager.m_tempPlayerData.m_experience += amount;
        }
        else
        {
            player.m_nview.InvokeRPC(nameof(ExperienceManager.RPC_AddExperience), amount, position);
        }
        DisplayText.ShowText(Color.cyan, position, $"+{amount} $text_xp");
        ExperienceBar.UpdateExperienceBar();
    }
}