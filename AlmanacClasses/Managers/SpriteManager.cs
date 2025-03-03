using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AlmanacClasses.FileSystem;
using UnityEngine;
using static AlmanacClasses.AlmanacClassesPlugin;

namespace AlmanacClasses.Managers;

public static class SpriteManager
{
    public static readonly Sprite HourGlass_Icon = _AssetBundle.LoadAsset<Sprite>("hourglass");
    public static readonly Sprite MeteorStrike_Icon = _AssetBundle.LoadAsset<Sprite>("FireMage_1");
    public static readonly Sprite GoblinBeam_Icon = _AssetBundle.LoadAsset<Sprite>("FireMage_19");
    public static readonly Sprite LightningStrike_Icon = _AssetBundle.LoadAsset<Sprite>("FrostMage_7");
    public static readonly Sprite BoulderStrike_Icon = _AssetBundle.LoadAsset<Sprite>("EarthMage_11");
    public static readonly Sprite Blink_Icon = _AssetBundle.LoadAsset<Sprite>("FrostMage_2");
    public static readonly Sprite SongOfSpeed_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_4");
    public static readonly Sprite SongOfVitality_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_10");
    public static readonly Sprite SongOfDamage_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_11");
    public static readonly Sprite SongOfHealing_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_7");
    public static readonly Sprite SongOfSpirit_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_21");
    public static readonly Sprite ShamanHeal_Icon = _AssetBundle.LoadAsset<Sprite>("Necromancer_24");
    public static readonly Sprite ShamanRoots_Icon = _AssetBundle.LoadAsset<Sprite>("EarthMage_18");
    public static readonly Sprite ShamanGhosts_Icon = _AssetBundle.LoadAsset<Sprite>("Necromancer_17");
    public static readonly Sprite ShamanRegeneration = _AssetBundle.LoadAsset<Sprite>("Necromancer_11");
    public static readonly Sprite ShamanProtection_Icon = _AssetBundle.LoadAsset<Sprite>("Necromancer_9");
    public static readonly Sprite DeerHunter_Icon = _AssetBundle.LoadAsset<Sprite>("Hunter_5");
    public static readonly Sprite LuckyShot_Icon = _AssetBundle.LoadAsset<Sprite>("Hunter_10");
    public static readonly Sprite CreatureMask_Icon = _AssetBundle.LoadAsset<Sprite>("Hunter_20");
    public static readonly Sprite RangerTrap_Icon = _AssetBundle.LoadAsset<Sprite>("Hunter_16");
    public static readonly Sprite QuickShot_Icon = _AssetBundle.LoadAsset<Sprite>("Hunter_18");
    public static readonly Sprite QuickStep_Icon = _AssetBundle.LoadAsset<Sprite>("Barbarian_32");
    public static readonly Sprite Reflect_Icon = _AssetBundle.LoadAsset<Sprite>("Barbarian_33");
    public static readonly Sprite Backstab_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_26");
    public static readonly Sprite Relentless_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_8");
    public static readonly Sprite Bleeding_Icon = _AssetBundle.LoadAsset<Sprite>("BloodMage_7");
    public static readonly Sprite HardHitter_Icon = _AssetBundle.LoadAsset<Sprite>("Barbarian_27");
    public static readonly Sprite BulkUp_Icon = _AssetBundle.LoadAsset<Sprite>("Barbarian_4");
    // public static readonly Sprite Impervious_Icon = AlmanacClassesPlugin._AssetBundle.LoadAsset<Sprite>("Barbarian_5");
    public static readonly Sprite Resistant_Icon = _AssetBundle.LoadAsset<Sprite>("Barbarian_16");
    public static readonly Sprite Sail_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_22");
    public static readonly Sprite ScrollMap = _AssetBundle.LoadAsset<Sprite>("scroll_map2");
    public static readonly Sprite WindIcon = _AssetBundle.LoadAsset<Sprite>("windicon");
    public static readonly Sprite ShieldIcon = _AssetBundle.LoadAsset<Sprite>("shield_basic_metal");
    public static readonly Sprite CrownIcon = _AssetBundle.LoadAsset<Sprite>("crown_gold");
    public static readonly Sprite WarriorIcon = _AssetBundle.LoadAsset<Sprite>("warrior_icon");
    public static readonly Sprite BardIcon = _AssetBundle.LoadAsset<Sprite>("bard_icon");
    public static readonly Sprite RangerIcon = _AssetBundle.LoadAsset<Sprite>("ranger_icon");
    public static readonly Sprite RogueIcon = _AssetBundle.LoadAsset<Sprite>("rogue_icon");
    public static readonly Sprite ShamanIcon = _AssetBundle.LoadAsset<Sprite>("shaman_icon");
    public static readonly Sprite MedalIcon = _AssetBundle.LoadAsset<Sprite>("medal");
    public static readonly Sprite Fireball_Icon = _AssetBundle.LoadAsset<Sprite>("FireMage_27");
    public static readonly Sprite ChainShot_Icon = _AssetBundle.LoadAsset<Sprite>("Monk_20");
    public static readonly Sprite? MonkeyWrench_Icon = RegisterSprite("monkeywrench_icon.png");
    public static readonly Sprite DualWield_Icon = _AssetBundle.LoadAsset<Sprite>("ability_sword_double");
    public static readonly Sprite Looter_Icon = _AssetBundle.LoadAsset<Sprite>("mapicon_trader");
    public static readonly Sprite MasterChef_Icon = _AssetBundle.LoadAsset<Sprite>("food_icon");
    
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

    private static Sprite? RegisterSprite(string fileName, string folderName = "icons")
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