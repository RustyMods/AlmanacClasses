using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Data;
using AlmanacClasses.FileSystem;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx.Configuration;
using UnityEngine;

namespace AlmanacClasses.Classes;

public static class TalentManager
{
    public static readonly Dictionary<string, Talent> AllTalents = new();

    public static void InitTalents(int level)
    {
        FilePaths.CreateFolders();
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Initializing talents");
        AllTalents.Clear();
        LoadBardTalents(level);
        LoadSageTalents(level);
        LoadRangerTalents(level);
        LoadShamanTalents(level);
        LoadWarriorTalents(level);
        LoadRogueTalents(level);
        LoadCoreTalents(level);
    }

    public static void PrestigeTalents(int level)
    {
        foreach (KeyValuePair<string, Talent> talent in AllTalents)
        {
            talent.Value.m_level = level;
            if (talent.Value.m_type is TalentType.Characteristic)
            {
                talent.Value.m_description =
                    $"+ <color=orange>{talent.Value.m_characteristicValue * talent.Value.m_level}</color> {DefaultData.LocalizeCharacteristics[talent.Value.m_characteristic]}";
            }
        }

        foreach (KeyValuePair<string, Talent> talent in PlayerManager.m_playerTalents)
        {
            talent.Value.m_level = level;
            if (talent.Value.m_type is TalentType.Characteristic)
            {
                talent.Value.m_description =
                    $"+ <color=orange>{talent.Value.m_characteristicValue * talent.Value.m_level}</color> {DefaultData.LocalizeCharacteristics[talent.Value.m_characteristic]}";
            }
        }

        foreach (AbilityData? talent in SpellBook.m_abilities)
        {
            talent.m_data.m_level = level;
            if (talent.m_data.m_type is TalentType.Characteristic)
            {
                talent.m_data.m_description =
                    $"+ <color=orange>{talent.m_data.m_characteristicValue * talent.m_data.m_level}</color> {DefaultData.LocalizeCharacteristics[talent.m_data.m_characteristic]}";
            }
        }
    }

    public static Talent? GetTalentByButton(string buttonName)
    {
        return AllTalents.Values.ToList().Find(x => x.m_buttonName == buttonName);
    }

    private static int GetTotalPlayerTalents(int level)
    {
        int points = AlmanacClassesPlugin._TalentPointPerLevel.Value * level;
        int extraPoints = (level / 10) * AlmanacClassesPlugin._TalentPointsPerTenLevel.Value;
        return points + extraPoints - PlayerManager.m_tempPlayerData.m_prestigePoints;
    }

    public static int GetTotalBoughtTalentPoints()
    {
        return PlayerManager.m_playerTalents.Sum(talent => talent.Value.m_cost);
    }

    public static int GetAvailableTalentPoints()
    {
        int boughtTalents = GetTotalBoughtTalentPoints();
        int level = PlayerManager.GetPlayerLevel(PlayerManager.m_tempPlayerData.m_experience);
        int totalPoints = GetTotalPlayerTalents(level);
        return totalPoints - boughtTalents;
    }

