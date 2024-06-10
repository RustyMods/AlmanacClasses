﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.Data;
using AlmanacClasses.FileSystem;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx.Configuration;
using UnityEngine;
using static AlmanacClasses.AlmanacClassesPlugin;

namespace AlmanacClasses.Classes;

public class Talent
{
    private readonly string m_prestigeColor = "#FF5733";
    public string m_key = "";
    public string m_button = "";
    public int m_statusEffectHash;
    public string m_ability = "";
    public string m_animation = "";
    public int m_level = 1;
    public int GetLevel() => m_level;
    public void AddLevel() => ++m_level;
    
    public ConfigEntry<Toggle>? m_alt;
    public void SetLevel(int level) => m_level = level;
    public TalentType m_type;
    public string GetTalentType() => $"$almanac_{m_type.ToString().ToLower()}";
    public EffectList? m_startEffects;
    public EffectList GetEffectList() => m_startEffects ?? new EffectList();
    public Sprite? m_sprite;
    public Sprite? GetSprite() => m_sprite;
    
    public ConfigEntry<int>? m_cost;
    public int GetCost() => m_cost?.Value ?? _StatsCost.Value;
    public ConfigEntry<float>? m_cooldown;
    public float GetCooldown() => m_cooldown?.Value ?? 0f;
    public ConfigEntry<float>? m_length;
    public float GetLength() => m_length?.Value ?? 0f;
    public ConfigEntry<float>? m_eitrCost;
    public float GetEitrCost() => m_eitrCost?.Value ?? 0f;
    public ConfigEntry<float>? m_staminaCost;
    public float GetStaminaCost() => m_staminaCost?.Value ?? 0f;
    public ConfigEntry<float>? m_healthCost;
    public float GetHealthCost() => m_healthCost?.Value ?? 0f;

    public TalentDamages? m_damages;
    public HitData.DamageTypes GetDamages(int level)
    {
        HitData.DamageTypes damages = new HitData.DamageTypes();
        if (m_damages == null) return damages;
        damages.m_blunt = m_damages.m_blunt?.Value ?? 0f;
        damages.m_pierce = m_damages.m_pierce?.Value ?? 0f;
        damages.m_slash = m_damages.m_slash?.Value ?? 0f;
        damages.m_chop = m_damages.m_chop?.Value ?? 0f;
        damages.m_pickaxe = m_damages.m_pickaxe?.Value ?? 0f;
        damages.m_fire = m_damages.m_fire?.Value ?? 0f;
        damages.m_frost = m_damages.m_frost?.Value ?? 0f;
        damages.m_lightning = m_damages.m_lightning?.Value ?? 0f;
        damages.m_poison = m_damages.m_poison?.Value ?? 0f;
        damages.m_spirit = m_damages.m_spirit?.Value ?? 0f;
        damages.Modify(level);
        return damages;
    }

    public TalentValues? m_values;

