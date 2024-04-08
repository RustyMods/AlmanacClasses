using System;
using System.Collections.Generic;
using System.IO;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.FileSystem;
using AlmanacClasses.LoadAssets;
using BepInEx;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace AlmanacClasses.Classes;

public static class ExperienceManager
{
    private static readonly CustomSyncedValue<string> ServerExperienceMap = new(AlmanacClassesPlugin.ConfigSync, "ServerTalentsExperienceMap", "");

    public static void InitServerExperienceMap()
    {
        if (!ZNet.instance) return;
        FilePaths.CreateFolders();
        if (ZNet.instance.IsServer())
        {
            ReadExperienceFiles(true);
        }
        else
        {
            ServerExperienceMap.ValueChanged += OnServerExperienceMapChange;
        }
    }

    public static void ReadExperienceFiles(bool sync = false)
    {
        ISerializer serializer = new SerializerBuilder().Build();
        if (!File.Exists(FilePaths.ExperienceFilePath))
        {
            File.WriteAllText(FilePaths.ExperienceFilePath, serializer.Serialize(CreatureExperienceMap));
        }
        else
        {
            try
            {
                string data = File.ReadAllText(FilePaths.ExperienceFilePath);
                IDeserializer deserializer = new DeserializerBuilder().Build();
                Dictionary<string, int> custom = deserializer.Deserialize<Dictionary<string, int>>(data);
                foreach (KeyValuePair<string, int> kvp in custom)
                {
                    CreatureExperienceMap[kvp.Key] = kvp.Value;
                }

                if (custom.Count < CreatureExperienceMap.Count)
                {
                    string appended = serializer.Serialize(CreatureExperienceMap);
                    File.WriteAllText(FilePaths.ExperienceFilePath, appended);
                }
            }
            catch
            {
                if (File.Exists(FilePaths.ExperienceFilePath)) File.Delete(FilePaths.ExperienceFilePath);
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to deserialize custom experience map");
            }
        }

        if (!sync) return;
        if (CreatureExperienceMap.Count <= 0) return;
        string newData = serializer.Serialize(CreatureExperienceMap);
        ServerExperienceMap.Value = newData;
    }

    private static void OnServerExperienceMapChange()
    {
        if (ServerExperienceMap.Value.IsNullOrWhiteSpace()) return;
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: server updated experience map");
        IDeserializer deserializer = new DeserializerBuilder().Build();
        Dictionary<string, int> map = deserializer.Deserialize<Dictionary<string, int>>(ServerExperienceMap.Value);
        CreatureExperienceMap = map;
    }

    public static void WriteExperienceMap()
    {
        string filePath = FilePaths.ExperienceFilePath;
        ISerializer serializer = new SerializerBuilder().Build();
        string data = serializer.Serialize(CreatureExperienceMap);
        File.WriteAllText(filePath, data);
        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Experience map written to disk:");
        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(FilePaths.ExperienceFilePath);
    }

    private static Dictionary<string, int> CreatureExperienceMap = new()
    {
        { "Deer", 1 },
        { "Boar", 1 },
        { "Neck", 1 },
        { "Greyling", 1 },
        { "Eikthyr", 5 },
        { "Greydwarf", 2 },
        { "Greydwarf_Shaman", 2 },
        { "Greydwarf_Elite", 3 },
        { "Ghost", 3},
        { "Skeleton", 1 },
        { "Troll", 4 },
        { "gd_king", 10},
        { "Leech", 2},
        { "Blob", 2},
        { "Draugr", 2},
        { "Draugr_Ranged", 2},
        { "Draugr_Elite", 4},
        { "BlobElite", 4},
        { "Wraith", 4},
        { "Abomination", 5},
        { "Bonemass", 15},
        { "Wolf", 2},
        { "Fenring", 4},
        { "Fenring_Cultist", 5},
        { "Ulv", 4},
        { "Hatchling", 3},
        { "StoneGolem", 7},
        { "Fenring_Cultist_Hildir", 20},
        { "Skeleton_Hildir", 20},
        { "GoblinShaman_Hildir", 30},
        { "GoblinBrute_Hildir", 30},
        { "Dragon", 30},
        { "Goblin", 5},
        { "BlobTar", 10},
        { "Lox", 6},
        { "Serpent", 10},
        { "GoblinKing", 50},
        { "Seeker", 10},
        { "SeekerBrute", 15},
        { "SeekerBrood", 6},
        { "Hare", 6},
        { "Tick", 6},
        { "Gjall", 20},
        { "Dverger", 20},
        { "DvergerArbalest", 20},
        { "DvergerMage", 20},
        { "DvergerMageFire", 20},
        { "DvergerMageIce", 20},
        { "DvergerMageSupport", 20},
        { "SeekerQueen", 100},
        { "Surtling", 3}
    };

    public static int GetExperienceAmount(Character instance)
    {
        return (CreatureExperienceMap.TryGetValue(instance.name.Replace("(Clone)", string.Empty), out int amount) ? amount : GetExpByBiome()) * instance.m_level * AlmanacClassesPlugin._ExperienceMultiplier.Value;
    }

