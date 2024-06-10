using System.Collections.Generic;
using System.IO;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.FileSystem;
using AlmanacClasses.LoadAssets;
using BepInEx;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using YamlDotNet.Serialization;
using Object = UnityEngine.Object;

namespace AlmanacClasses.Classes;

public static class ExperienceManager
{
    private static readonly CustomSyncedValue<string> ServerExperienceMap = new(AlmanacClassesPlugin.ConfigSync, "ServerTalentsExperienceMap", "");

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Start))]
    private static class ZNet_Start_Patch
    {
        private static void Postfix(ZNet __instance)
        {
            if (!__instance) return;
            InitServerExperienceMap();
        }
    }
    
    private static void InitServerExperienceMap()
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
        { "TentaRoot", 0},
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
        { "Troll", 6 },
        { "gd_king", 10},
        { "Leech", 2},
        { "Blob", 2},
        { "Draugr", 2},
        { "Draugr_Ranged", 3},
        { "Draugr_Elite", 4},
        { "BlobElite", 4},
        { "Wraith", 5},
        { "Abomination", 10},
        { "Bonemass", 15},
        { "Wolf", 2},
        { "Fenring", 4},
        { "Fenring_Cultist", 5},
        { "Ulv", 4},
        { "Hatchling", 3},
        { "StoneGolem", 15},
        { "Fenring_Cultist_Hildir", 20},
        { "Skeleton_Hildir", 20},
        { "GoblinShaman_Hildir", 30},
        { "GoblinBrute_Hildir", 30},
        { "Dragon", 30 },
        { "Goblin", 5 },
        { "BlobTar", 10 },
        { "Lox", 6},
        { "Serpent", 10 },
        { "GoblinKing", 50 },
        { "Seeker", 10 },
        { "SeekerBrute", 15 },
        { "SeekerBrood", 6 },
        { "Hare", 6 },
        { "Tick", 6 },
        { "Gjall", 20 },
        { "Dverger", 20 },
        { "DvergerArbalest", 20 },
        { "DvergerMage", 20 },
        { "DvergerMageFire", 20 },
        { "DvergerMageIce", 20 },
        { "DvergerMageSupport", 20 },
        { "SeekerQueen", 100 },
        { "Surtling", 3 },
        { "FallenValkyrie", 50 },
        { "Charred_Archer", 25 },
        { "Charred_Melee", 25 },
        { "Charred_Mage", 30 },
        { "Charred_Twitcher", 30 },
        { "Morgen", 30 },
        { "Morgen_NonSleeping", 30 },
        { "Goblin_Gem", 30 },
        { "Charred_Melee_Dyrnwyn", 50},
        { "Fader", 200 }
    };

    public static int GetExperienceAmount(Character instance)
    {
        return (int)((CreatureExperienceMap.TryGetValue(instance.name.Replace("(Clone)", string.Empty), out int amount) ? amount : GetExpByBiome()) * instance.m_level * AlmanacClassesPlugin._ExperienceMultiplier.Value);
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
                return 20;
            case Heightmap.Biome.DeepNorth:
                return 5;
            case Heightmap.Biome.Ocean:
                return 5;
            default:
                return 1;
        }
    }
    
    [HarmonyPatch(typeof(PlayerProfile), nameof(PlayerProfile.IncrementStat))]
    private static class PlayerProfile_IncrementStat_Patch
    {
        private static void Postfix(PlayerStatType stat, float amount) => AddExperience(stat, amount);
    }

    private static void AddExperience(PlayerStatType stat, float amount)
    {
        if (!ExperienceMap.TryGetValue(stat, out int value)) return;
        int total = (int)(value * amount);
        PlayerManager.m_tempPlayerData.m_experience += (int)(total * AlmanacClassesPlugin._ExperienceMultiplier.Value);
    }
    
    public static void AddExperience(Character instance)
    {
        if (!instance || instance.name.IsNullOrWhiteSpace()) return;
        int amount = GetExperienceAmount(instance);
        PlayerManager.m_tempPlayerData.m_experience += amount;
    }

    private static readonly Dictionary<PlayerStatType, int> ExperienceMap = new()
    {
        { PlayerStatType.TreeChops , 1},
        { PlayerStatType.LogChops , 1},
        { PlayerStatType.Logs , 1},
        { PlayerStatType.MineHits , 1},
        { PlayerStatType.CreatureTamed , 10},
        { PlayerStatType.ItemsPickedUp , 1},
    };

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    public static void CreateExperienceOrb(float amount, string name, string displayName, Color color, Color32 emission, Color shellColor, Color lightColor, Sprite icon)
    {
        GameObject UpgradeItem = ZNetScene.instance.GetPrefab("StaminaUpgrade_Greydwarf");
        GameObject item = Object.Instantiate(UpgradeItem, AlmanacClassesPlugin._Root.transform, false);
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
        // component.m_itemData.m_shared.m_icons = new[] { icon };
        component.m_itemData.m_shared.m_autoStack = true;
        component.m_itemData.m_shared.m_weight = 0f;
        component.m_autoPickup = true;

        ObjectDB.instance.m_items.Add(item);
        ObjectDB.instance.m_itemByHash[item.name.GetStableHashCode()] = item;
        ZNetScene.instance.m_prefabs.Add(item);
        ZNetScene.instance.m_namedPrefabs[item.name.GetStableHashCode()] = item;
    }
    public static void DropOrb(Character instance)
    {
        int number = Random.Range(0, 100);
        if (number > AlmanacClassesPlugin._ChanceForOrb.Value) return;
        Heightmap.Biome biome = Player.m_localPlayer.GetCurrentBiome();
        switch (biome)
        {
            case Heightmap.Biome.Swamp or Heightmap.Biome.Mountain:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Magic"), instance.transform.position, Quaternion.identity);
                break;
            case Heightmap.Biome.Plains or Heightmap.Biome.Mistlands:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Epic"), instance.transform.position, Quaternion.identity);
                break;
            case Heightmap.Biome.Ocean:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Legendary"), instance.transform.position, Quaternion.identity);
                break;
            default:
                Object.Instantiate(ZNetScene.instance.GetPrefab("ExperienceOrb_Simple"), instance.transform.position, Quaternion.identity);
                break;
        }
    }
    
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
            int exp = GetExperienceAmount(__instance);
            __result += $" [<color=orange>{exp}</color>]";
        }
    }
}