    public float GetHealAmount(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_heal?.Value ?? 0f) * level;
    }

    public float GetHealth(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_health?.Value ?? 0f) * level;
    }

    public float GetStamina(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_stamina?.Value ?? 0f) * level;
    }

    public float GetEitr(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_eitr?.Value ?? 0f) * level;
    }

    public float GetAbsorb(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_absorb?.Value ?? 0f) * level;
    }

    public float GetEitrRegen(int level)
    {
        if (m_values == null) return 1f;
        return (m_values.m_eitrRegen?.Value ?? 1f) * level - (level - 1);
    }

    public float GetHealthRegen(int level)
    {
        if (m_values == null) return 1f;
        return (m_values.m_healthRegen?.Value ?? 1f) * level - (level - 1);
    }

    public float GetStaminaRegen(int level)
    {
        if (m_values == null) return 1f;
        return (m_values.m_staminaRegen?.Value ?? 1f) * level - (level - 1);
    }

    public int GetCarryWeight(int level)
    {
        if (m_values == null) return 0;
        return (m_values.m_carryWeight?.Value ?? 0) * level;
    }

    public float GetAttack(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_modifyAttack?.Value ?? 0f) * level - (level - 1);
    }

    public float GetBleed(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_bleed?.Value ?? 0f) * level;
    }

    public float GetSpeedModifier(int level)
    {
        if (m_values == null) return 0;
        return (m_values.m_speed?.Value ?? 0f) * level - (level - 1);
    }

    public float GetChance(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_chance?.Value ?? 0f) * level;
    }

    public float GetReflect(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_reflect?.Value ?? 0f) * level ;
    }

    public float GetAddedComfort(int level)
    {
        if (m_values == null) return 0f;
        return (m_values.m_comfort?.Value ?? 0f) * level;
    }

    public float GetDamageReduction(int level)
    {
        if (m_values == null) return 1f;
        return Mathf.Clamp01(1 - ((m_values.m_damageReduction?.Value ?? 1f) - 0.1f * (level - 1)));
    }

    public float GetFoodModifier(int level)
    {
        if (m_values == null) return 1f;
        return (m_values.m_foodModifier?.Value ?? 1f) * level - (level - 1);
    }

    public float GetForageModifier(int level)
    {
        if (m_values == null) return 1f;
        return (m_values.m_forageModifier?.Value ?? 1f) * level - (level - 1);
    }

    public float GetSpeedReduction(int level)
    {
        if (m_values == null) return 1f;
        return Mathf.Clamp01(1 - ((m_values.m_speedReduction?.Value ?? 1f) + 0.1f * (level - 1)));
    }

    public float GetRunStaminaDrain(int level)
    {
        if (m_values == null) return 1f;
        return Mathf.Clamp(1 - (m_values.m_runStaminaDrain?.Value ?? 0f) * level, 0f, 1f);
    }
    
    public TalentResistances? m_resistances;

    public HitData.DamageModifiers GetDamageModifiers()
    {
        if (m_resistances == null) return new HitData.DamageModifiers();
        return new HitData.DamageModifiers
        {
            m_blunt = m_resistances.m_blunt?.Value ?? HitData.DamageModifier.Normal,
            m_pierce = m_resistances.m_pierce?.Value ?? HitData.DamageModifier.Normal,
            m_slash = m_resistances.m_slash?.Value ?? HitData.DamageModifier.Normal,
        };
    }
    public TalentCreatures? m_creatures;
    public GameObject? GetCreatures(Heightmap.Biome biome)
    {
        if (m_creatures == null) return null;
        var scene = ZNetScene.instance;
        if (!scene) return null;
        switch (biome)
        {
            case Heightmap.Biome.Meadows:
                return scene.GetPrefab(m_creatures.m_meadows?.Value ?? "Neck") ?? scene.GetPrefab("Neck");
            case Heightmap.Biome.BlackForest:
                return scene.GetPrefab(m_creatures.m_blackforest?.Value ?? "Greydwarf") ?? scene.GetPrefab("Greydwarf");
            case Heightmap.Biome.Swamp:
                return scene.GetPrefab(m_creatures.m_swamp?.Value ?? "Draugr") ?? scene.GetPrefab("Draugr");
            case Heightmap.Biome.Mountain:
                return scene.GetPrefab(m_creatures.m_mountains?.Value ?? "Ulv") ?? scene.GetPrefab("Ulv");
            case Heightmap.Biome.Plains:
                return scene.GetPrefab(m_creatures.m_plains?.Value ?? "Deathsquito") ?? scene.GetPrefab("Deathsquito");
            case Heightmap.Biome.Mistlands:
                return scene.GetPrefab(m_creatures.m_mistlands?.Value ?? "Seeker") ?? scene.GetPrefab("Seeker");
            case Heightmap.Biome.AshLands:
                return scene.GetPrefab(m_creatures.m_ashlands?.Value ?? "Surtling") ?? scene.GetPrefab("Surtling");
            case Heightmap.Biome.DeepNorth:
                return scene.GetPrefab(m_creatures.m_deepnorth?.Value ?? "Lox") ?? scene.GetPrefab("Lox");
            case Heightmap.Biome.Ocean:
                return scene.GetPrefab(m_creatures.m_ocean?.Value ?? "Serpent") ?? scene.GetPrefab("Serpent");
            default:
                return null;
        }
    }
    public ConfigEntry<string>? m_creature;
    public GameObject? GetCreature()
    {
        ZNetScene scene = ZNetScene.instance;
        if (!scene) return null;
        return scene.GetPrefab(m_creature?.Value ?? "Ghost") ?? scene.GetPrefab("Ghost");
    }

    public TalentCharacteristics? m_characteristic;
    public Characteristic GetCharacteristicType() => m_characteristic?.m_type ?? Characteristic.None;
    public int GetCharacteristic(int level) => (m_characteristic?.m_amount ?? 0) * level;
    public class TalentValues
    {
        public ConfigEntry<float>? m_heal;
        public ConfigEntry<float>? m_absorb;
        public ConfigEntry<float>? m_health;
        public ConfigEntry<float>? m_stamina;
        public ConfigEntry<float>? m_eitr;
        public ConfigEntry<float>? m_eitrRegen;
        public ConfigEntry<float>? m_healthRegen;
        public ConfigEntry<float>? m_staminaRegen;
        public ConfigEntry<int>? m_carryWeight;
        public ConfigEntry<float>? m_modifyAttack;
        public ConfigEntry<float>? m_speed;
        public ConfigEntry<float>? m_bleed;
        public ConfigEntry<float>? m_chance;
        public ConfigEntry<float>? m_reflect;
        public ConfigEntry<float>? m_comfort;
        public ConfigEntry<float>? m_damageReduction;
        public ConfigEntry<float>? m_foodModifier;
        public ConfigEntry<float>? m_forageModifier;
        public ConfigEntry<float>? m_speedReduction;
        public ConfigEntry<float>? m_runStaminaDrain;
    }
    public class TalentCreatures
    {
        public ConfigEntry<string>? m_meadows;
        public ConfigEntry<string>? m_blackforest;
        public ConfigEntry<string>? m_swamp;
        public ConfigEntry<string>? m_mountains;
        public ConfigEntry<string>? m_plains;
        public ConfigEntry<string>? m_mistlands;
        public ConfigEntry<string>? m_ashlands;
        public ConfigEntry<string>? m_deepnorth;
        public ConfigEntry<string>? m_ocean;
    }
    public class TalentResistances
    {
        public ConfigEntry<HitData.DamageModifier>? m_blunt;
        public ConfigEntry<HitData.DamageModifier>? m_slash;
        public ConfigEntry<HitData.DamageModifier>? m_pierce;
    }
    public class TalentDamages
    {
        public ConfigEntry<float>? m_blunt;
        public ConfigEntry<float>? m_pierce;
        public ConfigEntry<float>? m_slash;
        public ConfigEntry<float>? m_chop;
        public ConfigEntry<float>? m_pickaxe;
        public ConfigEntry<float>? m_fire;
        public ConfigEntry<float>? m_frost;
        public ConfigEntry<float>? m_lightning;
        public ConfigEntry<float>? m_poison;
        public ConfigEntry<float>? m_spirit;
    }
    public class TalentCharacteristics
    {
        public Characteristic m_type;
        public int m_amount;
    }

    public string GetName() => m_type is TalentType.Characteristic ? GetTalentType() : $"$talent_{m_key.ToLower()}";
    
    public string GetDescription() => $"$talent_{m_key.ToLower()}_desc";

    private float GetCreatureDespawnLength(int level)
    {
        if (m_creature == null) return 0f;
        return GetLength() * level;
    }

    private float GetCreaturesDespawnLength(int level)
    {
        if (m_creatures == null) return 0f;
        return GetLength() * level;
    }
    
    public string GetTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (m_type is TalentType.Characteristic)
        {
            if (m_characteristic == null) return stringBuilder.ToString();
            string key = DefaultData.LocalizeCharacteristics[GetCharacteristicType()];
            stringBuilder.Append($"${GetCharacteristicType().ToString().ToLower()}_desc\n");
            stringBuilder.Append($"+ <color=orange>{GetCharacteristic(GetLevel())} {key}</color>\n\n");
            switch (m_characteristic.m_type)
            {
                case Characteristic.Constitution:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{(int)CharacteristicManager.GetHealthRatio()}</color> $se_health\n");
                    break;
                case Characteristic.Strength:
                    stringBuilder.Append($"$se_max_carryweight: <color=orange>+ {GetCharacteristic(GetLevel())}</color>\n");
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{FormatPercentage(CharacteristicManager.GetStrengthModifier())}%</color> $almanac_physical\n");
                    break;
                case Characteristic.Intelligence:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{FormatPercentage(CharacteristicManager.GetIntelligenceModifier())}%</color> $almanac_elemental\n");
                    break;
                case Characteristic.Dexterity:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{(int)CharacteristicManager.GetStaminaRatio()}</color> $se_stamina\n");
                    stringBuilder.Append(
                        $"$almanac_attackspeedmod: <color=orange>+{FormatPercentage(CharacteristicManager.GetDexterityModifier())}%</color>");
                    break;
                case Characteristic.Wisdom:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{(int)CharacteristicManager.GetEitrRatio()}</color> $se_eitr");
                    break;
            }
        }
        else
        {
            stringBuilder.Append(GetDescription() + "\n\n");
            if (m_cooldown != null)
            {
                stringBuilder.Append($"$almanac_cooldown: <color=orange>{GetCooldown()}</color>sec\n");
            }

            if (m_length != null)
            {
                stringBuilder.Append($"$almanac_duration: <color=orange>{GetLength()}</color>sec\n");
            }

            if (m_eitrCost is { Value: > 0 }) stringBuilder.Append($"$se_eitr $almanac_cost: <color=orange>{GetEitrCost()}</color>\n");
            if (m_staminaCost is { Value: > 0 }) stringBuilder.Append($"$se_stamina $almanac_cost: <color=orange>{GetStaminaCost()}</color>\n");
            if (m_healthCost is { Value: > 0 }) stringBuilder.Append($"$se_health $almanac_cost: <color=orange>{GetHealthCost()}</color>\n");
            if (m_values != null)
            {
                if (m_values.m_absorb is { Value: > 0 }) stringBuilder.Append($"$almanac_damage_absorb: <color=orange>{GetAbsorb(GetLevel())}</color>\n");
                if (m_values.m_chance is { Value: > 0 }) stringBuilder.Append($"$almanac_chance: <color=orange>{GetChance(GetLevel())}%</color>\n");
                if (m_values.m_reflect is { Value: > 0 }) stringBuilder.Append($"$almanac_reflect: <color=orange>{FormatPercentage(GetReflect(GetLevel()) + 1f)}%</color>\n");
                if (m_values.m_bleed is { Value: > 0 }) stringBuilder.Append($"$almanac_bleed: <color=orange>{GetBleed(GetLevel())}/tick</color>\n");
                if (m_values.m_eitr is {Value: > 0}) stringBuilder.Append($"$se_eitr: <color=orange>{GetEitr(GetLevel())}</color>\n");
                if (m_values.m_heal is {Value: > 0}) stringBuilder.Append($"$almanac_heal: <color=orange>{GetHealAmount(GetLevel())}</color>\n");
                if (m_values.m_health is {Value: > 0}) stringBuilder.Append($"$se_health: <color=orange>{GetHealth(GetLevel())}</color>\n");
                if (m_values.m_stamina is {Value: > 0}) stringBuilder.Append($"$se_stamina: <color=orange>{GetStamina(GetLevel())}</color>\n");
                if (m_values.m_speed is { Value: > 0}) stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedModifier(GetLevel()))}%</color>\n");
                if (m_values.m_carryWeight is { Value: > 0}) stringBuilder.Append($"$se_max_carryweight: <color=orange>{GetCarryWeight(GetLevel())}</color>\n");
                if (m_values.m_eitrRegen is {Value: > 0}) stringBuilder.Append($"$se_eitrregen: <color=orange>{FormatPercentage(GetEitrRegen(GetLevel()))}%</color>\n");
                if (m_values.m_healthRegen is {Value: > 0}) stringBuilder.Append($"$se_healthregen: <color=orange>{FormatPercentage(GetHealthRegen(GetLevel()))}%</color>\n");
                if (m_values.m_staminaRegen is {Value: > 0}) stringBuilder.Append($"$se_staminaregen: <color=orange>{FormatPercentage(GetStaminaRegen(GetLevel()))}%</color>\n");
                if (m_values.m_modifyAttack is {Value: > 0}) stringBuilder.Append($"$almanac_attack: <color=orange>{FormatPercentage(GetAttack(GetLevel()))}%</color>\n");
                if (m_values.m_comfort is { Value: > 0 }) stringBuilder.Append($"$se_rested_comfort: <color=orange>{GetAddedComfort(GetLevel())}</color>\n");
                if (m_values.m_damageReduction is { Value: > 0 }) stringBuilder.Append($"$almanac_damage_reduction: <color=orange>{FormatPercentage(GetDamageReduction(GetLevel()))}%</color>\n");
                if (m_values.m_foodModifier is { Value: > 0 }) stringBuilder.Append($"$almanac_foodmod: <color=orange>{FormatPercentage(GetFoodModifier(GetLevel()))}%</color>\n");
                if (m_values.m_forageModifier is { Value: > 0 }) stringBuilder.Append($"$almanac_foragemod: <color=orange>{FormatPercentage(GetForageModifier(GetLevel()))}%</color>\n");
                if (m_values.m_speedReduction is { Value: > 0 }) stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedReduction(GetLevel()))}%</color>\n");
                if (m_values.m_runStaminaDrain is {Value: > 0}) stringBuilder.Append($"$se_runstamina: <color=orange>{FormatPercentage(GetRunStaminaDrain(GetLevel()))}%</color>\n");
            }

            if (m_damages != null)
            {
                HitData.DamageTypes damages = GetDamages(GetLevel());
                if (m_damages.m_blunt is {Value: > 0}) stringBuilder.Append($"$inventory_blunt: <color=orange>{damages.m_blunt}</color>\n");
                if (m_damages.m_pierce is {Value: > 0}) stringBuilder.Append($"$inventory_pierce: <color=orange>{damages.m_pierce}</color>\n");
                if (m_damages.m_slash is {Value: > 0}) stringBuilder.Append($"$inventory_slash: <color=orange>{damages.m_slash}</color>\n");
                if (m_damages.m_pickaxe is {Value: > 0}) stringBuilder.Append($"$inventory_pickaxe: <color=orange>{damages.m_pickaxe}</color>\n");
                if (m_damages.m_chop is {Value: > 0}) stringBuilder.Append($"$inventory_chop: <color=orange>{damages.m_chop}</color>\n");
                if (m_damages.m_fire is {Value: > 0}) stringBuilder.Append($"$inventory_fire: <color=orange>{damages.m_fire}</color>\n");
                if (m_damages.m_frost is {Value: > 0}) stringBuilder.Append($"$inventory_frost: <color=orange>{damages.m_frost}</color>\n");
                if (m_damages.m_lightning is {Value: > 0}) stringBuilder.Append($"$inventory_lightning: <color=orange>{damages.m_lightning}</color>\n");
                if (m_damages.m_poison is {Value: > 0}) stringBuilder.Append($"$inventory_poison: <color=orange>{damages.m_poison}</color>\n");
                if (m_damages.m_spirit is {Value: > 0}) stringBuilder.Append($"$inventory_spirit: <color=orange>{damages.m_spirit}</color>\n");
            }

            if (m_resistances != null)
            {
                if (m_resistances.m_pierce is { Value: > 0 }) stringBuilder.Append($"<color=orange>{GetDamageModifiers().m_pierce}</color> VS <color=orange>$inventory_pierce</color>\n");
                if (m_resistances.m_slash is { Value: > 0 }) stringBuilder.Append($"<color=orange>{GetDamageModifiers().m_slash}</color> VS <color=orange>$inventory_slash</color>\n");
                if (m_resistances.m_blunt is { Value: > 0 }) stringBuilder.Append($"<color=orange>{GetDamageModifiers().m_blunt}</color> VS <color=orange>$inventory_blunt</color>\n");
            }

            if (m_creature != null) stringBuilder.Append($"$almanac_creature: <color=orange>{m_creature.Value}</color> ({GetCreatureDespawnLength(GetLevel())}sec)\n");
            if (m_creatures != null)
            {
                if (Player.m_localPlayer)
                {
                    Heightmap.Biome currentBiome = Player.m_localPlayer.GetCurrentBiome();
                    ConfigEntry<string>? config = null!;
                    switch (currentBiome)
                    {
                        case Heightmap.Biome.Meadows:
                            config = m_creatures.m_meadows;
                            break;
                        case Heightmap.Biome.BlackForest:
                            config = m_creatures.m_blackforest;
                            break;
                        case Heightmap.Biome.Swamp:
                            config = m_creatures.m_swamp;
                            break;
                        case Heightmap.Biome.Mountain:
                            config = m_creatures.m_mountains;
                            break;
                        case Heightmap.Biome.Plains:
                            config = m_creatures.m_plains;
                            break;
                        case Heightmap.Biome.Mistlands:
                            config = m_creatures.m_mistlands;
                            break;
                        case Heightmap.Biome.AshLands:
                            config = m_creatures.m_ashlands;
                            break;
                        case Heightmap.Biome.DeepNorth:
                            config = m_creatures.m_deepnorth;
                            break;
                        case Heightmap.Biome.Ocean:
                            config = m_creatures.m_ocean;
                            break;
                    }

                    stringBuilder.Append($"{GetBiomeLocalized(currentBiome)} $almanac_creature: <color=orange>{config?.Value ?? ""}</color> ({GetCreaturesDespawnLength(GetLevel())}sec)\n");
                }
            }

            if (m_key == "Trader")
            {
                stringBuilder.Append($"$almanac_allows_to_tp <color=orange>{m_level}</color>\n");
            }

            if (m_key == "AirBender")
            {
                stringBuilder.Append($"$almanac_jumps: <color=orange>{m_level}</color>\n");
            }
        }
        return Localization.instance.Localize(stringBuilder.ToString());
    }

    private string GetBiomeLocalized(Heightmap.Biome biome) => $"$biome_{biome.ToString().ToLower()}";

    private int FormatPercentage(float value) => (int)(value * 100 - 100);

    public string GetPrestigeTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (m_type is TalentType.Characteristic)
        {
            if (m_characteristic == null) return stringBuilder.ToString();
            string key = DefaultData.LocalizeCharacteristics[GetCharacteristicType()];
            stringBuilder.Append($"${GetCharacteristicType().ToString().ToLower()}_desc\n");
            stringBuilder.Append($"+ <color=orange>{GetCharacteristic(GetLevel())}</color> --> <color={m_prestigeColor}>{GetCharacteristic(GetLevel() + 1)}</color> {key}\n\n");
            switch (m_characteristic.m_type)
            {
                case Characteristic.Constitution:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{(int)CharacteristicManager.GetHealthRatio()}</color> $se_health\n");
                    break;
                case Characteristic.Strength:
                    stringBuilder.Append($"$se_max_carryweight: <color=orange>+ {GetCharacteristic(GetLevel())}</color> --> <color={m_prestigeColor}>+ {GetCharacteristic(GetLevel() + 1)}</color>\n");
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{FormatPercentage(CharacteristicManager.GetStrengthModifier())}%</color> $almanac_physical\n");
                    break;
                case Characteristic.Intelligence:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{FormatPercentage(CharacteristicManager.GetIntelligenceModifier())}%</color> $almanac_elemental\n");
                    break;
                case Characteristic.Dexterity:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{(int)CharacteristicManager.GetStaminaRatio()}</color> $se_stamina\n");
                    stringBuilder.Append(
                        $"$almanac_attackspeedmod: <color=orange>+{FormatPercentage(CharacteristicManager.GetDexterityModifier())}%</color>");
                    break;
                case Characteristic.Wisdom:
                    stringBuilder.Append(
                        $"$almanac_current: <color=orange>+{(int)CharacteristicManager.GetEitrRatio()}</color> $se_eitr");
                    break;
            }
        }
        else
        {
            stringBuilder.Append(GetDescription() + "\n\n");
            if (m_cooldown != null)
            {
                stringBuilder.Append($"$almanac_cooldown: <color=orange>{GetCooldown()}</color>sec\n");
            }

            if (m_length != null)
            {
                stringBuilder.Append($"$almanac_duration: <color=orange>{GetLength()}</color>sec\n");
            }

            if (m_eitrCost is { Value: > 0 }) stringBuilder.Append($"$se_eitr $almanac_cost: <color=orange>{GetEitrCost()}</color>\n");
            if (m_staminaCost is { Value: > 0 }) stringBuilder.Append($"$se_stamina $almanac_cost: <color=orange>{GetStaminaCost()}</color>\n");
            if (m_healthCost is { Value: > 0 }) stringBuilder.Append($"$se_health $almanac_cost: <color=orange>{GetHealthCost()}</color>\n");
            if (m_values != null)
            {
                if (m_values.m_absorb is { Value: > 0 }) stringBuilder.Append($"$almanac_damage_absorb: <color=orange>{GetAbsorb(GetLevel())}</color> --> <color={m_prestigeColor}>{GetAbsorb(GetLevel() + 1)}</color>\n");
                if (m_values.m_chance is { Value: > 0 }) stringBuilder.Append($"$almanac_chance: <color=orange>{GetChance(GetLevel())}%</color> --> <color={m_prestigeColor}>{GetChance(GetLevel() + 1)}%</color>\n");
                if (m_values.m_reflect is { Value: > 0 }) stringBuilder.Append($"$almanac_reflect: <color=orange>{FormatPercentage(GetReflect(GetLevel()) + 1f)}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetReflect(GetLevel() + 1) + 1f)}%</color>\n");
                if (m_values.m_bleed is { Value: > 0 }) stringBuilder.Append($"$almanac_bleed: <color=orange>{GetBleed(GetLevel())}/tick</color> --> <color={m_prestigeColor}>{GetBleed(GetLevel() + 1)}/tick</color>\n");
                if (m_values.m_eitr is {Value: > 0}) stringBuilder.Append($"$se_eitr: <color=orange>{GetEitr(GetLevel())}</color> --> <color={m_prestigeColor}>{GetEitr(GetLevel() + 1)}</color>\n");
                if (m_values.m_heal is {Value: > 0}) stringBuilder.Append($"$almanac_heal: <color=orange>{GetHealAmount(GetLevel())}</color> --> <color={m_prestigeColor}>{GetHealAmount(GetLevel() + 1)}</color>\n");
                if (m_values.m_health is {Value: > 0}) stringBuilder.Append($"$se_health: <color=orange>{GetHealth(GetLevel())}</color> --> <color={m_prestigeColor}>{GetHealth(GetLevel() + 1)}</color>\n");
                if (m_values.m_stamina is {Value: > 0}) stringBuilder.Append($"$se_stamina: <color=orange>{GetStamina(GetLevel())}</color> --> <color={m_prestigeColor}>{GetStamina(GetLevel() + 1)}</color>\n");
                if (m_values.m_speed is { Value: > 0}) stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedModifier(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetSpeedModifier(GetLevel() + 1))}%</color>\n");
                if (m_values.m_carryWeight is { Value: > 0}) stringBuilder.Append($"$se_max_carryweight: <color=orange>{GetCarryWeight(GetLevel())}</color> --> <color={m_prestigeColor}>{GetCarryWeight(GetLevel() + 1)}</color>\n");
                if (m_values.m_eitrRegen is {Value: > 0}) stringBuilder.Append($"$se_eitrregen: <color=orange>{FormatPercentage(GetEitrRegen(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetEitrRegen(GetLevel() + 1))}%</color>\n");
                if (m_values.m_healthRegen is {Value: > 0}) stringBuilder.Append($"$se_healthregen: <color=orange>{FormatPercentage(GetHealthRegen(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetHealthRegen(GetLevel() + 1))}%</color>\n");
                if (m_values.m_staminaRegen is {Value: > 0}) stringBuilder.Append($"$se_staminaregen: <color=orange>{FormatPercentage(GetStaminaRegen(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetStaminaRegen(GetLevel() + 1))}%</color>\n");
                if (m_values.m_modifyAttack is {Value: > 0}) stringBuilder.Append($"$almanac_attack: <color=orange>{FormatPercentage(GetAttack(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetAttack(GetLevel() + 1))}%</color>\n");
                if (m_values.m_comfort is { Value: > 0 }) stringBuilder.Append($"$se_rested_comfort: <color=orange>{GetAddedComfort(GetLevel())}</color> --> <color={m_prestigeColor}>{GetAddedComfort(GetLevel() + 1)}</color>\n");
                if (m_values.m_damageReduction is { Value: > 0 }) stringBuilder.Append($"$almanac_damage_reduction: <color=orange>{FormatPercentage(GetDamageReduction(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetDamageReduction(GetLevel() + 1))}%</color>\n");
                if (m_values.m_foodModifier is { Value: > 0 }) stringBuilder.Append($"$almanac_foodmod: <color=orange>{FormatPercentage(GetFoodModifier(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetFoodModifier(GetLevel() + 1))}%</color>\n");
                if (m_values.m_forageModifier is { Value: > 0 }) stringBuilder.Append($"$almanac_foragemod: <color=orange>{FormatPercentage(GetForageModifier(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetForageModifier(GetLevel() + 1))}%</color>\n");
                if (m_values.m_speedReduction is { Value: > 0 }) stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedReduction(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetSpeedReduction(GetLevel() + 1))}%</color>\n");
                if (m_values.m_runStaminaDrain is {Value: > 0}) stringBuilder.Append($"$se_runstamina: <color=orange>{FormatPercentage(GetRunStaminaDrain(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetRunStaminaDrain(GetLevel() + 1))}%</color>\n");
            }

            if (m_damages != null)
            {
                HitData.DamageTypes damages = GetDamages(GetLevel());
                HitData.DamageTypes newDamages = GetDamages(GetLevel() + 1);
                if (m_damages.m_blunt is {Value: > 0}) stringBuilder.Append($"$inventory_blunt: <color=orange>{damages.m_blunt}</color> --> <color={m_prestigeColor}>{newDamages.m_blunt}</color>\n");
                if (m_damages.m_pierce is {Value: > 0}) stringBuilder.Append($"$inventory_pierce: <color=orange>{damages.m_pierce}</color> --> <color={m_prestigeColor}>{newDamages.m_pierce}</color>\n");
                if (m_damages.m_slash is {Value: > 0}) stringBuilder.Append($"$inventory_slash: <color=orange>{damages.m_slash}</color> --> <color={m_prestigeColor}>{newDamages.m_slash}</color>\n");
                if (m_damages.m_pickaxe is {Value: > 0}) stringBuilder.Append($"$inventory_pickaxe: <color=orange>{damages.m_pickaxe}</color> --> <color={m_prestigeColor}>{newDamages.m_pickaxe}</color>\n");
                if (m_damages.m_chop is {Value: > 0}) stringBuilder.Append($"$inventory_chop: <color=orange>{damages.m_chop}</color> --> <color={m_prestigeColor}>{newDamages.m_chop}</color>\n");
                if (m_damages.m_fire is {Value: > 0}) stringBuilder.Append($"$inventory_fire: <color=orange>{damages.m_fire}</color> --> <color={m_prestigeColor}>{newDamages.m_fire}</color>\n");
                if (m_damages.m_frost is {Value: > 0}) stringBuilder.Append($"$inventory_frost: <color=orange>{damages.m_frost}</color> --> <color={m_prestigeColor}>{newDamages.m_frost}</color>\n");
                if (m_damages.m_lightning is {Value: > 0}) stringBuilder.Append($"$inventory_lightning: <color=orange>{damages.m_lightning}</color> --> <color={m_prestigeColor}>{newDamages.m_lightning}</color>\n");
                if (m_damages.m_poison is {Value: > 0}) stringBuilder.Append($"$inventory_poison: <color=orange>{damages.m_poison}</color> --> <color={m_prestigeColor}>{newDamages.m_poison}</color>\n");
                if (m_damages.m_spirit is {Value: > 0}) stringBuilder.Append($"$inventory_spirit: <color=orange>{damages.m_spirit}</color> --> <color={m_prestigeColor}>{newDamages.m_spirit}</color>\n");
            }

            if (m_resistances != null)
            {
                if (m_resistances.m_pierce is { Value: > 0 }) stringBuilder.Append($"<color=orange>{GetDamageModifiers().m_pierce}</color> VS <color=orange>$inventory_pierce</color>\n");
                if (m_resistances.m_slash is { Value: > 0 }) stringBuilder.Append($"<color=orange>{GetDamageModifiers().m_slash}</color> VS <color=orange>$inventory_slash</color>\n");
                if (m_resistances.m_blunt is { Value: > 0 }) stringBuilder.Append($"<color=orange>{GetDamageModifiers().m_blunt}</color> VS <color=orange>$inventory_blunt</color>\n");
            }

            if (m_creature != null) stringBuilder.Append($"$almanac_creature: <color=orange>{m_creature.Value}</color> ({GetCreatureDespawnLength(GetLevel())}sec --> <color={m_prestigeColor}>{GetCreatureDespawnLength(GetLevel() + 1)}sec</color>)\n");
            if (m_creatures != null)
            {
                if (Player.m_localPlayer)
                {
                    Heightmap.Biome currentBiome = Player.m_localPlayer.GetCurrentBiome();
                    ConfigEntry<string>? config = null!;
                    switch (currentBiome)
                    {
                        case Heightmap.Biome.Meadows:
                            config = m_creatures.m_meadows;
                            break;
                        case Heightmap.Biome.BlackForest:
                            config = m_creatures.m_blackforest;
                            break;
                        case Heightmap.Biome.Swamp:
                            config = m_creatures.m_swamp;
                            break;
                        case Heightmap.Biome.Mountain:
                            config = m_creatures.m_mountains;
                            break;
                        case Heightmap.Biome.Plains:
                            config = m_creatures.m_plains;
                            break;
                        case Heightmap.Biome.Mistlands:
                            config = m_creatures.m_mistlands;
                            break;
                        case Heightmap.Biome.AshLands:
                            config = m_creatures.m_ashlands;
                            break;
                        case Heightmap.Biome.DeepNorth:
                            config = m_creatures.m_deepnorth;
                            break;
                        case Heightmap.Biome.Ocean:
                            config = m_creatures.m_ocean;
                            break;
                    }

                    stringBuilder.Append($"{GetBiomeLocalized(currentBiome)} $almanac_creature: <color=orange>{config?.Value ?? ""}</color> ({GetCreaturesDespawnLength(GetLevel())}sec --> <color={m_prestigeColor}>{GetCreaturesDespawnLength(GetLevel() + 1)}sec</color>)\n");
                }
            }

            if (m_key == "Trader")
            {
                stringBuilder.Append($"$almanac_allows_to_tp <color=orange>{GetLevel()}</color> --> <color={m_prestigeColor}>{GetLevel() + 1}</color>\n");
            }
            
            if (m_key == "AirBender")
            {
                stringBuilder.Append($"$almanac_jumps: <color=orange>{m_level}</color> --> <color={m_prestigeColor}>{GetLevel() + 1}</color>\n");
            }
        }
        return Localization.instance.Localize(stringBuilder.ToString());
    }
}

