using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AlmanacClasses.FileSystem;
using UnityEngine;
using static AlmanacClasses.AlmanacClassesPlugin;

namespace AlmanacClasses.Managers;

public static class SpriteManager
{
    
    //public static readonly Sprite Impervious_Icon = AlmanacClassesPlugin._AssetBundle.LoadAsset<Sprite>("Barbarian_5");
    public static readonly Sprite HourGlass_Icon = _AssetBundle.LoadAsset<Sprite>("hourglass"); // TODO: Not used?
    // Alternative Skills
    public static readonly Sprite WindIcon = _AssetBundle.LoadAsset<Sprite>("windicon");
    public static readonly Sprite Sail_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_22");
    public static readonly Sprite ScrollMap = _AssetBundle.LoadAsset<Sprite>("scroll_map2");
    public static readonly Sprite ShieldIcon = _AssetBundle.LoadAsset<Sprite>("shield_basic_metal");
    public static readonly Sprite CrownIcon = _AssetBundle.LoadAsset<Sprite>("crown_gold");
    // Class Icons
    public static readonly Sprite BardIcon = _AssetBundle.LoadAsset<Sprite>("bard_icon");
    public static readonly Sprite WarriorIcon = _AssetBundle.LoadAsset<Sprite>("warrior_icon");
    public static readonly Sprite RangerIcon = _AssetBundle.LoadAsset<Sprite>("ranger_icon");
    public static readonly Sprite RogueIcon = _AssetBundle.LoadAsset<Sprite>("rogue_icon");
    public static readonly Sprite ShamanIcon = _AssetBundle.LoadAsset<Sprite>("shaman_icon");
    // TODO: Sage icon?
    // Core
    public static readonly Sprite? MedalIcon = RegisterSprite("AirBender.png", "Icons.Classes.Core");
    public static readonly Sprite? Looter_Icon = RegisterSprite("Looter.png", "Icons.Classes.Core");
    public static readonly Sprite? MasterChef_Icon = RegisterSprite("MasterChef.png", "Icons.Classes.Core");
    // TODO: Berzerker, Comfort, Forager, PackMule, Resourceful, Sailor, TreasureHunter and Trader icons? Added sprites for them, in case
    // Bard
    public static readonly Sprite? SongOfSpeed_Icon = RegisterSprite("SongOfSpeed.png", "Icons.Classes.Bard");
    public static readonly Sprite? SongOfVitality_Icon = RegisterSprite("SongOfVitality.png", "Icons.Classes.Bard");
    public static readonly Sprite? SongOfDamage_Icon = RegisterSprite("SongOfDamage.png", "Icons.Classes.Bard");
    public static readonly Sprite? SongOfHealing_Icon = RegisterSprite("SongOfHealing.png", "Icons.Classes.Bard");
    public static readonly Sprite? SongOfSpirit_Icon = RegisterSprite("SongOfSpirit.png", "Icons.Classes.Bard");
    // Ranger
    public static readonly Sprite? DeerHunter_Icon = RegisterSprite("SE_Hunter.png", "Icons.Classes.Ranger");
    public static readonly Sprite? CreatureMask_Icon = RegisterSprite("RangerSpawn.png", "Icons.Classes.Ranger");
    public static readonly Sprite? LuckyShot_Icon = RegisterSprite("SE_LuckyShot.png", "Icons.Classes.Ranger");
    public static readonly Sprite? RangerTrap_Icon = RegisterSprite("RangerTrap.png", "Icons.Classes.Ranger");
    public static readonly Sprite? QuickShot_Icon = RegisterSprite("SE_QuickShot.png", "Icons.Classes.Ranger");
    public static readonly Sprite? ChainShot_Icon = RegisterSprite("SE_ChainShot.png", "Icons.Classes.Ranger");
    // Sage
    public static readonly Sprite? LightningStrike_Icon = RegisterSprite("CallOfLightning.png", "Icons.Classes.Sage");
    public static readonly Sprite? Fireball_Icon = RegisterSprite("Fireball.png", "Icons.Classes.Sage");
    public static readonly Sprite? GoblinBeam_Icon = RegisterSprite("GoblinBeam.png", "Icons.Classes.Sage");
    public static readonly Sprite? IceBreath_Icon = RegisterSprite("IceBreath.png", "Icons.Classes.Sage");
    public static readonly Sprite? MeteorStrike_Icon = RegisterSprite("MeteorStrike.png", "Icons.Classes.Sage");
    public static readonly Sprite? BoulderStrike_Icon = RegisterSprite("BoulderStrike.png", "Icons.Classes.Sage");
    // Shaman
    public static readonly Sprite? ShamanRoots_Icon = RegisterSprite("RootBeam.png", "Icons.Classes.Shaman");                           // RootBeam
    public static readonly Sprite? ShamanRegeneration_Icon = RegisterSprite("SE_ShamanRegeneration.png", "Icons.Classes.Shaman");       // SE_ShamanRegeneration
    public static readonly Sprite? ShamanProtection_Icon = RegisterSprite("SE_ShamanShield.png", "Icons.Classes.Shaman");               // SE_ShamanShield
    public static readonly Sprite? ShamanHeal_Icon = RegisterSprite("ShamanHeal.png", "Icons.Classes.Shaman");                          // ShamanHeal
    public static readonly Sprite? ShamanSpawn_Icon = RegisterSprite("ShamanSpawn.png", "Icons.Classes.Shaman");                        // ShamanSpawn
    // Rogue
    public static readonly Sprite? Backstab_Icon = RegisterSprite("SE_RogueBackstab.png", "Icons.Classes.Rogue");                       // SE_RogueBackstab
    public static readonly Sprite? Bleeding_Icon = RegisterSprite("SE_RogueBleed.png", "Icons.Classes.Rogue");                          // SE_RogueBleed
    public static readonly Sprite? Reflect_Icon = RegisterSprite("SE_RogueReflect.png", "Icons.Classes.Rogue");                         // SE_RogueReflect
    public static readonly Sprite? QuickStep_Icon = RegisterSprite("SE_RogueSpeed.png", "Icons.Classes.Rogue");                         // SE_RogueSpeed
    public static readonly Sprite? Relentless_Icon = RegisterSprite("SE_RogueStamina.png", "Icons.Classes.Rogue");                      // SE_RogueStamina
    // Warrior
    public static readonly Sprite? DualWield_Icon = RegisterSprite("SE_DualWield.png", "Icons.Classes.Warrior");                        // SE_DualWield
    public static readonly Sprite? MonkeyWrench_Icon = RegisterSprite("SE_MonkeyWrench.png", "Icons.Classes.Warrior");                  // SE_MonkeyWrench
    public static readonly Sprite? HardHitter_Icon = RegisterSprite("SE_WarriorStrength.png", "Icons.Classes.Warrior");                 // SE_WarriorStrength
    public static readonly Sprite? BulkUp_Icon = RegisterSprite("SE_WarriorVitality.png", "Icons.Classes.Warrior");                     // SE_WarriorVitality
    public static readonly Sprite? Resistant_Icon = RegisterSprite("SE_WarriorResistance.png", "Icons.Classes.Warrior");                // SE_WarriorResistance



    
    public static Sprite Wishbone_Icon = null!;