    private static void LoadCoreTalents(int level)
    {
        List<Talent> characteristicTalents = new()
        {
            new()
            {
                m_key = "PrestigeButton",
                m_name = "$talent_prestige",
                m_description = "$talent_prestige_desc",
                m_level = level,
                m_type = TalentType.Prestige,
                m_cost = 20 * level,
                m_buttonName = "$button_center"
            },
            new()
            {
                m_key = "Core1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_1"
            },
            new()
            {
                m_key = "Core2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_2"
            },
            new()
            {
                m_key = "Core3",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_3"
            },
            new()
            {
                m_key = "Core4",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_4"
            },
            new()
            {
                m_key = "Core5",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_5"
            },
            new()
            {
                m_key = "Core6",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_6"
            },
            new()
            {
                m_key = "Core7",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_7"
            },
            new()
            {
                m_key = "Core8",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_8"
            },
            new()
            {
                m_key = "Core9",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_9"
            },
            new()
            {
                m_key = "Core10",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_10"
            },
            new()
            {
                m_key = "Core11",
                m_name = "$almanac_constitution",
                m_description = $"+ {10 * level} $almanac_constitution",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Constitution,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_11"
            },
            new()
            {
                m_key = "Core12",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_core_12"
            },
            new()
            {
                m_key = "CoreSneak",
                m_name = "$talent_sneaky",
                m_description = "$talent_sneaky_desc",
                m_level = level,
                m_type = TalentType.Passive,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Stealth , AlmanacClassesPlugin._Plugin.config("General - Sneaky", "Stealth Modifier", 1.1f, new ConfigDescription("Set stealth modifier", new AcceptableValueRange<float>(1f, 2f)))}
                },
                
                m_statusEffect = new()
                {
                    name = "SE_Sneaker",
                    m_name = "$talent_sneaky",
                },
                m_buttonName = "$button_sneak",
                m_triggerNow = true,
            },
            new()
            {
                m_key = "CoreMerchant",
                m_name = "$talent_pickpocket",
                m_description = "$talent_pickpocket_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_merchant",
                m_chance = AlmanacClassesPlugin._Plugin.config("General - Pickpocket", "Chance", 20f, new ConfigDescription("Set base chance to get double loot", new AcceptableValueRange<float>(0, 100f)))
            },
            new()
            {
                m_key = "CoreSail",
                m_name = "$talent_sailor",
                m_description = "$talent_sailor_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_animation = "gpower",
                m_attribute = StatusEffect.StatusAttribute.SailingPower,
                m_duration = AlmanacClassesPlugin._Plugin.config("General - Sailor", "Duration", 25f, new ConfigDescription("Set the base duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                m_statusEffect = new()
                {
                    name = "SE_Sailor",
                    m_name = "$talent_sailor",
                    m_startEffects = LoadedAssets.GP_Moder.m_startEffects,
                },
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("General - Sailor", "Eitr Cost", 10, new ConfigDescription("Set eitr cost", new AcceptableValueRange<int>(0, 101))),
                m_ttl = AlmanacClassesPlugin._Plugin.config("General - Sailor", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_buttonName = "$button_sail",
                m_cost = 5,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("General - Sailor", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "RainProof",
                m_name = "$talent_rain_proof",
                m_description = "$talent_rain_proof_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_rain",
                m_cost = 5
            },
            new()
            {
                m_key = "CoreLumberjack",
                m_name = "$talent_lumberjack",
                m_description = "$talent_lumberjack_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_comfort_1",
            },
            new ()
            {
                m_key = "CoreComfort1",
                m_name = "$talent_airbender",
                m_description = "$talent_airbender_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("General - Airbender", "Eitr Cost", 2, new ConfigDescription("Set eitr cost to jump in air", new AcceptableValueRange<int>(0, 101))),
                m_healthCost = AlmanacClassesPlugin._Plugin.config("General - Airbender", "Jump Amount", 1, new ConfigDescription("Set base amount of air jumps, multiplied by the prestige level", new AcceptableValueRange<int>(1, 10))),
                m_buttonName = "$button_lumberjack"
            },
            new()
            {
                m_key = "CoreChef",
                m_name = "$talent_master_chef",
                m_description = "$talent_master_chef_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_chef",
                m_cost = 5
            },
            new()
            {
                m_key = "CoreCarry",
                m_name = "$talent_pack_mule",
                m_description = "$talent_pack_mule_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_shield",
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.MaxCarryWeight , AlmanacClassesPlugin._Plugin.config("General - Pack Mule", "Carry Weight Modifier", 25f, new ConfigDescription("Set max carry weight modifier", new AcceptableValueRange<float>(0f, 101f)))}
                },
                
                m_statusEffect = new()
                {
                    name = "SE_PackMule",
                    m_name = "Pack Mule",
                },
                m_triggerNow = true,
                m_cost = 5
            },
            new ()
            {
                m_key = "CoreComfort2",
                m_name = "$talent_comfort",
                m_description = "$talent_comfort_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_comfortAmount = AlmanacClassesPlugin._Plugin.config("General - Comfort", "Comfort Bonus", 2, new ConfigDescription("Set added comfort bonus, multiplied by prestige level", new AcceptableValueRange<int>(1, 10))),
                m_buttonName = "$button_comfort_2"
            }
        };

        InitTalents(characteristicTalents);
    }

