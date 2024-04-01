using System.Collections.Generic;
using System.IO;
using AlmanacClasses.FileSystem;
using BepInEx;
using ServerSync;
using UnityEngine;
using YamlDotNet.Serialization;

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
    public static Dictionary<string, int> CreatureExperienceMap = new()
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

    public static int GetExperienceAmount(string prefabName)
    {
        if (CreatureExperienceMap.Count == 0) return GetExpByBiome() * AlmanacClassesPlugin._ExperienceMultiplier.Value;
        return (CreatureExperienceMap.TryGetValue(prefabName, out int amount) ? amount : GetExpByBiome()) * AlmanacClassesPlugin._ExperienceMultiplier.Value;
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

    public static void AddIncrementExperience(PlayerStatType stat, float amount)
    {
        if (!ExperienceMap.TryGetValue(stat, out int value)) return;
        int total = (int)(value * amount);
        PlayerManager.m_tempPlayerData.m_experience += total * AlmanacClassesPlugin._ExperienceMultiplier.Value;
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
}