using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Classes.Abilities.Bard;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.FileSystem;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Classes;

public static class ExperienceManager
{
    private static readonly CustomSyncedValue<string> TierServerExperienceMap = new(AlmanacClassesPlugin.ConfigSync, "TierServerExperienceMap", "");
    private static Dictionary<string, ExperienceData> m_creatureExperienceMap = new();
    
    [Serializable]
    public class ExperienceData
    {
        public int Experience = 1;
        public int MinimumLevel = 1;
        public int MaximumLevel = 1;
    }
    
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Start))]
    private static class ZNet_Start_Patch
    {
        private static void Postfix(ZNet __instance)
        {
            if (!__instance) return;
            FilePaths.CreateFolders();
            if (!__instance.IsServer()) return;
            UpdateServerExperienceMap();
            StaticExperience.UpdateServerStaticExperience();
        }
    }

    private static void UpdateServerExperienceMap()
    {
        ISerializer serializer = new SerializerBuilder().Build();
        TierServerExperienceMap.Value = serializer.Serialize(m_creatureExperienceMap);
    }
    public static void LoadServerExperienceMapWatcher()
    {
        TierServerExperienceMap.ValueChanged += () =>
        {
            if (!ZNet.instance || ZNet.instance.IsServer()) return;
            if (TierServerExperienceMap.Value.IsNullOrWhiteSpace()) return;
            try
            {
                IDeserializer deserializer = new DeserializerBuilder().Build();
                m_creatureExperienceMap = deserializer.Deserialize<Dictionary<string, ExperienceData>>(TierServerExperienceMap.Value);
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Server experience map changed, reloading");
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to parse server experience map");
            }
        };
    }
    
    public static void LoadCreatureMap()
    {
        FilePaths.CreateFolders();
        ISerializer serializer = new SerializerBuilder().Build();
        if (File.Exists(FilePaths.TierExperienceFilePath))
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            string file = File.ReadAllText(FilePaths.TierExperienceFilePath);
            try
            {
                m_creatureExperienceMap = deserializer.Deserialize<Dictionary<string, ExperienceData>>(file);
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to parse experience map");
                m_creatureExperienceMap = GetDefaultCreatureMap();
            }
        }
        else
        {
            m_creatureExperienceMap = GetDefaultCreatureMap();
            string data = serializer.Serialize(m_creatureExperienceMap);
            File.WriteAllText(FilePaths.TierExperienceFilePath, data);
        }

        if (ZNet.instance && ZNet.instance.IsServer())
        {
            TierServerExperienceMap.Value = serializer.Serialize(m_creatureExperienceMap);
        }
    }

    private static Dictionary<string, ExperienceData> GetDefaultCreatureMap()
    {
        Dictionary<string, ExperienceData> output = new()
        {
            ["Greyling"] = CreateData(1, 1, 15),
            ["Boar"] = CreateData(1, 1, 15),
            ["Deer"] = CreateData(1, 1, 15),
            ["Neck"] = CreateData(1, 1, 15),
            ["Eikthyr"] = CreateData(10, 1, 15),
            ["Greydwarf"] = CreateData(2, 5, 25),
            ["Greydwarf_Elite"] = CreateData(5, 5, 25),
            ["Greydwarf_Shaman"] = CreateData(5, 5, 25),
            ["Ghost"] = CreateData(3, 5, 25),
            ["Skeleton"] = CreateData(2, 5, 25),
            ["Troll"] = CreateData(10, 5, 25),
            ["Skeleton_Hildir"] = CreateData(20, 5, 35),
            ["gd_king"] = CreateData(20, 5, 25),
            ["TentaRoot"] = CreateData(0, 100, 100),
            ["Surtling"] = CreateData(3, 10, 30),
            ["Leech"] = CreateData(5, 10, 30),
            ["Skeleton_Poison"] = CreateData(5, 10, 30),
            ["Draugr"] = CreateData(5, 10, 30),
            ["Draugr_ranged"] = CreateData(5, 10, 30),
            ["Draugr_Elite"] = CreateData(10, 10, 30),
            ["Blob"] = CreateData(5, 10, 30),
            ["BlobElite"] = CreateData(10, 10, 30),
            ["Wraith"] = CreateData(10, 10, 30),
            ["Abomination"] = CreateData(15, 10, 30),
            ["Bonemass"] = CreateData(30, 10, 30),
            ["Serpent"] = CreateData(30, 10, 50),
            ["Bat"] = CreateData(8, 15, 40),
            ["Wolf"] = CreateData(10, 15, 40),
            ["Hatchling"] = CreateData(10, 15, 40),
            ["Ulv"] = CreateData(15, 15, 40),
            ["Fenring"] = CreateData(18, 15, 40),
            ["Fenring_Cultist"] = CreateData(25, 15, 40),
            ["StoneGolem"] = CreateData(30, 15, 40),
            ["Dragon"] = CreateData(30, 15, 40),
            ["Fenring_Cultist_Hildr"] = CreateData(30, 15, 60),
            ["Deathsquito"] = CreateData(20, 20, 60),
            ["Goblin"] = CreateData(20, 20, 60),
            ["GoblinShaman"] = CreateData(25, 20, 60),
            ["GoblinBrute"] = CreateData(25, 20, 60),
            ["Lox"] = CreateData(25, 20, 60),
            ["BlobTar"] = CreateData(25, 20, 60),
            ["GoblinKing"] = CreateData(40, 20, 60),
            ["GoblinShaman_Hildir"] = CreateData(40, 20, 80),
            ["GoblinBrute_Hildir"] = CreateData(40, 20, 80),
            ["GoblinBruteBros"] = CreateData(40, 20, 80),
            ["Hare"] = CreateData(25, 25, 80),
            ["Tick"] = CreateData(25, 25, 80),
            ["SeekerBrood"] = CreateData(25, 25, 80),
            ["Seeker"] = CreateData(30, 25, 80),
            ["SeekerBrute"] = CreateData(35, 25, 80),
            ["Dverger"] = CreateData(35, 25, 80),
            ["DvergerArbalest"] = CreateData(35, 25, 80),
            ["DvergerMage"] = CreateData(35, 25, 80),
            ["DvergerMageFire"] = CreateData(35, 25, 80),
            ["DvergerMageIce"] = CreateData(35, 25, 80),
            ["DvergerMageSupport"] = CreateData(35, 25, 80),
            ["Gjall"] = CreateData(40, 25, 80),
            ["SeekerQueen"] = CreateData(50, 25, 80),
            ["Charred_Twitcher"] = CreateData(30, 30, 100),
            ["Charred_Archer"] = CreateData(35, 30, 100),
            ["Charred_Melee"] = CreateData(35, 30, 100),
            ["Charred_Mage"] = CreateData(35, 30, 100),
            ["Morgen"] = CreateData(40, 30, 100),
            ["Morgen_NonSleeping"] = CreateData(40, 30, 100),
            ["FallenValkyrie"] = CreateData(40, 30, 100),
            ["Goblin_Gem"] = CreateData(40, 30, 100),
            ["Charred_Melee_Dyrnwyn"] = CreateData(45, 30, 100),
            ["Fader"] = CreateData(65, 30, 100)
        };
        return output;
    }

    public static ExperienceData CreateData(int experience, int min, int max) => new(){ Experience = experience, MinimumLevel = min, MaximumLevel = max };
    public static void WriteExperienceMap()
    {
        string filePath = FilePaths.TierExperienceFilePath;
        ISerializer serializer = new SerializerBuilder().Build();
        string data = serializer.Serialize(m_creatureExperienceMap);
        File.WriteAllText(filePath, data);
        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Experience map written to disk:");
        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(FilePaths.TierExperienceFilePath);
    }

    private static int GetRaiderExperience()
    {
        if (!Player.m_localPlayer) return 0;
        return Player.m_localPlayer.GetCurrentBiome() switch
        {
            Heightmap.Biome.BlackForest => 10,
            Heightmap.Biome.Swamp => 20,
            Heightmap.Biome.Mountain => 40,
            Heightmap.Biome.Plains => 60,
            Heightmap.Biome.Mistlands => 80,
            Heightmap.Biome.AshLands or Heightmap.Biome.DeepNorth => 100,
            _ => 5
        };
    }
    private static int GetExperienceAmount(Character instance)
    {
        if (m_creatureExperienceMap.TryGetValue(instance.name.Replace("(Clone)", string.Empty), out ExperienceData data))
        {
            int playerLevel = PlayerManager.GetPlayerLevel(PlayerManager.GetExperience());
            if (AlmanacClassesPlugin._UseExperienceLevelCap.Value is AlmanacClassesPlugin.Toggle.On 
                && (playerLevel < data.MinimumLevel || playerLevel > data.MaximumLevel)) return 0;
            return (int)(data.Experience * instance.m_level * AlmanacClassesPlugin._ExperienceMultiplier.Value);
        }

        return (int)(GetExpByBiome() * instance.m_level * AlmanacClassesPlugin._ExperienceMultiplier.Value);
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnDeath))]
    private static class Player_OnDeath_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!__instance || __instance != Player.m_localPlayer) return;
            LoseExperience();
        }
    }

    private static void LoseExperience()
    {
        if (AlmanacClassesPlugin._loseExperience.Value is AlmanacClassesPlugin.Toggle.Off) return;
        var current = PlayerManager.GetExperience();
        var minimum = PlayerManager.GetRequiredExperience(PlayerManager.GetPlayerLevel(current));
        var maximum = current - minimum;

        var hundred = PlayerManager.GetRequiredExperience(PlayerManager.GetPlayerLevel(current) + 1);
        var amount = Mathf.FloorToInt((hundred - minimum) / AlmanacClassesPlugin._experienceLossFactor.Value);

        var lost = current - amount < minimum ? maximum : amount;

        if (lost == 0) return;
        PlayerManager.m_tempPlayerData.m_experience -= lost;
        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, $"$msg_lost_experience: {lost}");
        ExperienceBar.UpdateExperienceBar();
    }
    private static int GetExpByBiome()
    {
        if (!Player.m_localPlayer) return 0;
        switch (Player.m_localPlayer.GetCurrentBiome())
        {
            case Heightmap.Biome.Meadows:
                return 1;
            case Heightmap.Biome.BlackForest:
                return 2;
            case Heightmap.Biome.Swamp:
                return 3;
            case Heightmap.Biome.Mountain:
                return 4;
            case Heightmap.Biome.Plains:
                return 5;
            case Heightmap.Biome.Mistlands:
                return 10;
            case Heightmap.Biome.AshLands:
                return 20;
            case Heightmap.Biome.DeepNorth:
                return 5;
            case Heightmap.Biome.Ocean:
                return 5;
            default:
                return 1;
        }
    }

    public static void AddExperience(Character instance)
    {
        if (!instance || instance.name.IsNullOrWhiteSpace()) return;
        if (instance.m_lastHit != null && instance.m_lastHit.GetAttacker() != null && instance.m_lastHit.GetAttacker() is not Player) return;
        int amount = GetExperienceAmount(instance);
        if (amount == 0) return;
        foreach (Player player in Player.GetAllPlayers())
        {
            if (!player.m_nview.IsValid()) continue;
            player.m_nview.InvokeRPC(nameof(RPC_AddExperience), amount, instance.transform.position);
        }
    }

    public static void Command_GiveExperience(Player player, int amount)
    {
        if (!player.m_nview.IsValid()) return;
        player.m_nview.InvokeRPC(nameof(RPC_AddExperience), amount, player.transform.position);
    }

    public static void RPC_AddExperience(long sender, int experience, Vector3 pos)
    {
        PlayerManager.m_tempPlayerData.m_experience += experience;
        ExperienceBar.UpdateExperienceBar();
        DisplayText.ShowText(Color.cyan, pos, $"+{experience} $text_xp");
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    private static class Player_Awake_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!__instance) return;
            if (!__instance.m_nview || __instance.m_nview.GetZDO() == null) return;
            __instance.m_nview.Register<int, Vector3>(nameof(RPC_AddExperience), RPC_AddExperience);
            __instance.m_nview.Register<string>(nameof(RPC_SetKey), RPC_SetKey);
        }
    }
    
    private static void RPC_SetKey(long sender, string key) => Player.m_localPlayer.AddUniqueKey(key);
    
    public static void SetDefeatKey(Character __instance)
    {
        if (!__instance.IsBoss() || __instance.m_defeatSetGlobalKey.IsNullOrWhiteSpace()) return;
        Player.m_localPlayer.AddUniqueKey(__instance.m_defeatSetGlobalKey);
        foreach (Player? player in Player.GetAllPlayers())
        {
            if (player == null || player.IsDead()) continue;
            if (!player.m_nview.IsValid()) continue;
            if (player.m_nview.GetZDO() == null) continue;
            player.m_nview.InvokeRPC(nameof(RPC_SetKey), __instance.m_defeatSetGlobalKey);
        }
    }

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private static readonly List<ExperienceOrb> m_registeredOrbs = new();
    private static readonly List<ExperienceOrb> m_orbs = new();
    public class ExperienceOrb
    {
        public readonly string m_prefabName;
        public readonly string m_displayName;
        public int m_amount;
        public Color m_color;
        public Color32 m_emissionColor;
        public Color m_shellColor;
        public Color m_lightColor;
        public Heightmap.Biome m_biomes;

        public readonly ConfigEntry<Heightmap.Biome> m_biomeConfig;
        public readonly ConfigEntry<int> m_amountConfig;

        public GameObject m_prefab = null!;
        public ItemDrop m_item = null!;
        public SE_ExperienceOrb m_status = null!;

        public bool m_isValid = true;
        public ExperienceOrb(string prefabName, string displayName, int amount, Heightmap.Biome biome)
        {
            m_prefabName = prefabName;
            m_displayName = displayName;
            m_biomeConfig = AlmanacClassesPlugin._Plugin.config("5 - Experience Orbs", $"{m_displayName} Biome", biome, "Set biome, split by comma, where orb can drop");
            m_amountConfig = AlmanacClassesPlugin._Plugin.config("5 - Experience Orbs", $"{m_displayName} Amount", amount, $"Set amount of experience {m_displayName} gives");

            m_biomes = m_biomeConfig.Value;
            m_biomeConfig.SettingChanged += (sender, args) => m_biomes = m_biomeConfig.Value;

            m_orbs.Add(this);
        }

        public void Create()
        {
            if (!ZNetScene.instance)
            {
                m_isValid = false;
                return;
            }
            GameObject UpgradeItem = ZNetScene.instance.GetPrefab("StaminaUpgrade_Greydwarf");
            GameObject item = Object.Instantiate(UpgradeItem, AlmanacClassesPlugin._Root.transform, false);
            if (item.transform.Find("heart") is {} heart) Object.Destroy(heart.gameObject);
            GameObject inner = item.transform.Find("model/inner").gameObject;
            if (inner.TryGetComponent(out MeshRenderer innerRenderer))
            {
                Material[]? materials = innerRenderer.materials;
                List<Material> newMaterials = new List<Material>();
                foreach (var mat in materials)
                {
                    Material newMat = new Material(mat)
                    {
                        color = m_color
                    };
                    newMat.SetColor(EmissionColor, m_emissionColor);
                    newMaterials.Add(newMat);
                }

                innerRenderer.materials = newMaterials.ToArray();
                innerRenderer.sharedMaterials = newMaterials.ToArray();
            }

            GameObject sphere = item.transform.Find("model/Sphere").gameObject;
            if (sphere.TryGetComponent(out MeshRenderer sphereRenderer))
            {
                Material[]? materials = sphereRenderer.materials;
                List<Material> newMaterials = new List<Material>();
                foreach (var mat in materials)
                {
                    Material newMat = new Material(mat)
                    {
                        color = m_shellColor
                    };
                    newMaterials.Add(newMat);
                }

                sphereRenderer.materials = newMaterials.ToArray();
                sphereRenderer.sharedMaterials = newMaterials.ToArray();
            }

            if (item.transform.Find("Point light").TryGetComponent(out Light light))
            {
                light.color = m_lightColor;
            }
            item.name = m_prefabName;
            if (!item.TryGetComponent(out ItemDrop component))
            {
                m_isValid = false;
                return;
            }
            SE_ExperienceOrb exp = ScriptableObject.CreateInstance<SE_ExperienceOrb>();
            exp.name = $"SE_{m_prefabName}";
            exp.m_ttl = 10f;
            exp.m_name = m_displayName;
            exp.m_startEffects = VFX.FX_Experience.m_effectList;
            exp.m_icon = component.m_itemData.GetIcon();
            exp.m_amount = m_amountConfig;
            // exp.m_amount = AlmanacClassesPlugin._Plugin.config("5 - Experience Orbs", m_displayName, m_amount, "Set the amount of experience received");
            if (!ObjectDB.instance.m_StatusEffects.Contains(exp))
            {
                ObjectDB.instance.m_StatusEffects.Add(exp);
            }

            component.m_itemData.m_shared.m_consumeStatusEffect = exp;
            component.m_itemData.m_shared.m_name = m_displayName;
            component.m_itemData.m_shared.m_description = "";
            component.m_itemData.m_shared.m_questItem = false;
            component.m_itemData.m_shared.m_maxStackSize = 100;
            component.m_itemData.m_shared.m_autoStack = true;
            component.m_itemData.m_shared.m_weight = 0f;
            component.m_autoPickup = true;

            m_prefab = item;
            m_item = component;
            m_status = exp;

            // ConfigEntry<Heightmap.Biome> biomeConfig = AlmanacClassesPlugin._Plugin.config("5 - Experience Orbs", m_displayName, Heightmap.Biome.Meadows, "Set biomes where orb can drop");
            // m_biomes = biomeConfig.Value;
            // biomeConfig.SettingChanged += (_, _) => m_biomes = biomeConfig.Value;
            
            Register();
            Snapshot.SnapshotItem(component, 0.6f);
            m_registeredOrbs.Add(this);
        }

        private void Register()
        {
            if (!ObjectDB.instance.m_items.Contains(m_prefab)) ObjectDB.m_instance.m_items.Add(m_prefab);
            ObjectDB.instance.m_itemByHash[m_prefab.name.GetStableHashCode()] = m_prefab;
            if (!ZNetScene.instance.m_prefabs.Contains(m_prefab)) ZNetScene.instance.m_prefabs.Add(m_prefab);
            ZNetScene.instance.m_namedPrefabs[m_prefab.name.GetStableHashCode()] = m_prefab;
            if (!ObjectDB.instance.m_StatusEffects.Contains(m_status)) ObjectDB.instance.m_StatusEffects.Add(m_status);
        }
    }

    public static void LoadExperienceOrbs()
    {
        foreach(var orb in m_orbs) orb.Create();
    }
    public static void DropOrb(Character instance)
    {
        int number = Random.Range(0, 100);
        if (number > AlmanacClassesPlugin._ChanceForOrb.Value) return;
        Heightmap.Biome biome = Player.m_localPlayer.GetCurrentBiome();

        List<ExperienceOrb> orbs = m_registeredOrbs.FindAll(orb => orb.m_biomes.HasFlagFast(biome));
        var orb = orbs[Random.Range(0, orbs.Count)];
        Object.Instantiate(orb.m_prefab, instance.transform.position, Quaternion.identity);
    }
    
    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.UpdateHuds))]
    private static class AddExperience_HUD
    {
        private static void Postfix(EnemyHud __instance)
        {
            if (AlmanacClassesPlugin._DisplayExperience.Value is AlmanacClassesPlugin.Toggle.Off) return;
            foreach (KeyValuePair<Character, EnemyHud.HudData> kvp in __instance.m_huds)
            {
                if (kvp.Key == null || kvp.Value.m_gui == null) continue;
                if (kvp.Key.IsBoss() || kvp.Key.IsPlayer()) continue;
                if (IsFriendlyCreature(kvp.Key)) continue;
                int amount = GetExperienceAmount(kvp.Key);
                if (amount == 0) continue;
                kvp.Value.m_name.text += $" [<color=orange>{amount}</color>]";
            }
        }
    }
    public static bool IsFriendlyCreature(Character character) => character.GetComponent<Friendly>();
}