    private static void LoadRangerTalents(int level)
    {
        List<Talent> hunterTalents = new()
        {
            new()
            {
                m_key = "RangerTamer",
                m_name = "$talent_creature_mask",
                m_sprite = SpriteManager.CreatureMask_Icon,
                m_description = "$talent_creature_mask_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerHunterSpawn",
                m_isAbility = true,
                m_buttonName = "$button_ranger_talent_3",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Ranger - Creature Mask", "Eitr Cost", 10, new ConfigDescription("Set eitr cost to spawn friendly creature", new AcceptableValueRange<int>(0, 101))),
                m_healthCost = AlmanacClassesPlugin._Plugin.config("Ranger - Creature Mask", "Heath Cost", 10, new ConfigDescription("Set health cost to spawn friendly creature", new AcceptableValueRange<int>(0, 101))),
                m_skill = Skills.SkillType.Bows
            },
            new()
            {
                m_key = "CoreTreasure",
                m_name = "$talent_treasure_hunter",
                m_description = "$talent_treasure_hunter_desc",
                m_level = level,
                m_type = TalentType.Finder,
                m_ttl = AlmanacClassesPlugin._Plugin.config("General - Treasure Hunter", "Cooldown", 25f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_buttonName = "$button_treasure",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("General - Treasure Hunter", "Eitr Cost", 4, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
            },
            new()
            {
                m_key = "DeerHunter",
                m_name = "$talent_hunter",
                m_description = "$talent_hunter_desc",
                m_sprite = SpriteManager.DeerHunter_Icon,
                m_level = level,
                m_type = TalentType.Finder,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Ranger - Hunter", "Cooldown", 25f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_findCharacter = true,
                m_buttonName = "$button_ranger_talent_1",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Ranger - Hunter", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Ranger - Hunter", "Stamina Cost", 5, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_skill = Skills.SkillType.Bows
            },
            new()
            {
                m_key = "Ranger1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_ranger_1"
            },
            new()
            {
                m_key = "Ranger2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_ranger_2"
            },
            new()
            {
                m_key = "Ranger3",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_ranger_3"
            },
            new()
            {
                m_key = "Ranger4",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_ranger_4"
            },
            new()
            {
                m_key = "Ranger5",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_ranger_5"
            },
            new()
            {
                m_key = "Ranger6",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_ranger_6"
            },
            new()
            {
                m_key = "LuckyShot",
                m_name = "$talent_lucky_shot",
                m_sprite = SpriteManager.LuckyShot_Icon,
                m_description = "$talent_lucky_shot_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_ranger_talent_2",
                m_chance = AlmanacClassesPlugin._Plugin.config("Ranger - Lucky Shot", "Chance", 20f, new ConfigDescription("Set the base chance to get projectile back, multiplied by the prestige level", new AcceptableValueRange<float>(0f, 100f)))
            },
            new()
            {
                m_key = "QuickShot",
                m_name = "$talent_quick_shot",
                m_sprite = SpriteManager.QuickShot_Icon,
                m_description = "$talent_quick_shot_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                m_cost = 5,
                m_duration = AlmanacClassesPlugin._Plugin.config("Ranger - Quick Shot", "Duration", 20f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_QuickShot",
                    m_name = "$talent_quick_shot",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_chance = AlmanacClassesPlugin._Plugin.config("Ranger - Quick Shot", "Draw Speed Increase", 10f, new ConfigDescription("Set the draw speed multiplier", new AcceptableValueRange<float>(0f, 100f))),
                m_buttonName = "$button_ranger_talent_5",
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Ranger - Quick Shot", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Ranger - Quick Shot", "Stamina Cost", 20, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_animation = "nonono",
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Ranger - Quick Shot", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "RangerTrap",
                m_name = "$talent_trapped",
                m_sprite = SpriteManager.RangerTrap_Icon,
                m_description = "$talent_trapped_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_buttonName = "$button_ranger_talent_4",
                m_isAbility = true,
                m_ability = "TriggerSpawnTrap",
                m_talentDamages = new TalentDamages()
                {
                    pierce = AlmanacClassesPlugin._Plugin.config("Ranger - Trapped", "Pierce Damage", 50f, new ConfigDescription("Set pierce damage", new AcceptableValueRange<float>(0f, 101f))),
                    poison = AlmanacClassesPlugin._Plugin.config("Ranger - Trapped", "Poison Damage", 20f, new ConfigDescription("Set poison damage", new AcceptableValueRange<float>(0f, 101f)))
                },
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Ranger - Trapped", "Eitr Cost", 4, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Ranger - Trapped", "Stamina Cost", 10, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_ttl = AlmanacClassesPlugin._Plugin.config("Ranger - Trapped", "Cooldown", 10f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101f))),
                m_skill = Skills.SkillType.Bows
            }
        };
        InitTalents(hunterTalents);
    }

    private static void LoadSageTalents(int level)
    {
        List<Talent> sageTalents = new()
        {
            new()
            {
                m_key = "CallOfLightning",
                m_name = "$talent_lightning",
                m_sprite = SpriteManager.LightningStrike_Icon,
                m_description = "$talent_lightning_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerLightningAOE",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Sage - Call of Lightning", "Eitr Cost", 10, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_buttonName = "$button_sage_talent_4",
                m_ttl = AlmanacClassesPlugin._Plugin.config("Sage - Call of Lightning", "Cooldown", 20f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_talentDamages = new()
                {
                    pierce = AlmanacClassesPlugin._Plugin.config("Sage - Call of Lightning", "Pierce Damage", 20f, new ConfigDescription("Set base pierce damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101))),
                    lightning = AlmanacClassesPlugin._Plugin.config("Sage - Call of Lightning", "Lightning Damage", 40f, new ConfigDescription("Set base lightning damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101)))
                },
                m_skill = Skills.SkillType.ElementalMagic,
                m_animation = "point"
            },
            new()
            {
                m_key = "MeteorStrike",
                m_name = "$talent_meteor",
                m_sprite = SpriteManager.MeteorStrike_Icon,
                m_description = "$talent_meteor_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerMeteor",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Sage - Meteor Strike", "Eitr Cost", 20, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_buttonName = "$button_sage_talent_3",
                m_ttl = AlmanacClassesPlugin._Plugin.config("Sage - Meteor Strike", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_talentDamages = new()
                {
                    blunt = AlmanacClassesPlugin._Plugin.config("Sage - Meteor Strike", "Blunt Damage", 50f, new ConfigDescription("Set base blunt damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101))),
                    fire = AlmanacClassesPlugin._Plugin.config("Sage - Meteor Strike", "Fire Damage", 50f, new ConfigDescription("Set base fire damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101)))
                },
                m_skill = Skills.SkillType.ElementalMagic,
                m_animation = "point"
            },
            new()
            {
                m_key = "StoneThrow",
                m_name = "$talent_boulder",
                m_sprite = SpriteManager.BoulderStrike_Icon,
                m_description = "$talent_boulder_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerStoneThrow",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Sage - Boulder Strike", "Eitr Cost", 4, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_ttl = AlmanacClassesPlugin._Plugin.config("Sage - Boulder Stroke", "Cooldown", 10f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_buttonName = "$button_sage_talent_1",
                m_talentDamages = new()
                {
                    blunt = AlmanacClassesPlugin._Plugin.config("Sage - Boulder Strike", "Blunt Damage", 50f, new ConfigDescription("Set base blunt damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101)))
                },
                m_skill = Skills.SkillType.ElementalMagic,
                m_animation = "point"
            },
            new()
            {
                m_key = "GoblinBeam",
                m_name = "$talent_beam",
                m_sprite = SpriteManager.GoblinBeam_Icon,
                m_description = "$talent_beam_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerGoblinBeam",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Sage - Goblin Beam", "Eitr Cost", 20, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_buttonName = "$button_sage_talent_2",
                m_ttl = AlmanacClassesPlugin._Plugin.config("Sage - Goblin Beam", "Cooldown", 25f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_talentDamages = new()
                {
                    pierce = AlmanacClassesPlugin._Plugin.config("Sage - Goblin Beam", "Pierce Damage", 13f, new ConfigDescription("Set base pierce damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101))),
                    fire = AlmanacClassesPlugin._Plugin.config("Sage - Goblin Beam", "Fire Damage", 13f, new ConfigDescription("Set base fire damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101))),
                    lightning = AlmanacClassesPlugin._Plugin.config("Sage - Goblin Beam", "Lightning Damage", 13f, new ConfigDescription("Set base lightning damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101)))
                },
                m_skill = Skills.SkillType.ElementalMagic,
                m_animation = "roar"
            },
            new()
            {
                m_key = "IceBreath",
                m_name = "$talent_ice_breath",
                m_sprite = SpriteManager.Blink_Icon,
                m_description = "$talent_ice_breath_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerIceBreath",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Sage - Ice Breath", "Eitr Cost", 30, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_ttl = AlmanacClassesPlugin._Plugin.config("Sage - Ice Breath", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_buttonName = "$button_sage_talent_5",
                m_cost = 5,
                m_skill = Skills.SkillType.ElementalMagic,
                m_talentDamages = new()
                {
                    frost = AlmanacClassesPlugin._Plugin.config("Sage - Ice Breath", "Frost Damage", 10f, new ConfigDescription("Set base frost damage, multiplied by prestige level", new AcceptableValueRange<float>(0, 101f))),
                    slash = AlmanacClassesPlugin._Plugin.config("Sage - Ice Breath", "Slash Damage", 10f, new ConfigDescription("Set base slash damage, multiplied by prestige level", new AcceptableValueRange<float>(0, 101f)))
                },
                m_animation = "roar"
            },
            new()
            {
                m_key = "Sage1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_sage_1"
            },
            new()
            {
                m_key = "Sage2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_sage_2"
            },
            new()
            {
                m_key = "Sage3",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_sage_3"
            },
            new()
            {
                m_key = "Sage4",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_sage_4"
            },
            new()
            {
                m_key = "Sage5",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_sage_5"
            },
            new()
            {
                m_key = "Sage6",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_sage_6"
            },
        };

        InitTalents(sageTalents);
    }

    private static void LoadShamanTalents(int level)
    {
        List<Talent> shamanTalents = new()
        {
            new()
            {
                m_key = "ShamanHeal",
                m_name = "$talent_shaman_heal",
                m_sprite = SpriteManager.ShamanHeal_Icon,
                m_description = "$talent_shaman_heal_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_heal = AlmanacClassesPlugin._Plugin.config("Shaman - Heal", "Heal Amount", 200f, new ConfigDescription("Set base amount healed, multiplied by the prestige level", new AcceptableValueRange<float>(0, 300f))),
                m_ability = "TriggerHeal",
                m_ttl = AlmanacClassesPlugin._Plugin.config("Shaman - Heal", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_isAbility = true,
                m_buttonName = "$button_shaman_talent_1",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Shaman - Heal", "Eitr Cost", 4, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_radius = 10f,
            },
            new()
            {
                m_key = "ShamanShield",
                m_name = "$talent_shaman_shield",
                m_sprite = SpriteManager.ShamanProtection_Icon,
                m_description = "$talent_shaman_shield_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.DamageAbsorb , AlmanacClassesPlugin._Plugin.config("Shaman Protection", "Damage Absorb", 75f, new ConfigDescription("Set base damage absorb amount, multiplied by the prestige level", new AcceptableValueRange<float>(0f, 200f)))}
                },
                m_animation = "staff_summon",
                m_radius = 10f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Shaman - Protection", "Duration", 30f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),

                m_statusEffect = new()
                {
                    name = "SE_ShamanProtection",
                    m_name = "Shaman Protection",
                },
                m_ttl = AlmanacClassesPlugin._Plugin.config("Shaman - Protection", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_isAbility = true,
                m_buttonName = "$button_shaman_talent_5",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Shaman - Protection", "Eitr Cost", 15, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_cost = 5,
                m_skill = Skills.SkillType.BloodMagic
            },
            new()
            {
                m_key = "ShamanRegeneration",
                m_name = "$talent_shaman_regen",
                m_sprite = SpriteManager.ShamanRegeneration,
                m_description = "$talent_shaman_regen_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.EitrRegen , AlmanacClassesPlugin._Plugin.config("Shaman - Regeneration", "Eitr Regen", 1.1f, new ConfigDescription("Set eitr regeneration amount, 1.1 = 110%, 1.5 = 150%, 2 = 200%", new AcceptableValueRange<float>(1f, 2f)))},
                    { StatusEffectData.Modifier.Eitr , AlmanacClassesPlugin._Plugin.config("Shaman - Regeneration", "Eitr", 10f, new ConfigDescription("Set amount of extra eitr, multiplied by the prestige level", new AcceptableValueRange<float>(0f, 101f)))}
                },
                m_animation = "roar",
                m_radius = 10f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Shaman - Regeneration", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_ShamanRegeneration",
                    m_name = "$talent_shaman_regen",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_ttl = AlmanacClassesPlugin._Plugin.config("Shaman - Regeneration", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_isAbility = true,
                m_buttonName = "$button_shaman_talent_4",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Shaman - Regeneration", "Eitr Cost", 10, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Shaman - Regeneration", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "ShamanGhosts",
                m_name = "$talent_shaman_spawn",
                m_sprite = SpriteManager.ShamanGhosts_Icon,
                m_description = "$talent_shaman_spawn_desc",
                m_level = Mathf.Clamp(level, 0, 3),
                m_type = TalentType.Ability,
                m_ability = "TriggerShamanSpawn",
                m_isAbility = true,
                m_buttonName = "$button_shaman_talent_3",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Shaman - Ghastly Ambitions", "Eitr Cost", 30, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_ttl = AlmanacClassesPlugin._Plugin.config("Shaman - Ghastly Ambitions", "Cooldown", 30f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic
            },
            new()
            {
                m_key = "RootBeam",
                m_name = "$talent_root",
                m_sprite = SpriteManager.ShamanRoots_Icon,
                m_description = "$talent_root_desc",
                m_level = level,
                m_type = TalentType.Ability,
                m_ability = "TriggerRootBeam",
                m_isAbility = true,
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Shaman - Rooting", "Eitr Cost", 20, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_buttonName = "$button_shaman_talent_2",
                m_talentDamages = new()
                {
                    poison = AlmanacClassesPlugin._Plugin.config("Shaman - Rooting", "Poison Damage", 20f, new ConfigDescription("Set base poison damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101))),
                    slash = AlmanacClassesPlugin._Plugin.config("Shaman - Rooting", "Slash Damage", 10f, new ConfigDescription("Set base slash damage, multiplied by the prestige level", new AcceptableValueRange<float>(0, 101)))
                },
                m_skill = Skills.SkillType.ElementalMagic,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Shaman - Rooting", "Cooldown", 10f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_animation = "roar"
            },
            new()
            {
                m_key = "Shaman1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_shaman_1"
            },
            new()
            {
                m_key = "Shaman2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_shaman_2"
            },
            new()
            {
                m_key = "Shaman3",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_shaman_3"
            },
            new()
            {
                m_key = "Shaman4",
                m_name = "$almanac_intelligence",
                m_description = $"+ {10 * level} $almanac_intelligence",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Intelligence,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_shaman_4"
            },
            new()
            {
                m_key = "Shaman5",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_shaman_5"
            },
            new()
            {
                m_key = "Shaman6",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_shaman_6"
            },
        };
        InitTalents(shamanTalents);
    }

    private static void LoadBardTalents(int level)
    {
        List<Talent> bardTalents = new()
        {
            new ()
            {
                m_key = "SongOfDamage",
                m_name = "$talent_song_damage",
                m_sprite = SpriteManager.SongOfDamage_Icon,
                m_description = $"$talent_song_damage_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Attack , AlmanacClassesPlugin._Plugin.config("Bard - Song of Damage", "Attack Boost", 1.1f, new ConfigDescription("Set attack damage output increase, multiplied by the prestige level, 1.1 = 110%, 1.5 = 150%, 2 = 200%", new AcceptableValueRange<float>(1f, 2f)))}
                },
                m_animation = "dance",
                m_radius = 10f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Bard - Song of Damage", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Damage_Song",
                    m_name = "$talent_song_damage",
                    m_startEffects = LoadedAssets.AddBardFX(new Color(1f, 1f, 0f, 1f), "VFX_Song_Yellow"),
                },
                m_buttonName = "$button_bard_talent_3",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Bard - Song of Damage", "Eitr Cost", 20, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Bard - Song of Damage", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.ElementalMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Bard - Song of Damage", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new ()
            {
                m_key = "SongOfHealing",
                m_name = "$talent_song_heal",
                m_sprite = SpriteManager.SongOfHealing_Icon,
                m_description = "$talent_song_heal_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Heal , AlmanacClassesPlugin._Plugin.config("Bard - Song of Healing", "Heal Amount", 100f, new ConfigDescription("Set total amount of heal, multiplied by the prestige level", new AcceptableValueRange<float>(0f, 200f)))}
                },
                m_animation = "dance",
                m_radius = 10f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Bard - Song of Healing", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Healing_Song",
                    m_name = "$talent_song_heal",
                    m_startEffects = LoadedAssets.AddBardFX(new Color(1f, 0.5f, 0f, 1f), "VFX_Song_Orange"),
                },
                m_buttonName = "$button_bard_talent_4",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Bard - Song of Healing", "Eitr Cost", 25, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Bard - Song of Healing", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Bard - Song of Healing", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new ()
            {
                m_key = "SongOfVitality",
                m_name = "$talent_song_vitality",
                m_sprite = SpriteManager.SongOfVitality_Icon,
                m_description = "$talent_song_vitality_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Vitality , AlmanacClassesPlugin._Plugin.config("Bard - Song of Vitality", "Vitality", 50f, new ConfigDescription("Set amount of vitality rewarded, multiplied by the prestige level", new AcceptableValueRange<float>(0f, 101f)))}
                },
                m_animation = "dance",
                m_radius = 10f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Bard - Song of Vitality", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Health_Song",
                    m_name = "$talent_song_vitality",
                    m_startEffects = LoadedAssets.FX_MusicNotes,
                },
                m_buttonName = "$button_bard_talent_2",
                m_cost = 3,
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Bard - Song of Vitality", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Bard - Song of Vitality", "Eitr Cost", 4, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Bard - Song of Vitality", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "SongOfSpeed",
                m_name = "$talent_song_speed",
                m_sprite = SpriteManager.SongOfSpeed_Icon,
                m_description = "$talent_song_speed",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Speed , AlmanacClassesPlugin._Plugin.config("Bard - Song of Speed", "Speed Modifier", 1.20f, new ConfigDescription("Set base of speed increase, multiplied by prestige level, 1.1 = 110%, 1.5 = 150%, 2 = 200%", new AcceptableValueRange<float>(1f, 2f)))}
                },
                m_animation = "dance",
                m_radius = 10f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Bard - Song of Speed", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Speed_Song",
                    m_name = "$talent_song_speed",
                    m_startEffects = LoadedAssets.FX_MusicNotes,
                },
                m_buttonName = "$button_bard_talent_1",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Bard - Song of Speed", "Eitr Cost", 4, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Bard - Song of Speed", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Bard - Song of Speed", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "SongOfSpirit",
                m_name = "$talent_song_spirit",
                m_sprite = SpriteManager.SongOfSpirit_Icon,
                m_description = "$talent_song_spirit_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_animation = "dance",
                m_radius = 20f,
                m_damageInterval = 1f,
                m_duration = AlmanacClassesPlugin._Plugin.config("Bard - Song of Spirit", "Duration", 10f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Taunt_Song",
                    m_name = "$talent_song_spirit",
                    m_startEffects = LoadedAssets.FX_MusicNotes,
                },
                m_buttonName = "$button_bard_talent_5",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Bard - Song of Spirit", "Eitr Cost", 25, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_talentDamages = new()
                {
                    spirit = AlmanacClassesPlugin._Plugin.config("Bard - Song of Spirit", "Spirit Damage", 10f, new ConfigDescription("Set base spirit damage, multiplied by prestige level", new AcceptableValueRange<float>(0, 101))),
                    lightning = AlmanacClassesPlugin._Plugin.config("Bard - Song of Spirit", "Lightning Damage", 10f, new ConfigDescription("Set base lightning damage, multiplied by prestige level", new AcceptableValueRange<float>(0, 101)))
                },
                m_cost = 5,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Bard - Song of Spirit", "Cooldown", 25f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.ElementalMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Bard - Song of Spirit", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "Bard1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_bard_1"
            },
            new()
            {
                m_key = "Bard2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_bard_2"
            },
            new()
            {
                m_key = "Bard3",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_bard_3"
            },
            new()
            {
                m_key = "Bard4",
                m_name = "$almanac_constitution",
                m_description = $"+ {10 * level} $almanac_constitution",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Constitution,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_bard_4"
            },
            new()
            {
                m_key = "Bard5",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_bard_5"
            },
            new()
            {
                m_key = "Bard6",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_bard_6"
            },
        };
        InitTalents(bardTalents);
    }

    private static void LoadRogueTalents(int level)
    {
        List<Talent> rogueTalents = new()
        {
            new ()
            {
                m_key = "RogueSpeed",
                m_name = "$talent_rogue_speed",
                m_sprite = SpriteManager.QuickStep_Icon,
                m_description = "$talent_rogue_speed_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Speed , AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Speed Modifier", 1.1f, new ConfigDescription("Set amount of speed increase, multiplied by prestige level, 1.1 = 110%, 1.5 = 150%, 2 = 200%", new AcceptableValueRange<float>(1f, 2f)))},
                    { StatusEffectData.Modifier.RunStaminaDrain , AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Run Stamina Drain", 0.5f, new ConfigDescription("Set amount of run stamina drain modified, multiplied by prestige level, 0.5 = 50%, 1 = 100%", new AcceptableValueRange<float>(0f, 1f)))}
                },
                m_animation = "challenge",
                m_duration = AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Rogue_Speed",
                    m_name = "$talent_rogue_speed",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_rogue_talent_1",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Stamina Cost", 10, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.Knives,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Rogue - Quick Step", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new ()
            {
                m_key = "RogueStamina",
                m_name = "$talent_rogue_stamina",
                m_sprite = SpriteManager.Relentless_Icon,
                m_description = "$talent_rogue_stamina_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Stamina , AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Stamina", 30f, new ConfigDescription("Set amount of increased stamina, multiplied by prestige level", new AcceptableValueRange<float>(0f, 101f)))},
                    { StatusEffectData.Modifier.StaminaRegen , AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Stamina Regen", 1.1f, new ConfigDescription("Set stamina regeneration amount, multiplied by prestige level, 1.1 = 110%, 1.5 = 150%, 2 = 200%", new AcceptableValueRange<float>(1f, 2f)))}
                },
                m_animation = "challenge",
                m_duration = AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Rogue_Stamina",
                    m_name = "$talent_rogue_stamina",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_rogue_talent_4",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Stamina Cost", 10, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Rogue - Relentless", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new ()
            {
                m_key = "RogueReflect",
                m_name = "$talent_reflect",
                m_sprite = SpriteManager.Reflect_Icon,
                m_description = "$talent_reflect_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Reflect , AlmanacClassesPlugin._Plugin.config("Rogue - Retaliation", "Reflect", 0.1f, new ConfigDescription("Set value of reflected damage, multiplied by prestige level, 0.1 = 10%, 0.5 = 50%, 1 = 100%", new AcceptableValueRange<float>(0f, 1f)))}
                },
                m_animation = "challenge",
                m_duration = AlmanacClassesPlugin._Plugin.config("Rogue - Retaliation", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Rogue_Reflect",
                    m_name = "$talent_reflect",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_rogue_talent_2",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Rogue - Retaliation", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Rogue - Retaliation", "Stamina Cost", 10, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Rogue - Retaliation", "Cooldown", 75f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Rogue - Retaliation", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new ()
            {
                m_key = "RogueDamage",
                m_name = "$talent_rogue_damage",
                m_sprite = SpriteManager.Backstab_Icon,
                m_description = "$talent_rogue_damage_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_animation = "challenge",
                m_chance = AlmanacClassesPlugin._Plugin.config("Rogue - Backstabbing", "Chance", 20f, new ConfigDescription("Set the chance to backstab on hit", new AcceptableValueRange<float>(0, 100f))),
                m_duration = AlmanacClassesPlugin._Plugin.config("Rogue - Backstabbing", "Duration", 20f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Rogue_Reflect",
                    m_name = "$talent_rogue_damage",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_rogue_talent_3",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Rogue - Backstabbing", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_staminaCost = AlmanacClassesPlugin._Plugin.config("Rogue - Backstabbing", "Stamina Cost", 10, new ConfigDescription("Set stamina cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Rogue - Backstabbing", "Cooldown", 20f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.Knives,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Rogue - Backstabbing", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new ()
            {
                m_key = "RogueBleed",
                m_name = "$talent_bleed",
                m_sprite = SpriteManager.Bleeding_Icon,
                m_description = "$talent_bleed_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_rogue_talent_5",
                m_cost = 5,
                m_talentDamages = new()
                {
                    pierce = AlmanacClassesPlugin._Plugin.config("Rogue - Bleeding", "Pierce Damage", 1f, new ConfigDescription("Set bleeding pierce damage, multiplied by prestige level", new AcceptableValueRange<float>(0f, 10f)))
                },
                m_skill = Skills.SkillType.BloodMagic
            },
            new()
            {
                m_key = "Rogue1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_rogue_1"
            },
            new()
            {
                m_key = "Rogue2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_rogue_2"
            },
            new()
            {
                m_key = "Rogue3",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_rogue_3"
            },
            new()
            {
                m_key = "Rogue4",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_rogue_4"
            },
            new()
            {
                m_key = "Rogue5",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_rogue_5"
            },
            new()
            {
                m_key = "Rogue6",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_rogue_6"
            },
        };
        InitTalents(rogueTalents);
    }

    private static void LoadWarriorTalents(int level)
    {
        List<Talent> warriorTalents = new()
        {
            new ()
            {
                m_key = "WarriorStrength",
                m_name = "$talent_warrior_power",
                m_sprite = SpriteManager.HardHitter_Icon,
                m_description = "$talent_warrior_power_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Attack , AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Attack Modifier", 1.1f, new ConfigDescription("Set amount of increased damage output, multiplied by prestige level, 1.1 = 110% 1.5 = 150%, 2 = 200%", new AcceptableValueRange<float>(1f, 2f)))},
                    { StatusEffectData.Modifier.HealthRegen , AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Health Regen", 1.1f,  new ConfigDescription("Set amount of health regeneration increase, multiplied by prestige level", new AcceptableValueRange<float>(1f, 2f)))}
                },
                m_animation = "flex",
                m_duration = AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Warrior_Strength",
                    m_name = "$talent_warrior_power",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_warrior_talent_1",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_healthCost = AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Health Cost", 10, new ConfigDescription("Set health cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.Swords,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Warrior - Hard Hitter", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false)
            },
            new ()
            {
                m_key = "WarriorVitality",
                m_name = "$talent_warrior_vitality",
                m_sprite = SpriteManager.BulkUp_Icon,
                m_description = "$talent_warrior_vitality_desc",
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_modifiers = new()
                {
                    { StatusEffectData.Modifier.Vitality , AlmanacClassesPlugin._Plugin.config("Warrior - Bulk Up", "Vitality", 30f, new ConfigDescription("Set amount of increased vitality, multiplied by prestige level", new AcceptableValueRange<float>(0f, 101f)))}
                },
                m_animation = "flex",
                m_duration = AlmanacClassesPlugin._Plugin.config("Warrior - Bulk Up", "Duration", 50f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Warrior_Vitality",
                    m_name = "$talent_warrior_vitality",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_warrior_talent_2",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Warrior - Bulk Up", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_healthCost = AlmanacClassesPlugin._Plugin.config("Warrior - Bulk Up", "Health Cost", 10, new ConfigDescription("Set health cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Warrior - Bulk Up", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Warrior - Bulk Up", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false)
            },
            new ()
            {
                m_key = "MonkeyWrench",
                m_name = "$talent_two_hand",
                m_description = "$talent_two_hand_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_warrior_talent_4",
            },
            new ()
            {
                m_key = "WarriorResistance",
                m_name = "$talent_resist",
                m_description = "$talent_resist_desc",
                m_sprite = SpriteManager.Resistant_Icon,
                m_level = level,
                m_type = TalentType.StatusEffect,
                
                m_resistances = new()
                {
                    { HitData.DamageType.Blunt , AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "1. Blunt Resistance", HitData.DamageModifier.Resistant, "Set resistance to blunt")},
                    { HitData.DamageType.Slash , AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "2. Slash Resistance", HitData.DamageModifier.Resistant, "Set resistance to slash")},
                    { HitData.DamageType.Pierce , AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "3. Pierce Resistance", HitData.DamageModifier.Resistant, "Set resistance to pierce")}
                },
                m_animation = "flex",
                m_duration = AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "Duration", 10f, new ConfigDescription("Set duration of effect", new AcceptableValueRange<float>(1f, 101f))),
                
                m_statusEffect = new()
                {
                    name = "SE_Warrior_Resistance",
                    m_name = "$talent_resist",
                    m_startEffects = LoadedAssets.FX_DvergerPower,
                },
                m_buttonName = "$button_warrior_talent_3",
                m_eitrCost = AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "Eitr Cost", 0, new ConfigDescription("Set eitr cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_healthCost = AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "Health Cost", 15, new ConfigDescription("Set health cost to activate ability", new AcceptableValueRange<int>(0, 101))),
                m_isAbility = true,
                m_ttl = AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "Cooldown", 50f, new ConfigDescription("Set cooldown", new AcceptableValueRange<float>(0, 101))),
                m_skill = Skills.SkillType.BloodMagic,
                m_triggerStartEffects = AlmanacClassesPlugin._Plugin.config("Warrior - Resistance", "Visual Effects", AlmanacClassesPlugin.Toggle.On, "If on, activating ability triggers visual effects", false),
            },
            new()
            {
                m_key = "DualWield",
                m_name = "$talent_dual",
                m_description = "$talent_dual_desc",
                m_level = level,
                m_type = TalentType.Passive,
                m_buttonName = "$button_warrior_talent_5",
                m_cost = 5,
            },
            new()
            {
                m_key = "Warrior1",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_warrior_1"
            },
            new()
            {
                m_key = "Warrior2",
                m_name = "$almanac_wisdom",
                m_description = $"+ {10 * level} $almanac_wisdom",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Wisdom,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_warrior_2"
            },
            new()
            {
                m_key = "Warrior3",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_warrior_3"
            },
            new()
            {
                m_key = "Warrior4",
                m_name = "$almanac_dexterity",
                m_description = $"+ {10 * level} $almanac_dexterity",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Dexterity,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_warrior_4"
            },
            new()
            {
                m_key = "Warrior5",
                m_name = "$almanac_strength",
                m_description = $"+ {10 * level} $almanac_strength",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Strength,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_warrior_5"
            },
            new()
            {
                m_key = "Warrior6",
                m_name = "$almanac_constitution",
                m_description = $"+ {10 * level} $almanac_constitution",
                m_level = level,
                m_type = TalentType.Characteristic,
                m_characteristic = Characteristic.Constitution,
                m_characteristicValue = 10 * level,
                m_buttonName = "$button_warrior_6"
            },
        };
        InitTalents(warriorTalents);
    }

    private static void InitTalents(List<Talent> talents)
    {
        foreach (Talent talent in talents)
        {
            talent.InitTalent();
        }
    }

    public static HitData.DamageTypes GetDamages(Talent talent)
    {
        HitData.DamageTypes damages = new HitData.DamageTypes();
        if (talent.m_talentDamages == null) return damages;
        TalentDamages? talentDamages = talent.m_talentDamages;
        damages.m_blunt = talentDamages.blunt?.Value ?? 0f;
        damages.m_pierce = talentDamages.pierce?.Value ?? 0f;
        damages.m_slash = talentDamages.slash?.Value ?? 0f;
        damages.m_chop = talentDamages.chop?.Value ?? 0f;
        damages.m_pickaxe = talentDamages.pickaxe?.Value ?? 0f;
        damages.m_fire = talentDamages.fire?.Value ?? 0f;
        damages.m_frost = talentDamages.frost?.Value ?? 0f;
        damages.m_lightning = talentDamages.lightning?.Value ?? 0f;
        damages.m_poison = talentDamages.poison?.Value ?? 0f;
        damages.m_spirit = talentDamages.spirit?.Value ?? 0f;
        damages.Modify(talent.m_level);
        return damages;
    }
}