    private static int GetExpByBiome()
    {
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
                return 3;
            case Heightmap.Biome.DeepNorth:
                return 5;
            case Heightmap.Biome.Ocean:
                return 5;
            default:
                return 1;
        }
    }

    public static void AddExperience(PlayerStatType stat, float amount)
    {
        if (!ExperienceMap.TryGetValue(stat, out int value)) return;
        int total = (int)(value * amount);
        PlayerManager.m_tempPlayerData.m_experience += total * AlmanacClassesPlugin._ExperienceMultiplier.Value;
    }
    
    public static void AddExperience(Character instance)
    {
        if (!instance || instance.name.IsNullOrWhiteSpace()) return;
        int amount = GetExperienceAmount(instance);
        PlayerManager.m_tempPlayerData.m_experience += amount;
    }

    public static void AddExperienceRPCAll(Character instance)
    {
        try
        {
            if (instance.m_lastHit != null && instance.m_lastHit.GetAttacker() == Player.m_localPlayer)
            {
                List<Player> players = new();
                Player.GetPlayersInRange(Player.m_localPlayer.transform.position, 30f, players);
                int experience = ExperienceManager.GetExperienceAmount(instance);
                foreach (Player player in players)
                {
                    if (!player.IsPlayer()) continue;
                    long id = player.GetPlayerID();
                    ZDOID zdoid = player.GetZDOID();
                    ZRoutedRpc.instance.InvokeRoutedRPC(id, zdoid, nameof(ExperienceManager.RPC_AddExperience), experience);
                }
            }
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to tell everyone to add experience");
        }
    }

    public static void RPC_AddExperience(long sender, int amount)
    {
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Invoked RPC to add experience");
        PlayerManager.AddExperience(amount);
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class ZNetScene_Awake_Patch
    {
        private static void Postfix()
        {
            ZRoutedRpc.instance.Register<int>(nameof(RPC_AddExperience), RPC_AddExperience);
        }
    }
    
    private static readonly Dictionary<PlayerStatType, int> ExperienceMap = new()
    {
        { PlayerStatType.TreeChops , 1},
        { PlayerStatType.LogChops , 1},
        { PlayerStatType.Logs , 1},
        { PlayerStatType.MineHits , 1},
        { PlayerStatType.CreatureTamed , 10},
        { PlayerStatType.ArrowsShot , 1},
        { PlayerStatType.ItemsPickedUp , 1},
    };

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public static void CreateExperienceOrb(float amount, string name, string displayName, Color color, Color32 emission, Color shellColor, Color lightColor, Sprite icon)
    {
        GameObject UpgradeItem = ZNetScene.instance.GetPrefab("StaminaUpgrade_Greydwarf");
        GameObject item = Object.Instantiate(UpgradeItem, AlmanacClassesPlugin._Root.transform, false);
        item.AddComponent<OrbBob>();
        if (item.transform.Find("heart"))
        {
            Object.Destroy(item.transform.Find("heart").gameObject);
        }

        GameObject inner = item.transform.Find("model/inner").gameObject;
        if (inner.TryGetComponent(out MeshRenderer innerRenderer))
        {
            Material[]? materials = innerRenderer.materials;
            List<Material> newMaterials = new List<Material>();
            foreach (var mat in materials)
            {
                Material newMat = new Material(mat)
                {
                    color = color
                };
                newMat.SetColor(EmissionColor, emission);
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
                    color = shellColor
                };
                newMaterials.Add(newMat);
            }

            sphereRenderer.materials = newMaterials.ToArray();
            sphereRenderer.sharedMaterials = newMaterials.ToArray();
        }

        if (item.transform.Find("Point light").TryGetComponent(out Light light))
        {
            light.color = lightColor;
        }
        item.name = name;
        
        StatusEffect exp = ScriptableObject.CreateInstance<SE_ExperienceOrb>();
        exp.name = $"SE_{name}";
        exp.m_ttl = amount + 1f;
        exp.m_name = displayName;
        exp.m_tooltip = "Adds <color=orange>1</color>xp/sec";
        exp.m_startEffects = LoadedAssets.FX_Experience;
        if (!ObjectDB.instance.m_StatusEffects.Contains(exp))
        {
            ObjectDB.instance.m_StatusEffects.Add(exp);
        }

        if (!item.TryGetComponent(out ItemDrop component)) return;
        component.m_itemData.m_shared.m_consumeStatusEffect = ObjectDB.instance.GetStatusEffect($"SE_{name}".GetStableHashCode());
        component.m_itemData.m_shared.m_consumeStatusEffect.m_ttl = amount;
        component.m_itemData.m_shared.m_name = displayName;
        component.m_itemData.m_shared.m_description = $"Adds <color=orange>{amount}</color> class experience";
        component.m_itemData.m_shared.m_questItem = false;
        component.m_itemData.m_shared.m_maxStackSize = 100;
        component.m_itemData.m_shared.m_icons = new[] { icon };
        component.m_itemData.m_shared.m_autoStack = true;

        ObjectDB.instance.m_items.Add(item);
        ObjectDB.instance.m_itemByHash[item.name.GetStableHashCode()] = item;
        ZNetScene.instance.m_prefabs.Add(item);
        ZNetScene.instance.m_namedPrefabs[item.name.GetStableHashCode()] = item;
    }

    public static void DropOrb(Character instance)
    {
        var number = UnityEngine.Random.Range(0, 100);
        if (number > AlmanacClassesPlugin._ChanceForOrb.Value) return;
        var biome = Player.m_localPlayer.GetCurrentBiome();
        switch (biome)
        {
            case Heightmap.Biome.Swamp or Heightmap.Biome.Mountain:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Magic"), instance.transform.position,
                    Quaternion.identity);
                break;
            case Heightmap.Biome.Plains or Heightmap.Biome.Mistlands:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Epic"), instance.transform.position,
                    Quaternion.identity);
                break;
            case Heightmap.Biome.Ocean:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Legendary"), instance.transform.position,
                    Quaternion.identity);
                break;
            default:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Simple"), instance.transform.position,
                    Quaternion.identity);
                break;
                
                
        }
        
    }
}