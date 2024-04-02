using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.FileSystem;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine;

namespace AlmanacClasses
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class AlmanacClassesPlugin : BaseUnityPlugin
    {
        internal const string ModName = "AlmanacClasses";
        internal const string ModVersion = "0.2.2";
        internal const string Author = "RustyMods";
        private const string ModGUID = Author + "." + ModName;
        private static readonly string ConfigFileName = ModGUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource AlmanacClassesLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        public static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        public enum Toggle { On = 1, Off = 0 }

        public static readonly AssetBundle _AssetBundle = GetAssetBundle("classesbundle");
        public static AlmanacClassesPlugin _Plugin = null!;
        public static GameObject _Root = null!;
        public void Awake()
        {
            Localizer.Load(); 

            _Plugin = this;
            _Root = new GameObject("almanac_classes_root");
            DontDestroyOnLoad(_Root);
            _Root.SetActive(false);
            
            InitConfigs();
            FilePaths.CreateFolders();
            LoadPieces.LoadClassAltar();
            
            KG_Managers.AnimationReplaceManager.AddAnimationSet("classesbundle", "SpellCasting");
            KG_Managers.AnimationReplaceManager.AddAnimationSet("classesbundle", "StoneThrow");
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        public void Update()
        {
            TalentBook.UpdateTalentBookUI();
            AbilityManager.CheckInput();
            
            SpellElementChange.UpdateSpellMouseElement();
            ExperienceBarMove.UpdateElement();
            SpellBarMove.UpdateElement();
        }

        #region Utils
        private void OnDestroy() => Config.Save();
        
        private static AssetBundle GetAssetBundle(string fileName)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
            using Stream? stream = execAssembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(stream);
        }
        
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                AlmanacClassesLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                AlmanacClassesLogger.LogError($"There was an issue loading your {ConfigFileName}");
                AlmanacClassesLogger.LogError("Please check your config entries for spelling and format!");
            }
        }
        #endregion

        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        #region Settings
        public static ConfigEntry<int> _PrestigeThreshold = null!;
        public static ConfigEntry<int> _ResetCost = null!;
        public static ConfigEntry<Toggle> _DisplayExperience = null!;
        public static ConfigEntry<Vector2> _ExperienceBarPos = null!;
        public static ConfigEntry<Toggle> _HudVisible = null!;
        public static ConfigEntry<Vector2> _SpellBookPos = null!;
        private static ConfigEntry<Toggle> _PanelBackground = null!;
        public static ConfigEntry<int> _ExperienceMultiplier = null!;
        public static ConfigEntry<int> _TalentPointPerLevel = null!;
        public static ConfigEntry<int> _TalentPointsPerTenLevel = null!;
        #endregion

        public static ConfigEntry<int> _MaxEitr = null!;
        public static ConfigEntry<int> _MaxHealth = null!;
        public static ConfigEntry<int> _MaxStamina = null!;
        public static ConfigEntry<int> _EitrRatio = null!;
        public static ConfigEntry<int> _HealthRatio = null!;
        public static ConfigEntry<int> _StaminaRatio = null!;

        public static ConfigEntry<int> _DamageRatio = null!;

        #region Key Codes
        public static ConfigEntry<KeyCode> _SpellAlt = null!;
        public static ConfigEntry<KeyCode> _Spell1 = null!;
        public static ConfigEntry<KeyCode> _Spell2 = null!;
        public static ConfigEntry<KeyCode> _Spell3 = null!;
        public static ConfigEntry<KeyCode> _Spell4 = null!;
        public static ConfigEntry<KeyCode> _Spell5 = null!;
        public static ConfigEntry<KeyCode> _Spell6 = null!;
        public static ConfigEntry<KeyCode> _Spell7 = null!;
        public static ConfigEntry<KeyCode> _Spell8 = null!;
        #endregion
        #region Ranger
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerMeadowSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerBlackForestSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerSwampSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerMountainSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerPlainsSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerMistLandsSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerAshlandSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerDeepNorthSpawn = null!;
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _RangerOceanSpawn = null!;

        public static ConfigEntry<string> _HunterMeadows = null!;
        public static ConfigEntry<string> _HunterBlackForest = null!;
        public static ConfigEntry<string> _HunterSwamp = null!;
        public static ConfigEntry<string> _HunterMountain = null!;
        public static ConfigEntry<string> _HunterPlains = null!;
        public static ConfigEntry<string> _HunterMistLands = null!;
        public static ConfigEntry<string> _HunterAshLands = null!;
        public static ConfigEntry<string> _HunterDeepNorth = null!;
        public static ConfigEntry<string> _HunterOcean = null!;
        #endregion
        public static ConfigEntry<Classes.Abilities.SpawnSystem.SpawnOptions> _ShamanSpawn = null!;
        public static ConfigEntry<float> _MasterChefIncrease = null!;
        public static ConfigEntry<float> _LumberjackIncrease = null!;

        private void InitConfigs()
        {
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
                "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            InitSettingsConfigs();
            InitKeyCodeConfigs();

            _EitrRatio = config("4 - Characteristics", "1. Eitr Ratio", 2,
                new ConfigDescription("Set the amount of wisdom points required for 1 eitr", new AcceptableValueRange<int>(1, 10)));
            _HealthRatio = config("4 - Characteristics", "3. Health Ratio", 1,
                new ConfigDescription("Set the amount of constitution points for 1 health",
                    new AcceptableValueRange<int>(1, 10)));
            _StaminaRatio = config("4 - Characteristics", "5. Stamina Ratio", 3,
                new ConfigDescription("Set the amount of dexterity points for 1 stamina",
                    new AcceptableValueRange<int>(1, 10)));

            _MaxEitr = config("4 - Characteristics", "2. Max Eitr", 100,
                new ConfigDescription("Set max amount of eitr that can be gained from characteristic points",
                    new AcceptableValueRange<int>(0, 500)));
            _MaxHealth = config("4 - Characteristics", "4. Max Health", 100,
                new ConfigDescription("Set max amount of health that can be gained from characteristic points",
                    new AcceptableValueRange<int>(0, 500)));
            _MaxStamina = config("4 - Characteristics", "6. Max Stamina", 100,
                new ConfigDescription("Set max amount of stamina that can be gained from characteristic points",
                    new AcceptableValueRange<int>(0, 500)));

            _DamageRatio = config("4 - Characteristics", "7. Damage Ratio", 10,
                new ConfigDescription(
                    "Set the ratio of strength, dexterity, intelligence to increased damage output of, melee, ranged and magic damage",
                    new AcceptableValueRange<int>(0, 20)));
            
            _RangerMeadowSpawn = config("Ranger - Creature Mask", "1. Meadows",
                Classes.Abilities.SpawnSystem.SpawnOptions.Neck, "Set friendly spawn");
            _RangerBlackForestSpawn = config("Ranger - Creature Mask", "2. Black Forest",
                Classes.Abilities.SpawnSystem.SpawnOptions.Greydwarf, "Set friendly spawn");
            _RangerSwampSpawn = config("Ranger - Creature Mask", "3. Swamp",
                Classes.Abilities.SpawnSystem.SpawnOptions.Draugr, "Set friendly spawn");
            _RangerMountainSpawn = config("Ranger - Creature Mask", "4. Mountain",
                Classes.Abilities.SpawnSystem.SpawnOptions.Ulv, "Set friendly spawn");
            _RangerPlainsSpawn = config("Ranger - Creature Mask", "5. Plains",
                Classes.Abilities.SpawnSystem.SpawnOptions.Deathsquito, "Set friendly spawn");
            _RangerMistLandsSpawn = config("Ranger - Creature Mask", "6. Mistlands",
                Classes.Abilities.SpawnSystem.SpawnOptions.Tick, "Set friendly spawn");
            _RangerAshlandSpawn = config("Ranger - Creature Mask", "7. Ashlands",
                Classes.Abilities.SpawnSystem.SpawnOptions.Surtling, "Set friendly spawn");
            _RangerDeepNorthSpawn = config("Ranger - Creature Mask", "8. Deep North",
                Classes.Abilities.SpawnSystem.SpawnOptions.Fenring_Cultist, "Set friendly spawn");
            _RangerOceanSpawn = config("Ranger - Creature Mask", "9. Ocean",
                Classes.Abilities.SpawnSystem.SpawnOptions.Serpent, "Set friendly spawn");

            _ShamanSpawn = config("Shaman - Ghastly Ambitions", "Creature", Classes.Abilities.SpawnSystem.SpawnOptions.Ghost,
                "Set friendly spawn");
            _MasterChefIncrease = config("General - Master Chef", "Food Boost", 1.1f,
                new ConfigDescription("Set the modifier for food benefit, multiplied by prestige level",
                    new AcceptableValueRange<float>(1f, 2f)));
            _LumberjackIncrease = config("General - Lumberjack", "Chop Damage Boost", 1.1f,
                new ConfigDescription("Set the amount of chop damage increase, multiplied by prestige level",
                    new AcceptableValueRange<float>(1f, 2f)));

            _HunterMeadows = config("Ranger - Hunter", "1. Meadows", "Deer", "Set the affected creature by the ability");
            _HunterBlackForest = config("Ranger - Hunter", "2. Black Forest", "Greydwarf", "Set the affected creature by the ability");
            _HunterSwamp = config("Ranger - Hunter", "3. Swamp", "Draugr", "Set the affected creature by the ability");
            _HunterMountain = config("Ranger - Hunter", "4. Mountains", "Wolf", "Set the affected creature by the ability");
            _HunterPlains = config("Ranger - Hunter", "5. Plains", "Lox", "Set the affected creature by the ability");
            _HunterMistLands = config("Ranger - Hunter", "6. Mistlands", "Hare", "Set the affected creature by the ability");
            _HunterAshLands = config("Ranger - Hunter", "7. Ashlands", "Surtling", "Set the affected creature by the ability");
            _HunterDeepNorth = config("Ranger - Hunter", "8. Deep North", "", "Set the affected creature by the ability");
            _HunterOcean = config("Ranger - Hunter", "9. Ocean", "Serpent", "Set the affected creature by the ability");

        }

        private void InitSettingsConfigs()
        {
            _PrestigeThreshold = config("2 - Settings", "Prestige Threshold", 60,
                new ConfigDescription("Minimum amount of talent points needed to spend to access prestige",
                    new AcceptableValueRange<int>(1, 101)));
            _ResetCost = config("2 - Settings", "Reset Cost", 999,
                new ConfigDescription("Set the cost to reset talents", new AcceptableValueRange<int>(0, 999)));

            _DisplayExperience = config("2 - Settings", "Show Creature Experience", Toggle.Off,
                "If on, creature hover names will display the amount of experience they give");

            _ExperienceBarPos = config("2 - Settings", "XP Bar Postion", new Vector2(300f, 25f),
                "Set the position of the experience bar", false);
            _ExperienceBarPos.SettingChanged += LoadUI.OnChangeExperienceBarPosition;

            _HudVisible = config("2 - Settings", "XP Bar Visible", Toggle.On, "If on, experience bar is visible on HUD",
                false);
            _HudVisible.SettingChanged += LoadUI.OnChangeExperienceBarVisibility;

            _SpellBookPos = config("2 - Settings", "Spell Bar Position", new Vector2(1500f, 100f),
                "Set the location of the spellbar", false);
            _SpellBookPos.SettingChanged += SpellBook.OnSpellBarPosChange;
            
            _PanelBackground = config("2 - Settings", "UI Background", Toggle.On,
                "If on, panel is visible, else transparent", false);
            _PanelBackground.SettingChanged += OnPanelBackgroundToggle;

            _ExperienceMultiplier = config("2 - Settings", "Experience Multiplier", 1,
                new ConfigDescription("Set experience multiplier to easily increase experience gains",
                    new AcceptableValueRange<int>(1, 10)));
            _TalentPointPerLevel = config("2 - Settings", "Talent Points Per Level", 3,
                new ConfigDescription("Set amount of talent points rewarded per level",
                    new AcceptableValueRange<int>(1, 10)));
            _TalentPointsPerTenLevel = config("2 - Settings", "Talent Points Per Ten Levels", 7,
                new ConfigDescription("Set extra talent points rewarded per 10 levels",
                    new AcceptableValueRange<int>(0, 10)));
        }

        private void InitKeyCodeConfigs()
        {
            _SpellAlt = config("3 - Spell Keys", "Alt Key", KeyCode.None, "Set the alt key code, If None, then it ignores", false);
            _SpellAlt.SettingChanged += OnSpellAltKeyChange;
            _Spell1 = config("3 - Spell Keys", "Spell 1", KeyCode.Keypad1, "Set the key code for spell 1", false);
            _Spell2 = config("3 - Spell Keys", "Spell 2", KeyCode.Keypad2, "Set the key code for spell 2", false);
            _Spell3 = config("3 - Spell Keys", "Spell 3", KeyCode.Keypad3, "Set the key code for spell 3", false);
            _Spell4 = config("3 - Spell Keys", "Spell 4", KeyCode.Keypad4, "Set the key code for spell 4", false);
            _Spell5 = config("3 - Spell Keys", "Spell 5", KeyCode.Keypad5, "Set the key code for spell 5", false);
            _Spell6 = config("3 - Spell Keys", "Spell 6", KeyCode.Keypad6, "Set the key code for spell 6", false);
            _Spell7 = config("3 - Spell Keys", "Spell 7", KeyCode.Keypad7, "Set the key code for spell 7", false);
            _Spell8 = config("3 - Spell Keys", "Spell 8", KeyCode.Keypad8, "Set the key code for spell 8", false);
        }

        private static void OnPanelBackgroundToggle(object sender, EventArgs e)
        {
            LoadUI.PanelBackground.color = _PanelBackground.Value is Toggle.On ? Color.white : Color.clear;
        }

        private static void OnSpellAltKeyChange(object sender, EventArgs e)
        {
            LoadUI.SpellBarHotKeyTooltip.text = $"Spell Book Alt Key: <color=orange>{_SpellAlt.Value}</color>";
        }
        #region Config Utils
        public ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        public ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order;
            [UsedImplicitly] public bool? Browsable;
            [UsedImplicitly] public string? Category;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        }
        #endregion
        #endregion
    }

    public static class KeyboardExtensions
    {
        public static bool IsKeyDown(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey) &&
                   shortcut.Modifiers.All(Input.GetKey);
        }

        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) &&
                   shortcut.Modifiers.All(Input.GetKey);
        }
    }
}