public enum TalentType { Ability, Passive, StatusEffect, Characteristic }

public static class TalentManager
{
    public static readonly Dictionary<string, Talent> m_talents = new();
    public static readonly Dictionary<string, Talent> m_talentsByButton = new();
    public static readonly Dictionary<string, Talent> m_altTalentsByButton = new();
    public static readonly Dictionary<int, Talent> m_talentsByStatusEffect = new();

    public static void ResetTalentLevels()
    {
        foreach (var kvp in m_talents)
        {
            kvp.Value.SetLevel(1);
        }
    }

    public static void InitializeTalents()
    {
        FilePaths.CreateFolders();
        AlmanacClassesLogger.LogDebug("Initializing talents");
        m_talents.Clear();
        m_talentsByButton.Clear();
        LoadTalents();
        LoadAltTalents();
    }

    private static void LoadTalents()
    {
        List<Talent> talents = new();
        talents.AddRange(LoadCore());
        talents.AddRange(LoadRanger());
        talents.AddRange(LoadSage());
        talents.AddRange(LoadShaman());
        talents.AddRange(LoadBard());
        talents.AddRange(LoadRogue());
        talents.AddRange(LoadWarrior());

        foreach (Talent talent in talents)
        {
            m_talents[talent.m_key] = talent;
            m_talentsByButton[talent.m_button] = talent;
            if (talent.m_statusEffectHash != 0)
            {
                m_talentsByStatusEffect[talent.m_statusEffectHash] = talent;
            }
        }
    }