    public static readonly Dictionary<string, Sprite> m_backgrounds = new ();
    public static void LoadSpriteResources()
    {
        GameObject wishbone = ZNetScene.instance.GetPrefab("Wishbone");
        if (wishbone.TryGetComponent(out ItemDrop wishboneItem))
        {
            Wishbone_Icon = wishboneItem.m_itemData.GetIcon();
        }
    }

    public static void ReadBackgroundFiles()
    {
        FilePaths.CreateFolders();
        foreach (var file in Directory.GetFiles(FilePaths.CustomBackgroundFilePath, "*.png"))
        {
            RegisterBackground(file);
        }
    }

    private static void RegisterBackground(string fileName)
    {
        if (!File.Exists(fileName)) return;

        byte[] fileData = File.ReadAllBytes(fileName);
        Texture2D texture = new Texture2D(4, 4);

        if (!texture.LoadImage(fileData)) return;
        texture.name = Path.GetFileName(fileName);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        m_backgrounds[texture.name] = sprite;
    }

    private static Sprite? RegisterSprite(string fileName, string folderName = "Icons")
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        string path = $"{ModName}.{folderName}.{fileName}";
        using var stream = assembly.GetManifestResourceStream(path);
        if (stream == null) return null;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        Texture2D texture = new Texture2D(2, 2);
        
        return texture.LoadImage(buffer) ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero) : null;
    }
}