    private static void LoadAltTalents()
    {
        List<Talent> talents = new();
        talents.AddRange(LoadAltWarrior());

        foreach (Talent talent in talents)
        {
            m_talents[talent.m_key] = talent;
            m_altTalentsByButton[talent.m_button] = talent;
            if (talent.m_statusEffectHash != 0)
            {
                m_talentsByStatusEffect[talent.m_statusEffectHash] = talent;
            }
        }
        
    }

    private static List<Talent> LoadAltWarrior()
    {
        Talent survivor = new Talent()
        {
            m_key = "Survivor",
            m_button = "$button_warrior_talent_5",
            m_type = TalentType.Passive,
            m_statusEffectHash = "SE_Survivor".GetStableHashCode(),
            m_values = new Talent.TalentValues()
            {
                m_chance = _Plugin.config("Warrior - Survivor", "Chance", 20f,
                    new ConfigDescription("Set the chance to not die and regain health",
                        new AcceptableValueRange<float>(0f, 100f))),
            },
            m_cost = _Plugin.config("Warrior - Survivor", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock ability", new AcceptableValueRange<int>(1, 10))),
            m_alt = _Plugin.config("Warrior - Survivor", "Enable", Toggle.Off, "If on, replaces dual wield talent"),
            m_startEffects = LoadedAssets.VFX_SongOfSpirit
        };
        survivor.m_alt.SettingChanged += (sender, args) =>
        {
            LoadUI.ChangeButton(survivor, survivor.m_alt.Value is Toggle.Off);
        };
        Talent battleFury = new Talent()
        {
            m_key = "BattleFury",
            m_button = "$button_warrior_talent_4",
            m_type = TalentType.Passive,
            m_statusEffectHash = "$SE_BattleFury".GetStableHashCode(),
            m_cost = _Plugin.config("Warrior - Battle Fury", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock talent", new AcceptableValueRange<int>(1, 10))),
            m_alt = _Plugin.config("Warrior - Battle Fury", "Enable", Toggle.Off, "If on, replaces monkey wrench talent"),
            m_startEffects = LoadedAssets.FX_BattleFury
        };
        battleFury.m_alt.SettingChanged += (sender, args) =>
        {
            LoadUI.ChangeButton(battleFury, battleFury.m_alt.Value is Toggle.Off);
        };

        return new() { survivor, battleFury };
    }

    #region Data
    private static List<Talent> LoadCore()
    {
        return new List<Talent>
        {
            new()
            {
                m_key = "Core1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10
                },
                m_button = "$button_core_1"
            },
            new()
            {
                m_key = "Core2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10
                },
                m_button = "$button_core_2"
            },
            new()
            {
                m_key = "Core3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10
                },
                m_button = "$button_core_3"
            },
            new()
            {
                m_key = "Core4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10
                },
                m_button = "$button_core_4"
            },
            new()
            {
                m_key = "Core5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10
                },
                m_button = "$button_core_5"
            },
            new()
            {
                m_key = "Core6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10
                },
                m_button = "$button_core_6"
            },
            new()
            {
                m_key = "Core7",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10
                },
                m_button = "$button_core_7"
            },
            new()
            {
                m_key = "Core8",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10
                },
                m_button = "$button_core_8"
            },
            new()
            {
                m_key = "Core9",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10
                },
                m_button = "$button_core_9"
            },
            new()
            {
                m_key = "Core10",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10
                },
                m_button = "$button_core_10"
            },
            new()
            {
                m_key = "Core11",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10
                },
                m_button = "$button_core_11"
            },
            new()
            {
                m_key = "Core12",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10
                },
                m_button = "$button_core_12"
            },
            new()
            {
                m_key = "Wise",
                m_button = "$button_sneak",
                m_type = TalentType.Passive,
                m_statusEffectHash = "SE_Enlightened".GetStableHashCode(),
                m_cost = _Plugin.config("Core - Wise", "Purchase Cost", 5, new ConfigDescription("Set the cost to purchase talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_eitr = _Plugin.config("Core - Wise", "Eitr", 15f, new ConfigDescription("Set the amount of eitr", new AcceptableValueRange<float>(0f, 101f)))
                }
            },
            new()
            {
                m_key = "AirBender",
                m_button = "$button_lumberjack",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Air Bender", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_eitrCost = _Plugin.config("Core - Air Bender", "Eitr Cost", 10f, new ConfigDescription("Set the eitr cost to jump in the air", new AcceptableValueRange<float>(0f, 1000f)))
            },
            new()
            {
                m_key = "Comfort",
                m_button = "$button_comfort_2",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Relax", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_comfort = _Plugin.config("Core - Relax", "Comfort", 1f, new ConfigDescription("Set comfort amount", new AcceptableValueRange<float>(0f, 10f)))
                },
            },
            new()
            {
                m_key = "Resourceful",
                m_button = "$button_comfort_1",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Resourceful", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_statusEffectHash = "SE_Resourceful".GetStableHashCode(),
                m_damages = new Talent.TalentDamages()
                {
                    m_chop = _Plugin.config("Core - Resourceful", "Chop Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 101f))),
                    m_pickaxe = _Plugin.config("Core - Resourceful", "Pickaxe Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 101f)))
                }
            },
            new()
            {
                m_key = "MasterChef",
                m_button = "$button_chef",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Chef", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_foodModifier = _Plugin.config("Core - Chef", "Modifier", 1.1f, new ConfigDescription("Set the modifier", new AcceptableValueRange<float>(1f, 2f)))
                },
            },
            new()
            {
                m_key = "PackMule",
                m_button = "$button_shield",
                m_statusEffectHash = "SE_PackMule".GetStableHashCode(),
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Pack Mule", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_carryWeight = _Plugin.config("Core - Pack Mule", "Carry Weight", 30, new ConfigDescription("Set increase carry weight", new AcceptableValueRange<int>(0, 1000)))
                }
            },
            new()
            {
                m_key = "RainProof",
                m_button = "$button_rain",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Rain Proof", "Purchase Cost", 10, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)))
            },
            new()
            {
                m_key = "DoubleLoot",
                m_button = "$button_merchant",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Double Loot", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_chance = _Plugin.config("Core - Double Loot", "Chance", 10f, new ConfigDescription("Set the chance to get loot", new AcceptableValueRange<float>(0f, 100f)))
                }
            },
            new()
            {
                m_key = "Trader",
                m_button = "$button_sail",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Trader", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
            },
            new()
            {
                m_key = "Forager",
                m_button = "$button_treasure",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Core - Forager", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_forageModifier = _Plugin.config("Core - Forager", "Modifier", 1.25f, new ConfigDescription("Set the multiplier of foraged amount", new AcceptableValueRange<float>(0f, 10f)))
                },
            }
        };
    }

    private static List<Talent> LoadRanger()
    {
        return new List<Talent>
        {
            new()
            {
                m_key = "Ranger1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_ranger_1"
            },
            new()
            {
                m_key = "Ranger2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_ranger_2"
            },
            new()
            {
                m_key = "Ranger3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_ranger_3"
            },
            new()
            {
                m_key = "Ranger4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_ranger_4"
            },
            new()
            {
                m_key = "Ranger5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10,
                },
                m_button = "$button_ranger_5"
            },
            new()
            {
                m_key = "Ranger6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10,
                },
                m_button = "$button_ranger_6"
            },
            new()
            {
                m_key = "RangerTamer",
                m_button = "$button_ranger_talent_3",
                m_ability = "TriggerHunterSpawn",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Ranger - Summon", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Ranger - Summon", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_creatures = new Talent.TalentCreatures()
                {
                    m_meadows = _Plugin.config("Ranger - Summon", "Meadows", "Neck", "Set creature summon"),
                    m_blackforest = _Plugin.config("Ranger - Summon", "Black Forest", "Greydwarf", "Set creature summon"),
                    m_swamp = _Plugin.config("Ranger - Summon", "Swamp", "Draugr", "Set creature summon"),
                    m_mountains = _Plugin.config("Ranger - Summon", "Mountains", "Ulv", "Set creature summon"),
                    m_plains = _Plugin.config("Ranger - Summon", "Plains", "Deathsquito", "Set creature summon"),
                    m_mistlands = _Plugin.config("Ranger - Summon", "Mistlands", "Seeker", "Set creature summon"),
                    m_ashlands = _Plugin.config("Ranger - Summon", "Ashlands", "Surtling", "Set creature summon"),
                    m_deepnorth = _Plugin.config("Ranger - Summon", "Deep North", "Lox", "Set creature summon"),
                    m_ocean = _Plugin.config("Ranger - Summon", "Ocean", "Serpent", "Set creature summon"),
                },
                m_length = _Plugin.config("Ranger - Summon", "Length", 100f, new ConfigDescription("Set the amount of time until de-spawn", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.CreatureMask_Icon,
                m_animation = "Summon",
                m_healthCost = _Plugin.config("Ranger - Summon", "Health Cost", 10f, new ConfigDescription("Set health cost to summon", new AcceptableValueRange<float>(0f, 100f))),
                m_staminaCost = _Plugin.config("Ranger - Summon", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Ranger - Summon", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RangerHunter",
                m_button = "$button_ranger_talent_1",
                m_statusEffectHash = "SE_Hunter".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Ranger - Hunter", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Ranger - Hunter", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_speedReduction = _Plugin.config("Ranger - Hunter", "Speed Reduction", 0.1f, new ConfigDescription("Set the speed of creatures nearby", new AcceptableValueRange<float>(0f, 1f)))
                },
                m_length = _Plugin.config("Ranger - Hunter", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.DeerHunter_Icon,
                m_startEffects = LoadedAssets.SoothEffects,
                m_healthCost = _Plugin.config("Ranger - Hunter", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Ranger - Hunter", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Ranger - Hunter", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "LuckyShot",
                m_button = "$button_ranger_talent_2",
                m_type = TalentType.Passive,
                m_cooldown = _Plugin.config("Ranger - Lucky Shot", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Ranger - Lucky Shot", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_chance = _Plugin.config("Ranger - Lucky Shot", "Chance", 10f, new ConfigDescription("Set the chance to not consume projectile", new AcceptableValueRange<float>(0f, 100f)))
                },
                m_startEffects = LoadedAssets.SoothEffects
            },
            new()
            {
                m_key = "QuickShot",
                m_button = "$button_ranger_talent_5",
                m_statusEffectHash = "SE_QuickShot".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Ranger - Quick Shot", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Ranger - Quick Shot", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_speed = _Plugin.config("Ranger - Quick Shot", "Draw Speed Modifier", 1.05f, new ConfigDescription("Set the draw speed multiplier", new AcceptableValueRange<float>(1f, 2f)))
                },
                m_length = _Plugin.config("Ranger - Quick Shot", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(0f, 1000f))),
                m_sprite = SpriteManager.QuickShot_Icon,
                m_startEffects = LoadedAssets.SoothEffects,
                m_animation = "roar",
                m_healthCost = _Plugin.config("Ranger - Quick Shot", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Ranger - Quick Shot", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Ranger - Quick Shot", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RangerTrap",
                m_button = "$button_ranger_talent_4",
                m_ability = "TriggerSpawnTrap",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Ranger - Trap", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Ranger - Trap", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_pierce = _Plugin.config("Ranger - Trap", "Blunt Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_poison = _Plugin.config("Ranger - Trap", "Poison Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_length = _Plugin.config("Ranger - Trap", "Length", 60f, new ConfigDescription("Set the length until de-spawn", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.RangerTrap_Icon,
                m_animation = "SetTrap",
                m_healthCost = _Plugin.config("Ranger - Trap", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Ranger - Trap", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Ranger - Trap", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            }
        };
    }

    private static List<Talent> LoadSage()
    {
        return new List<Talent>
        {
            new()
            {
                m_key = "Sage1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_sage_1"
            },
            new()
            {
                m_key = "Sage2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_sage_2"
            },
            new()
            {
                m_key = "Sage3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10,
                },
                m_button = "$button_sage_3"
            },
            new()
            {
                m_key = "Sage4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10,
                },
                m_button = "$button_sage_4"
            },
            new()
            {
                m_key = "Sage5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10,
                },
                m_button = "$button_sage_5"
            },
            new()
            {
                m_key = "Sage6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10,
                },
                m_button = "$button_sage_6"
            },
            new()
            {
                m_key = "Sage1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_sage_1"
            },
            new()
            {
                m_key = "CallOfLightning",
                m_button = "$button_sage_talent_4",
                m_ability = "TriggerLightningAOE",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Sage - Lightning", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Sage - Lightning", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_pierce = _Plugin.config("Sage - Lightning", "Pierce Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_lightning = _Plugin.config("Sage - Lightning", "Lightning Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_sprite = SpriteManager.LightningStrike_Icon,
                m_animation = "LightningStrike",
                m_healthCost = _Plugin.config("Sage - Lightning", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Sage - Lightning", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Sage - Lightning", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "MeteorStrike",
                m_button = "$button_sage_talent_3",
                m_ability = "TriggerMeteor",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Sage - Meteor", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Sage - Meteor", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_blunt = _Plugin.config("Sage - Meteor", "Blunt Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_fire = _Plugin.config("Sage - Meteor", "Fire Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                },
                m_sprite = SpriteManager.MeteorStrike_Icon,
                m_animation = "MeteorStrike",
                m_healthCost = _Plugin.config("Sage - Meteor", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Sage - Meteor", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Sage - Meteor", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "StoneThrow",
                m_button = "$button_sage_talent_1",
                m_ability = "TriggerStoneThrow",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Sage - Stone", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Sage - Stone", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_blunt = _Plugin.config("Sage - Stone", "Blunt Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                },
                m_sprite = SpriteManager.BoulderStrike_Icon,
                m_animation = "StoneThrow",
                m_healthCost = _Plugin.config("Sage - Stone", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Sage - Stone", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Sage - Stone", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "GoblinBeam",
                m_button = "$button_sage_talent_2",
                m_ability = "TriggerGoblinBeam",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Sage - Beam", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Sage - Beam", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_blunt = _Plugin.config("Sage - Beam", "Blunt Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_fire = _Plugin.config("Sage - Beam", "Fire Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_spirit = _Plugin.config("Sage - Beam", "Spirit Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_sprite = SpriteManager.GoblinBeam_Icon,
                m_animation = "roar",
                m_healthCost = _Plugin.config("Sage - Beam", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Sage - Beam", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Sage - Beam", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "IceBreath",
                m_button = "$button_sage_talent_5",
                m_ability = "TriggerIceBreath",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Sage - Ice Breath", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Sage - Ice Breath", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_slash = _Plugin.config("Sage - Ice Breath", "Slash Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_frost = _Plugin.config("Sage - Ice Breath", "Frost Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                },
                m_sprite = SpriteManager.Blink_Icon,
                m_animation = "roar",
                m_healthCost = _Plugin.config("Sage - Ice Breath", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Sage - Ice Breath", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Sage - Ice Breath", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            }
        };
    }

    private static List<Talent> LoadShaman()
    {
        return new()
        {
            new()
            {
                m_key = "Shaman1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_shaman_1"
            },
            new()
            {
                m_key = "Shaman2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_shaman_2"
            },
            new()
            {
                m_key = "Shaman3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10,
                },
                m_button = "$button_shaman_3"
            },
            new()
            {
                m_key = "Shaman4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10,
                },
                m_button = "$button_shaman_4"
            },
            new()
            {
                m_key = "Shaman5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_shaman_5"
            },
            new()
            {
                m_key = "Shaman6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_shaman_6"
            },
            new()
            {
                m_key = "ShamanHeal",
                m_button = "$button_shaman_talent_1",
                m_ability = "TriggerHeal",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Shaman - Heal", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Shaman - Heal", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_heal = _Plugin.config("Shaman - Heal", "Heal", 25f, new ConfigDescription("Set heal", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_sprite = SpriteManager.ShamanHeal_Icon,
                m_animation = "Heal",
                m_healthCost = _Plugin.config("Shaman - Heal", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Shaman - Heal", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Shaman - Heal", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "ShamanShield",
                m_button = "$button_shaman_talent_5",
                m_statusEffectHash = "SE_ShamanShield".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Shaman - Shield", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Shaman - Shield", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_absorb = _Plugin.config("Shaman - Shield", "Absorb", 50f, new ConfigDescription("Set amount absorbed my shield", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_length = _Plugin.config("Shaman - Shield", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.ShamanProtection_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "LightningStrike",
                m_healthCost = _Plugin.config("Shaman - Shield", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Shaman - Shield", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Shaman - Shield", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "ShamanRegeneration",
                m_button = "$button_shaman_talent_4",
                m_statusEffectHash = "SE_ShamanRegeneration".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Shaman - Regeneration", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Shaman - Regeneration", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_eitrRegen = _Plugin.config("Shaman - Regeneration", "Eitr Regeneration", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(0f, 10f)))
                },
                m_length = _Plugin.config("Shaman - Regeneration", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.ShamanRegeneration,
                m_startEffects = LoadedAssets.UnSummonEffects,
                m_animation = "cheer",
                m_healthCost = _Plugin.config("Shaman - Regeneration", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Shaman - Regeneration", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Shaman - Regeneration", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "ShamanSpawn",
                m_button = "$button_shaman_talent_3",
                m_ability = "TriggerShamanSpawn",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Shaman - Summon", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Shaman - Summon", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_creature = _Plugin.config("Shaman - Summon", "Creature Name", "Ghost", "Set the creature to summon"),
                m_length = _Plugin.config("Shaman - Summon", "Length", 100f, new ConfigDescription("Set the length of time until de-spawn", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.ShamanGhosts_Icon,
                m_animation = "Summon",
                m_healthCost = _Plugin.config("Shaman - Summon", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Shaman - Summon", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Shaman - Summon", "Eitr Cost", 25f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RootBeam",
                m_button = "$button_shaman_talent_2",
                m_ability = "TriggerRootBeam",
                m_type = TalentType.Ability,
                m_cooldown = _Plugin.config("Shaman - Roots", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Shaman - Roots", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_blunt =  _Plugin.config("Shaman - Roots", "Blunt Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
                    m_poison = _Plugin.config("Shaman - Roots", "Poison Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_sprite = SpriteManager.ShamanRoots_Icon,
                m_animation = "roar",
                m_healthCost = _Plugin.config("Shaman - Roots", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Shaman - Roots", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Shaman - Roots", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            }
        };
    }

    private static List<Talent> LoadBard()
    {
        return new List<Talent>
        {
            new()
            {
                m_key = "Bard1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_bard_1"
            },
            new()
            {
                m_key = "Bard2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10,
                },
                m_button = "$button_bard_2"
            },
            new()
            {
                m_key = "Bard3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Intelligence,
                    m_amount = 10,
                },
                m_button = "$button_bard_3"
            },
            new()
            {
                m_key = "Bard4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_bard_4"
            },
            new()
            {
                m_key = "Bard5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_bard_5"
            },
            new()
            {
                m_key = "Bard6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_bard_6"
            },
            new()
            {
                m_key = "SongOfDamage",
                m_button = "$button_bard_talent_3",
                m_statusEffectHash = "SE_SongOfDamage".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Bard - Song of Damage", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Bard - Song of Damage", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_modifyAttack = _Plugin.config("Bard - Song of Damage", "Attack Increase", 1.05f, new ConfigDescription("Set the attack multiplier", new AcceptableValueRange<float>(0f, 10f)))
                },
                m_length = _Plugin.config("Bard - Song of Damage", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.SongOfDamage_Icon,
                m_startEffects = LoadedAssets.AddBardFX(Color.red, "FX_Bard_Music_Red", true),
                m_animation = "LutePlay",
                m_healthCost = _Plugin.config("Bard - Song of Damage", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Bard - Song of Damage", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Bard - Song of Damage", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "SongOfHealing",
                m_button = "$button_bard_talent_4",
                m_statusEffectHash = "SE_SongOfHealing".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Bard - Song of Healing", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Bard - Song of Healing", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_heal = _Plugin.config("Bard - Song of Healing", "Heal", 5f, new ConfigDescription("Set the amount healed per tick", new AcceptableValueRange<float>(0f, 101f)))
                },
                m_length = _Plugin.config("Bard - Song of Healing", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.SongOfHealing_Icon,
                m_startEffects = LoadedAssets.AddBardFX(Color.yellow, "FX_Bard_Music_Yellow"),
                m_animation = "LutePlay",
                m_healthCost = _Plugin.config("Bard - Song of Healing", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Bard - Song of Healing", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Bard - Song of Healing", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "SongOfVitality",
                m_button = "$button_bard_talent_2",
                m_statusEffectHash = "SE_SongOfVitality".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Bard - Song of Vitality", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Bard - Song of Vitality", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_health = _Plugin.config("Bard - Song of Vitality", "Health", 10f, new ConfigDescription("Set the health gained from effect", new AcceptableValueRange<float>(0f, 100f)))
                },
                m_length = _Plugin.config("Bard - Song of Vitality", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.SongOfVitality_Icon,
                m_startEffects = LoadedAssets.AddBardFX(Color.blue, "FX_Bard_Music_Blue"),
                m_animation = "LutePlay",
                m_healthCost = _Plugin.config("Bard - Song of Vitality", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Bard - Song of Vitality", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Bard - Song of Vitality", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "SongOfSpeed",
                m_button = "$button_bard_talent_1",
                m_statusEffectHash = "SE_SongOfSpeed".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Bard - Song of Speed", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Bard - Song of Speed", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_speed = _Plugin.config("Bard - Song of Speed", "Speed", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(1f, 2f)))
                },
                m_length = _Plugin.config("Bard - Song of Speed", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.SongOfSpeed_Icon,
                m_startEffects = LoadedAssets.AddBardFX(Color.green, "FX_Bard_Music_Green"),
                m_animation = "LutePlay",
                m_healthCost = _Plugin.config("Bard - Song of Speed", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Bard - Song of Speed", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Bard - Song of Speed", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "SongOfAttrition",
                m_button = "$button_bard_talent_5",
                m_statusEffectHash = "SE_SongOfAttrition".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Bard - Song of Attrition", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Bard - song of Attrition", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_damages = new Talent.TalentDamages()
                {
                    m_spirit = _Plugin.config("Bard - Song of Attrition", "Spirit Damage", 10f, new ConfigDescription("Set the damages", new AcceptableValueRange<float>(0f, 1000f)))
                },
                m_length = _Plugin.config("Bard - Song of Attrition", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.SongOfSpirit_Icon,
                m_startEffects = LoadedAssets.VFX_SongOfSpirit,
                m_animation = "LutePlay",
                m_healthCost = _Plugin.config("Bard - Song of Attrition", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Bard - Song of Attrition", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Bard - Song of Attrition", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            }
        };
    }

    private static List<Talent> LoadRogue()
    {
        return new List<Talent>
        {
            new()
            {
                m_key = "Rogue1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Wisdom,
                    m_amount = 10,
                },
                m_button = "$button_rogue_1"
            },
            new()
            {
                m_key = "Rogue2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_rogue_2"
            },
            new()
            {
                m_key = "Rogue3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10,
                },
                m_button = "$button_rogue_3"
            },
            new()
            {
                m_key = "Rogue4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10,
                },
                m_button = "$button_rogue_4"
            },
            new()
            {
                m_key = "Rogue5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_rogue_5"
            },
            new()
            {
                m_key = "Rogue6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_rogue_6"
            },
            new()
            {
                m_key = "RogueSpeed",
                m_button = "$button_rogue_talent_1",
                m_statusEffectHash = "SE_RogueSpeed".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Rogue - Quick Step", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Rogue - Quick Step", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_speed = _Plugin.config("Rogue - Quick Step", "Speed", 1.05f, new ConfigDescription("Set the speed multiplier", new AcceptableValueRange<float>(1f, 2f))),
                    m_runStaminaDrain = _Plugin.config("Rogue - Quick Step", "Run Stamina Drain", 0.1f, new ConfigDescription("Set the drain modifier", new AcceptableValueRange<float>(0f, 1f))),
                },
                m_length = _Plugin.config("Rogue - Quick Step", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.QuickStep_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Rogue - Quick Step", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Rogue - Quick Step", "Stamina Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Rogue - Quick Step", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RogueStamina",
                m_button = "$button_rogue_talent_4",
                m_statusEffectHash = "SE_RogueStamina".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Rogue - Swift", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Rogue - Quick Hands", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_staminaRegen = _Plugin.config("Rogue - Swift", "Stamina Regeneration", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(1f, 2f))),
                    m_stamina = _Plugin.config("Rogue - Swift", "Stamina", 10f, new ConfigDescription("Set the amount", new AcceptableValueRange<float>(0f, 100f)))
                },
                m_length = _Plugin.config("Rogue - Swift", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.Relentless_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Rogue - Swift", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Rogue - Swift", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Rogue - Swift", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RogueReflect",
                m_button = "$button_rogue_talent_2",
                m_statusEffectHash = "SE_RogueReflect".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Rogue - Retaliation", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Rogue - Retaliation", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_reflect = _Plugin.config("Rogue - Retaliation", "Reflect", 0.1f, new ConfigDescription("Set the amount reflected back", new AcceptableValueRange<float>(0f, 1f)))
                },
                m_length = _Plugin.config("Rogue - Retaliation", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.Reflect_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Rogue - Retaliation", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Rogue - Retaliation", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Rogue - Retaliation", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RogueBackstab",
                m_button = "$button_rogue_talent_3",
                m_statusEffectHash = "SE_RogueBackstab".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Rogue - Backstab", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Rogue - Backstab", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values =  new Talent.TalentValues()
                {
                    m_chance = _Plugin.config("Rogue - Backstab", "Chance", 5f, new ConfigDescription("Set the chance to backstab", new AcceptableValueRange<float>(0f, 100f)))
                },
                m_length = _Plugin.config("Rogue - Backstab", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.Backstab_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Rogue - Backstab", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Rogue - Backstab", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Rogue - Backstab", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "RogueBleed",
                m_button = "$button_rogue_talent_5",
                m_statusEffectHash = "SE_RogueBleed".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Rogue - Bleed", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Rogue - Bleed", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_bleed = _Plugin.config("Rogue - Bleed", "Damage Per Tick", 1f, new ConfigDescription("Set the damage per tick, stackable", new AcceptableValueRange<float>(1f, 10f)))
                },
                m_length = _Plugin.config("Rogue - Bleed", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.Bleeding_Icon,
                m_startEffects = LoadedAssets.FX_RogueBleed,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Rogue - Bleed", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Rogue - Bleed", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Rogue - Bleed", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            }
        };
    }

    private static List<Talent> LoadWarrior()
    {
        return new List<Talent>
        {
            new()
            {
                m_key = "Warrior1",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10,
                },
                m_button = "$button_warrior_1"
            },
            new()
            {
                m_key = "Warrior2",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10,
                },
                m_button = "$button_warrior_2"
            },
            new()
            {
                m_key = "Warrior3",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Dexterity,
                    m_amount = 10,
                },
                m_button = "$button_warrior_3"
            },
            new()
            {
                m_key = "Warrior4",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10,
                },
                m_button = "$button_warrior_4"
            },
            new()
            {
                m_key = "Warrior5",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Strength,
                    m_amount = 10,
                },
                m_button = "$button_warrior_5"
            },
            new()
            {
                m_key = "Warrior6",
                m_type = TalentType.Characteristic,
                m_characteristic = new Talent.TalentCharacteristics()
                {
                    m_type = Characteristic.Constitution,
                    m_amount = 10,
                },
                m_button = "$button_warrior_6"
            },
            new()
            {
                m_key = "WarriorStrength",
                m_button = "$button_warrior_talent_1",
                m_statusEffectHash = "SE_WarriorStrength".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Warrior - Power", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Warrior - Power", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_modifyAttack = _Plugin.config("Warrior - Power", "Attack", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(1f, 10f)))
                },
                m_length = _Plugin.config("Warrior - Power", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.HardHitter_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Warrior - Power", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Warrior - Power", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Warrior - Power", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "WarriorVitality",
                m_button = "$button_warrior_talent_2",
                m_statusEffectHash = "SE_WarriorVitality".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Warrior - Vitality", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Warrior - Vitality", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_health = _Plugin.config("Warrior - Vitality", "Health", 10f, new ConfigDescription("Set the health increase", new AcceptableValueRange<float>(0f, 101f))),
                    m_healthRegen = _Plugin.config("Warrior - Vitality", "Health Regeneration", 1.1f, new ConfigDescription("Set health regeneration modifier", new AcceptableValueRange<float>(0f, 101f)))
                },
                m_length = _Plugin.config("Warrior - Vitality", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f))),
                m_sprite = SpriteManager.BulkUp_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Warrior - Vitality", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Warrior - Vitality", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Warrior - Vitality", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "MonkeyWrench",
                m_button = "$button_warrior_talent_4",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Warrior - Monkey Wrench", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_damageReduction = _Plugin.config("Warrior - Monkey Wrench", "Damage Decrease", 0.5f, new ConfigDescription("Set the damage reduction", new AcceptableValueRange<float>(0f, 1f)))
                },
            },
            new()
            {
                m_key = "WarriorResistance",
                m_button = "$button_warrior_talent_3",
                m_statusEffectHash = "SE_WarriorResistance".GetStableHashCode(),
                m_type = TalentType.StatusEffect,
                m_cooldown = _Plugin.config("Warrior - Fortification", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f))),
                m_cost = _Plugin.config("Warrior - Fortification", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_resistances = new Talent.TalentResistances()
                {
                    m_blunt = _Plugin.config("Warrior - Fortification", "Blunt Resistance", HitData.DamageModifier.Resistant, "Set resistance"),
                    m_slash = _Plugin.config("Warrior - Fortification", "Slash Resistance", HitData.DamageModifier.Resistant, "Set resistance"),
                    m_pierce = _Plugin.config("Warrior - Fortification", "Pierce Resistance", HitData.DamageModifier.Resistant, "Set resistance"),
                },
                m_length = _Plugin.config("Warrior - Fortification", "Effect Length", 30f, new ConfigDescription("Set the length of the talent", new AcceptableValueRange<float>(0f, 1000f))),
                m_sprite = SpriteManager.Resistant_Icon,
                m_startEffects = LoadedAssets.FX_DvergerPower,
                m_animation = "flex",
                m_healthCost = _Plugin.config("Warrior - Fortification", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_staminaCost = _Plugin.config("Warrior - Fortification", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
                m_eitrCost = _Plugin.config("Warrior - Fortification", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f))),
            },
            new()
            {
                m_key = "DualWield",
                m_button = "$button_warrior_talent_5",
                m_type = TalentType.Passive,
                m_cost = _Plugin.config("Warrior - Dual Wield", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10))),
                m_values = new Talent.TalentValues()
                {
                    m_damageReduction = _Plugin.config("Warrior - Dual Wield", "Damage Decrease", 0.5f, new ConfigDescription("Set the damage reduction", new AcceptableValueRange<float>(0f, 1f)))
                }
            }
        };
    }
    #endregion

    #region Methods

    private static int GetTalentPoints()
    {
        int level = PlayerManager.GetPlayerLevel(PlayerManager.GetExperience());
        int points = level * _TalentPointPerLevel.Value;
        int pointsPerTen = (level / 10) * _TalentPointsPerTenLevel.Value;
        return points + pointsPerTen;
    }
    
    public static int GetAvailableTalentPoints() => GetTalentPoints() - GetTotalBoughtTalentPoints();

    public static int GetTotalBoughtTalentPoints() => PlayerManager.m_playerTalents.Values.Select(x => x.GetCost() * x.m_level).Sum();
    

    #endregion
}