using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Classes.Abilities.Bard;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.Classes.Abilities.Ranger;
using AlmanacClasses.Classes.Abilities.Rogue;
using AlmanacClasses.Classes.Abilities.Sage;
using AlmanacClasses.Classes.Abilities.Shaman;
using AlmanacClasses.Classes.Abilities.Warrior;
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
    private const string m_prestigeColor = "#FF5733";
    public readonly string m_key;
    public readonly string m_button;
    public StatusEffect? m_status;
    public Func<bool>? m_ability;
    public string m_animation = "";
    public int m_level = 1;
    public ConfigEntry<Toggle>? m_alt;
    public ConfigEntry<int>? m_cap;
    public TalentType m_type;
    public ConfigEntry<Toggle>? m_effectsEnabled;
    public EffectList? m_startEffects;
    public Sprite? m_sprite;
    public ConfigEntry<int>? m_cost;
    public ConfigEntry<float>? m_cooldown;
    public ConfigEntry<float>? m_length;
    public ConfigEntry<float>? m_eitrCost;
    public ConfigEntry<float>? m_staminaCost;
    public ConfigEntry<float>? m_healthCost;
    public bool m_eitrCostReduces;
    public TalentDamages? m_damages;
    public TalentValues? m_values;
    public TalentCreatures? m_creatures;
    public CreaturesByLevel? m_creaturesByLevel;
    public ResistancePercentages? m_resistances;
    public Sprite? m_altButtonSprite;
    public ConfigEntry<string>? m_forageItems;
    public ConfigEntry<Toggle>? m_useAnimation;
    public bool m_passiveActive = true;
    public Func<bool>? m_onClickPassive;
    public bool m_addToPassiveBar;
    public string GetAnimation() => m_useAnimation?.Value is Toggle.On ? m_animation : "";
    public List<string> GetCustomForageItems()
    {
        List<string> output = new();
        if (m_forageItems == null) return output;
        output.AddRange(m_forageItems.Value.Split(':'));
        return output;
    }
    public int GetLevel() => m_level;
    public void AddLevel() => ++m_level;
    public int GetPrestigeCap() => m_cap?.Value ?? int.MaxValue;
    public void SetLevel(int level) => m_level = level;
    public string GetTalentType() => $"$almanac_{m_type.ToString().ToLower()}";
    public bool UseEffects() => m_effectsEnabled == null || m_effectsEnabled.Value is Toggle.On;
    public EffectList GetEffectList() => UseEffects() ?  m_startEffects ?? new EffectList() : new EffectList();
    public Sprite? GetSprite() => m_sprite;
    public int GetCost() => m_cost?.Value ?? _StatsCost.Value;
    public float GetCooldown(int level) => Mathf.Clamp((m_cooldown?.Value ?? 0f) - (level - 1) * 5f, GetLength(level), float.MaxValue);
    public float GetLength(int level) => (m_length?.Value ?? 0f) + (level - 1) * 5f;
    public float GetEitrCost(bool reduces, int level = 1) => reduces ? Mathf.Max((m_eitrCost?.Value ?? 0f) - level * 5f, 0) : m_eitrCost?.Value ?? 0f;
    public float GetStaminaCost() => m_staminaCost?.Value ?? 0f;
    public float GetHealthCost() => m_healthCost?.Value ?? 0f;
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
    public float GetHealAmount(int level) => (m_values == null) ? 0f : (m_values.m_heal?.Value ?? 0f) * level;
    public float GetHealth(int level) => m_values == null ? 0f : (m_values.m_health?.Value ?? 0f) * level;
    public float GetStamina(int level) => m_values == null ? 0f : (m_values.m_stamina?.Value ?? 0f) * level;
    public float GetEitr(int level) => m_values == null ? 0f : (m_values.m_eitr?.Value ?? 0f) * level;
    public float GetAbsorb(int level) => m_values == null ? 0f : (m_values.m_absorb?.Value ?? 0f) * level;
    public float GetEitrRegen(int level) => m_values == null ? 1f : (m_values.m_eitrRegen?.Value ?? 1f) * level - (level - 1);
    public float GetHealthRegen(int level) => m_values == null ? 1f : (m_values.m_healthRegen?.Value ?? 1f) * level - (level - 1);
    public float GetStaminaRegen(int level) => m_values == null ? 1f : (m_values.m_staminaRegen?.Value ?? 1f) * level - (level - 1);
    public int GetCarryWeight(int level) => m_values == null ? 0 : (m_values.m_carryWeight?.Value ?? 0) * level;
    public float GetAttack(int level) => m_values == null ? 0f : (m_values.m_modifyAttack?.Value ?? 0f) * level - (level - 1);
    public float GetBleed(int level) => m_values == null ? 0f : (m_values.m_bleed?.Value ?? 0f) * level;
    public float GetSpeedModifier(int level) => m_values == null ? 0f : (m_values.m_speed?.Value ?? 0f) * level - (level - 1);
    public float GetChance(int level) => m_values == null ? 0f : Mathf.Clamp((m_values.m_chance?.Value ?? 0f) + (level - 1) * 5f, 0f, 100f);
    public float GetReflect(int level) => m_values == null ? 0f : (m_values.m_reflect?.Value ?? 0f) * level;
    public float GetAddedComfort(int level) => m_values == null ? 0f : (m_values.m_comfort?.Value ?? 0f) * level;
    public float GetDamageReduction(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_damageReduction?.Value ?? 1f) - 0.1f * (level - 1)));
    public float GetFoodModifier(int level) => m_values == null ? 1f : (m_values.m_foodModifier?.Value ?? 1f) * level - (level - 1);
    public float GetForageModifier(int level) => m_values == null ? 1f : (m_values.m_forageModifier?.Value ?? 1f) * level - (level - 1);
    public float GetSpeedReduction(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_speedReduction?.Value ?? 1f) + 0.1f * (level - 1)));
    public float GetRunStaminaDrain(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - (m_values.m_runStaminaDrain?.Value ?? 0f) * level);
    public float GetAttackSpeedReduction(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_attackSpeedReduction?.Value ?? 1f) - 0.1f * (level - 1)));
    public float GetAttackStaminaUsage(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_attackStaminaUsage?.Value ?? 0f) + (level - 1) * 0.1f));
    public float GetSneakStaminaUsage(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_sneakStaminaUsage?.Value ?? 0f) + (level - 1) * 0.1f));
    public float GetResistance(int level, HitData.DamageType type)
    {
        if (m_resistances == null) return 1f;
        return Mathf.Clamp01(type switch
        {
            HitData.DamageType.Blunt => (m_resistances.m_blunt?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Slash => (m_resistances.m_slash?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Pierce => (m_resistances.m_pierce?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Fire => (m_resistances.m_fire?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Frost => (m_resistances.m_frost?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Lightning => (m_resistances.m_lightning?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Poison => (m_resistances.m_poison?.Value ?? 1f) - (level - 1) * 0.05f,
            HitData.DamageType.Spirit => (m_resistances.m_spirit?.Value ?? 1f) - (level - 1) * 0.05f,
            _ => 1f
        });
    }
    public GameObject? GetCreatures(Heightmap.Biome biome)
    {
        if (m_creatures == null) return null;
        ZNetScene scene = ZNetScene.instance;
        if (!scene) return null;
        var creature = biome switch
        {
            Heightmap.Biome.Meadows => scene.GetPrefab(m_creatures.m_meadows?.Value ?? "Neck") ?? scene.GetPrefab("Neck"),
            Heightmap.Biome.BlackForest => scene.GetPrefab(m_creatures.m_blackforest?.Value ?? "Greydwarf") ?? scene.GetPrefab("Greydwarf"),
            Heightmap.Biome.Swamp => scene.GetPrefab(m_creatures.m_swamp?.Value ?? "Draugr") ?? scene.GetPrefab("Draugr"),
            Heightmap.Biome.Mountain => scene.GetPrefab(m_creatures.m_mountains?.Value ?? "Ulv") ?? scene.GetPrefab("Ulv"),
            Heightmap.Biome.Plains => scene.GetPrefab(m_creatures.m_plains?.Value ?? "Deathsquito") ?? scene.GetPrefab("Deathsquito"),
            Heightmap.Biome.Mistlands => scene.GetPrefab(m_creatures.m_mistlands?.Value ?? "Seeker") ?? scene.GetPrefab("Seeker"),
            Heightmap.Biome.AshLands => scene.GetPrefab(m_creatures.m_ashlands?.Value ?? "Surtling") ?? scene.GetPrefab("Surtling"),
            Heightmap.Biome.DeepNorth => scene.GetPrefab(m_creatures.m_deepnorth?.Value ?? "Lox") ?? scene.GetPrefab("Lox"),
            Heightmap.Biome.Ocean => scene.GetPrefab(m_creatures.m_ocean?.Value ?? "Serpent") ?? scene.GetPrefab("Serpent"),
            _ => scene.GetPrefab("Neck")
        };
        return creature.TryGetComponent(out Humanoid _) ? creature : null;
    }
    public GameObject? GetCreaturesByLevel(int level)
    {
        if (m_creaturesByLevel == null) return null;
        ZNetScene scene = ZNetScene.instance;
        if (!scene) return null;
        bool defeatedBonemass = ZoneSystem.instance.CheckKey("defeated_bonemass", GameKeyType.Player);
        bool defeatedKing = ZoneSystem.instance.CheckKey("defeated_goblinking", GameKeyType.Player);
        bool defeatedQueen = ZoneSystem.instance.CheckKey("defeated_queen", GameKeyType.Player);
        return level switch
        {
            4 => defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            5 => defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            6 => defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            7 => defeatedKing ? scene.GetPrefab(m_creaturesByLevel.m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            8 => defeatedKing ? scene.GetPrefab(m_creaturesByLevel.m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            9 => defeatedKing ? scene.GetPrefab(m_creaturesByLevel.m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            >= 10 => defeatedQueen ? scene.GetPrefab(m_creaturesByLevel.m_ten?.Value ?? "FallenValkyrie") : defeatedKing ? scene.GetPrefab(m_creaturesByLevel.m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost"),
            _ => scene.GetPrefab(m_creaturesByLevel.m_oneToThree?.Value ?? "Ghost")
        };
    }
    private string GetCreatureName(GameObject? prefab) => prefab != null ? prefab.TryGetComponent(out Humanoid component) ? component.m_name : "Unknown" : "Invalid";

    public TalentCharacteristics? m_characteristic;
    public Characteristic GetCharacteristicType() => m_characteristic?.m_type ?? Characteristic.None;
    public int GetCharacteristic(int level) => (m_characteristic?.m_amount ?? 0) + (level - 1) * 5;
    public string GetName() => m_type is TalentType.Characteristic ? GetTalentType() : $"$talent_{m_key.ToLower()}";
    private string GetDescription() => $"$talent_{m_key.ToLower()}_desc";
    private float GetCreaturesLength(int level) => m_creatures == null ? 0f : GetLength(level);
    public float GetCreaturesByLevelLength(int level) => m_creaturesByLevel == null ? 0f : GetLength(level);
    public int GetCreatureByLevelLevel(int level) => level switch { 2 => 2, 3 => 3, 4 => 1, 5 => 2, 6 => 3, 7 => 1, 8 => 2, 9 => 3, _ => 1, };
    public float GetArmor(int level) => m_values == null ? 0f : (m_values.m_armor?.Value ?? 0f) + (level - 1) * 2f;
    private float GetHealthRatio(int level) => GetCharacteristic(level) / _HealthRatio.Value;
    private float GetCarryWeightRatio(int level) => GetCharacteristic(level) / _CarryWeightRatio.Value;
    private float GetEitrRatio(int level) => GetCharacteristic(level) / _EitrRatio.Value;
    private float GetStaminaRatio(int level) => GetCharacteristic(level) / _StaminaRatio.Value;
    private float GetStrengthModifier(int level) => 1 + GetCharacteristic(level) / _PhysicalRatio.Value / 100f;
    private float GetIntelligenceModifier(int level) => 1 + GetCharacteristic(level) / _ElementalRatio.Value / 100f;
    private float GetDexterityModifier(int level) => 1 + GetCharacteristic(level) / _SpeedRatio.Value / 100f;
    private string GetCharacteristicLocalized() => $"$almanac_{GetCharacteristicType().ToString().ToLower()}";
    private string GetCharacteristicDescription() => $"${GetCharacteristicType().ToString().ToLower()}_desc";
    public string GetTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (m_type is TalentType.Characteristic)
        {
            if (m_characteristic == null) return stringBuilder.ToString();
            stringBuilder.Append($"{GetCharacteristicDescription()}\n");
            stringBuilder.Append($"+ <color=orange>{GetCharacteristic(GetLevel())} {GetCharacteristicLocalized()}</color>\n\n");
            switch (m_characteristic.m_type)
            {
                case Characteristic.Constitution:
                    int health = (int)GetHealthRatio(GetLevel());
                    if (health > 0) stringBuilder.AppendFormat("$se_health: <color=orange>+{0}</color>\n", health);
                    break;
                case Characteristic.Strength:
                    int carryWeight = (int)GetCarryWeightRatio(GetLevel());
                    int physical = FormatPercentage(GetStrengthModifier(GetLevel()));
                    if (carryWeight > 0) stringBuilder.AppendFormat("$se_max_carryweight: <color=orange>+ {0}</color>\n", carryWeight);
                    if (physical > 0) stringBuilder.AppendFormat("$almanac_physical: <color=orange>+{0}%</color>\n", physical);
                    break;
                case Characteristic.Intelligence:
                    int intel = FormatPercentage(GetIntelligenceModifier(GetLevel()));
                    if (intel > 0) stringBuilder.AppendFormat("$almanac_elemental: <color=orange>+{0}%</color>\n", intel);
                    break;
                case Characteristic.Dexterity:
                    int stamina = (int)GetStaminaRatio(GetLevel());
                    int attackSpeed = FormatPercentage(GetDexterityModifier(GetLevel()));
                    if (stamina > 0) stringBuilder.AppendFormat("$se_stamina: <color=orange>+{0}</color>\n", stamina);
                    if (attackSpeed > 0) stringBuilder.AppendFormat("$almanac_attackspeedmod: <color=orange>+{0}%</color>", attackSpeed);
                    break;
                case Characteristic.Wisdom:
                    int eitr = (int)GetEitrRatio(GetLevel());
                    if (eitr > 0) stringBuilder.AppendFormat("$se_eitr: <color=orange>+{0}</color>", eitr);
                    break;
            }
        }
        else
        {
            stringBuilder.Append(GetDescription() + "\n\n");
            if (m_cooldown != null) 
                stringBuilder.Append($"$almanac_cooldown: <color=orange>{GetCooldown(GetLevel())}</color>$text_sec\n");
            if (m_length != null) 
                stringBuilder.Append($"$almanac_duration: <color=orange>{GetLength(GetLevel())}</color>$text_sec\n");
            if (m_eitrCost is { Value: > 0 }) 
                stringBuilder.Append($"$se_eitr $almanac_cost: <color=orange>{GetEitrCost(m_eitrCostReduces, GetLevel())}</color>\n");
            if (m_staminaCost is { Value: > 0 }) 
                stringBuilder.Append($"$se_stamina $almanac_cost: <color=orange>{GetStaminaCost()}</color>\n");
            if (m_healthCost is { Value: > 0 }) 
                stringBuilder.Append($"$se_health $almanac_cost: <color=orange>{GetHealthCost()}</color>\n");
            if (m_values != null)
            {
                if (m_values.m_absorb is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_damage_absorb: <color=orange>{GetAbsorb(GetLevel())}</color>\n");
                if (m_values.m_chance is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_chance: <color=orange>{GetChance(GetLevel())}%</color>\n");
                if (m_values.m_reflect is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_reflect: <color=orange>{FormatPercentage(GetReflect(GetLevel()) + 1f)}%</color>\n");
                if (m_values.m_bleed is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_bleed: <color=orange>{GetBleed(GetLevel())}/$text_tick</color>\n");
                if (m_values.m_eitr is {Value: > 0}) 
                    stringBuilder.Append($"$se_eitr: <color=orange>{GetEitr(GetLevel())}</color>\n");
                if (m_values.m_heal is {Value: > 0}) 
                    stringBuilder.Append($"$almanac_heal: <color=orange>{GetHealAmount(GetLevel())}</color>\n");
                if (m_values.m_health is {Value: > 0}) 
                    stringBuilder.Append($"$se_health: <color=orange>{GetHealth(GetLevel())}</color>\n");
                if (m_values.m_stamina is {Value: > 0}) 
                    stringBuilder.Append($"$se_stamina: <color=orange>{GetStamina(GetLevel())}</color>\n");
                if (m_values.m_speed is { Value: > 0}) 
                    stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedModifier(GetLevel()))}%</color>\n");
                if (m_values.m_carryWeight is { Value: > 0}) 
                    stringBuilder.Append($"$se_max_carryweight: <color=orange>{GetCarryWeight(GetLevel())}</color>\n");
                if (m_values.m_eitrRegen is {Value: > 0}) 
                    stringBuilder.Append($"$se_eitrregen: <color=orange>{FormatPercentage(GetEitrRegen(GetLevel()))}%</color>\n");
                if (m_values.m_healthRegen is {Value: > 0}) 
                    stringBuilder.Append($"$se_healthregen: <color=orange>{FormatPercentage(GetHealthRegen(GetLevel()))}%</color>\n");
                if (m_values.m_staminaRegen is {Value: > 0}) 
                    stringBuilder.Append($"$se_staminaregen: <color=orange>{FormatPercentage(GetStaminaRegen(GetLevel()))}%</color>\n");
                if (m_values.m_modifyAttack is {Value: > 0}) 
                    stringBuilder.Append($"$almanac_attack: <color=orange>{FormatPercentage(GetAttack(GetLevel()))}%</color>\n");
                if (m_values.m_comfort is { Value: > 0 }) 
                    stringBuilder.Append($"$se_rested_comfort: <color=orange>{GetAddedComfort(GetLevel())}</color>\n");
                if (m_values.m_damageReduction is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_damage_reduction: <color=orange>{FormatPercentage(GetDamageReduction(GetLevel()))}%</color>\n");
                if (m_values.m_foodModifier is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_foodmod: <color=orange>{FormatPercentage(GetFoodModifier(GetLevel()))}%</color>\n");
                if (m_values.m_forageModifier is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_foragemod: <color=orange>{FormatPercentage(GetForageModifier(GetLevel()))}%</color>\n");
                if (m_values.m_speedReduction is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedReduction(GetLevel()))}%</color>\n");
                if (m_values.m_runStaminaDrain is {Value: > 0}) 
                    stringBuilder.Append($"$se_runstamina: <color=orange>{FormatPercentage(GetRunStaminaDrain(GetLevel()))}%</color>\n");
                if (m_values.m_attackSpeedReduction is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_attackspeedmod $almanac_reduction: <color=orange>{FormatPercentage(GetAttackSpeedReduction(GetLevel()))}%</color>\n");
                if (m_values.m_sneakStaminaUsage is { Value: > 0 }) 
                    stringBuilder.Append($"$se_sneakstamina: <color=orange>{FormatPercentage(GetSneakStaminaUsage(GetLevel()))}%</color>\n");
                if (m_values.m_attackStaminaUsage is { Value: > 0 }) 
                    stringBuilder.Append($"$se_attackstamina: <color=orange>{FormatPercentage(GetAttackStaminaUsage(GetLevel()))}%</color>\n");
                if (m_values.m_armor is { Value: > 0 })
                    stringBuilder.Append($"$inventory_armor: <color=orange>+{GetArmor(GetLevel())}</color>\n");
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
                if (m_resistances.m_blunt is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_blunt $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Blunt))}%</color>\n");
                if (m_resistances.m_pierce is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_pierce $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Pierce))}%</color>\n");
                if (m_resistances.m_slash is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_slash $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Slash))}%</color>\n");
                if (m_resistances.m_fire is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_fire $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Fire))}%</color>\n");
                if (m_resistances.m_frost is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_frost $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Frost))}%</color>\n");
                if (m_resistances.m_lightning is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_lightning $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Lightning))}%</color>\n");
                if (m_resistances.m_poison is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_poison $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Poison))}%</color>\n");
                if (m_resistances.m_spirit is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_spirit $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Spirit))}%</color>\n");
            }

            if (m_creaturesByLevel != null)
            {
                if (ZNetScene.instance)
                {
                    stringBuilder.AppendFormat(
                        "$almanac_creature: <color=yellow>x{0}</color> <color=orange>{1}</color> $text_level {0}\n",
                        GetCreatureByLevelLevel(GetLevel()), 
                        GetCreatureName(GetCreaturesByLevel(GetLevel())));
                    stringBuilder.AppendFormat(
                        "$text_lvl 1-3: <color=orange>{0}</color>\n", 
                        GetCreatureName(GetCreaturesByLevel(3)));
                    stringBuilder.AppendFormat(
                        "$text_lvl 4-6: <color=orange>{0}</color> $info_required: $info_defeated {1}\n",
                        GetCreatureName(ZNetScene.instance.GetPrefab(m_creaturesByLevel.m_fourToSix?.Value ?? "")), GetCreatureName(ZNetScene.instance.GetPrefab("Bonemass")));
                    stringBuilder.AppendFormat(
                        "$text_lvl 6-9: <color=orange>{0}</color> $info_required: $info_defeated {1}\n",
                        GetCreatureName(ZNetScene.instance.GetPrefab(m_creaturesByLevel.m_sevenToNine?.Value ?? "")), 
                        GetCreatureName(ZNetScene.instance.GetPrefab("GoblinKing")));
                    stringBuilder.AppendFormat(
                        "$text_lvl 10+: <color=orange>{0}</color> $info_required: $info_defeated {1}",
                        GetCreatureName(ZNetScene.instance.GetPrefab(m_creaturesByLevel.m_ten?.Value ?? "")),
                        GetCreatureName(ZNetScene.instance.GetPrefab("SeekerQueen")));
                }
            }
            if (m_creatures != null)
                stringBuilder.Append($"{GetBiomeLocalized(GetCurrentBiome())} $almanac_creature: <color=orange>{GetCreatureName(GetCreatures(GetCurrentBiome()))}</color> $text_lvl {GetLevel()} \n ({GetCreaturesLength(GetLevel())}$text_sec)\n");
            if (m_key == "Trader")
                stringBuilder.Append($"$almanac_allows_to_tp <color=orange>{GetLevel()}</color>\n");
            if (m_key == "AirBender")
                stringBuilder.Append($"$almanac_jumps: <color=orange>{GetLevel()}</color>\n");
        }
        return Localization.instance.Localize(stringBuilder.ToString());
    }
    private ConfigEntry<string>? GetCreaturesConfig()
    {
        if (!Player.m_localPlayer || m_creatures == null) return null;
        ConfigEntry<string>? config = null!;
        switch (GetCurrentBiome())
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
            case Heightmap.Biome.None:
                return null;
        }
        return config;
    }
    private Heightmap.Biome GetCurrentBiome() => Player.m_localPlayer == null ? Heightmap.Biome.None : Player.m_localPlayer.GetCurrentBiome();
    private string GetBiomeLocalized(Heightmap.Biome biome) => $"$biome_{biome.ToString().ToLower()}";
    private int FormatPercentage(float value) => (int)(value * 100 - 100);
    public string GetPrestigeTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (m_type is TalentType.Characteristic)
        {
            if (m_characteristic == null) return stringBuilder.ToString();
            stringBuilder.Append($"{GetCharacteristicDescription()}\n");
            stringBuilder.AppendFormat("+ <color=orange>{0}</color> --> <color={1}>{2}</color> {3}\n\n", 
                GetCharacteristic(GetLevel()), m_prestigeColor, GetCharacteristic(GetLevel() + 1), GetCharacteristicLocalized());
            switch (m_characteristic.m_type)
            {
                case Characteristic.Constitution:
                    stringBuilder.AppendFormat("$se_health: <color=orange>+{0}</color> --> <color={1}>{2}</color>\n", 
                        (int)GetHealthRatio(GetLevel()), m_prestigeColor, (int)GetHealthRatio(GetLevel() + 1));
                    break;
                case Characteristic.Strength:
                    stringBuilder.AppendFormat("$se_max_carryweight: <color=orange>+{0}</color> --> <color={1}>+{2}</color>\n", 
                        (int)GetCarryWeightRatio(GetLevel()), m_prestigeColor, (int)GetCarryWeightRatio(GetLevel() + 1));
                    stringBuilder.AppendFormat("$almanac_physical: <color=orange>+{0}%</color> --> <color={1}>{2}%</color>\n",
                        FormatPercentage(GetStrengthModifier(GetLevel())), m_prestigeColor, FormatPercentage(GetStrengthModifier(GetLevel() + 1)));
                    break;
                case Characteristic.Intelligence:
                    stringBuilder.AppendFormat("$almanac_elemental: <color=orange>+{0}%</color> --> <color={1}>{2}%</color>\n", 
                        FormatPercentage(GetIntelligenceModifier(GetLevel())), m_prestigeColor, FormatPercentage(GetIntelligenceModifier(GetLevel() + 1)));
                    break;
                case Characteristic.Dexterity:
                    stringBuilder.AppendFormat("$se_stamina: <color=orange>+{0}</color> --> <color={1}>{2}</color>\n", 
                        (int)GetStaminaRatio(GetLevel()), m_prestigeColor, (int)GetStaminaRatio(GetLevel() + 1));
                    stringBuilder.AppendFormat("$almanac_attackspeedmod: <color=orange>+{0}%</color> --> <color={1}>{2}%</color> \n", 
                        FormatPercentage(GetDexterityModifier(GetLevel())), m_prestigeColor, FormatPercentage(GetDexterityModifier(GetLevel() + 1)));
                    break;
                case Characteristic.Wisdom:
                    stringBuilder.AppendFormat("$se_eitr: <color=orange>+{0}</color> --> <color={1}>{2}</color>", 
                        (int)GetEitrRatio(GetLevel()), m_prestigeColor, (int)GetEitrRatio(GetLevel() + 1));
                    break;
            }
        }
        else
        {
            stringBuilder.Append(GetDescription() + "\n\n");
            if (m_cooldown != null)
                stringBuilder.Append($"$almanac_cooldown: <color=orange>{GetCooldown(GetLevel())}</color>$text_sec --> <color={m_prestigeColor}>{GetCooldown(GetLevel() + 1)}</color>$text_sec\n");
            if (m_length != null)
                stringBuilder.Append($"$almanac_duration: <color=orange>{GetLength(GetLevel())}</color>$text_sec --> <color={m_prestigeColor}>{GetLength(GetLevel() + 1)}</color>$text_sec\n");
            if (m_eitrCost is { Value: > 0 })
            {
                if (m_eitrCostReduces)
                {
                    stringBuilder.AppendFormat(
                        "$se_eitr $almanac_cost: <color=orange>{0}</color> --> <color={1}>{2}</color>\n",
                        GetEitrCost(true, GetLevel()), m_prestigeColor, GetEitrCost(true, GetLevel() + 1));
                }
                else
                {
                    stringBuilder.Append($"$se_eitr $almanac_cost: <color=orange>{GetEitrCost(false)}</color>\n");
                }
            }
            if (m_staminaCost is { Value: > 0 }) 
                stringBuilder.Append($"$se_stamina $almanac_cost: <color=orange>{GetStaminaCost()}</color>\n");
            if (m_healthCost is { Value: > 0 }) 
                stringBuilder.Append($"$se_health $almanac_cost: <color=orange>{GetHealthCost()}</color>\n");
            if (m_values != null)
            {
                if (m_values.m_absorb is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_damage_absorb: <color=orange>{GetAbsorb(GetLevel())}</color> --> <color={m_prestigeColor}>{GetAbsorb(GetLevel() + 1)}</color>\n");
                if (m_values.m_chance is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_chance: <color=orange>{GetChance(GetLevel())}%</color> --> <color={m_prestigeColor}>{GetChance(GetLevel() + 1)}%</color>\n");
                if (m_values.m_reflect is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_reflect: <color=orange>{FormatPercentage(GetReflect(GetLevel()) + 1f)}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetReflect(GetLevel() + 1) + 1f)}%</color>\n");
                if (m_values.m_bleed is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_bleed: <color=orange>{GetBleed(GetLevel())}/$text_tick</color> --> <color={m_prestigeColor}>{GetBleed(GetLevel() + 1)}/$text_tick</color>\n");
                if (m_values.m_eitr is {Value: > 0}) 
                    stringBuilder.Append($"$se_eitr: <color=orange>{GetEitr(GetLevel())}</color> --> <color={m_prestigeColor}>{GetEitr(GetLevel() + 1)}</color>\n");
                if (m_values.m_heal is {Value: > 0}) 
                    stringBuilder.Append($"$almanac_heal: <color=orange>{GetHealAmount(GetLevel())}</color> --> <color={m_prestigeColor}>{GetHealAmount(GetLevel() + 1)}</color>\n");
                if (m_values.m_health is {Value: > 0}) 
                    stringBuilder.Append($"$se_health: <color=orange>{GetHealth(GetLevel())}</color> --> <color={m_prestigeColor}>{GetHealth(GetLevel() + 1)}</color>\n");
                if (m_values.m_stamina is {Value: > 0}) 
                    stringBuilder.Append($"$se_stamina: <color=orange>{GetStamina(GetLevel())}</color> --> <color={m_prestigeColor}>{GetStamina(GetLevel() + 1)}</color>\n");
                if (m_values.m_speed is { Value: > 0}) 
                    stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedModifier(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetSpeedModifier(GetLevel() + 1))}%</color>\n");
                if (m_values.m_carryWeight is { Value: > 0}) 
                    stringBuilder.Append($"$se_max_carryweight: <color=orange>{GetCarryWeight(GetLevel())}</color> --> <color={m_prestigeColor}>{GetCarryWeight(GetLevel() + 1)}</color>\n");
                if (m_values.m_eitrRegen is {Value: > 0}) 
                    stringBuilder.Append($"$se_eitrregen: <color=orange>{FormatPercentage(GetEitrRegen(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetEitrRegen(GetLevel() + 1))}%</color>\n");
                if (m_values.m_healthRegen is {Value: > 0}) 
                    stringBuilder.Append($"$se_healthregen: <color=orange>{FormatPercentage(GetHealthRegen(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetHealthRegen(GetLevel() + 1))}%</color>\n");
                if (m_values.m_staminaRegen is {Value: > 0}) 
                    stringBuilder.Append($"$se_staminaregen: <color=orange>{FormatPercentage(GetStaminaRegen(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetStaminaRegen(GetLevel() + 1))}%</color>\n");
                if (m_values.m_modifyAttack is {Value: > 0}) 
                    stringBuilder.Append($"$almanac_attack: <color=orange>{FormatPercentage(GetAttack(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetAttack(GetLevel() + 1))}%</color>\n");
                if (m_values.m_comfort is { Value: > 0 }) 
                    stringBuilder.Append($"$se_rested_comfort: <color=orange>{GetAddedComfort(GetLevel())}</color> --> <color={m_prestigeColor}>{GetAddedComfort(GetLevel() + 1)}</color>\n");
                if (m_values.m_damageReduction is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_damage_reduction: <color=orange>{FormatPercentage(GetDamageReduction(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetDamageReduction(GetLevel() + 1))}%</color>\n");
                if (m_values.m_foodModifier is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_foodmod: <color=orange>{FormatPercentage(GetFoodModifier(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetFoodModifier(GetLevel() + 1))}%</color>\n");
                if (m_values.m_forageModifier is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_foragemod: <color=orange>{FormatPercentage(GetForageModifier(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetForageModifier(GetLevel() + 1))}%</color>\n");
                if (m_values.m_speedReduction is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_speed: <color=orange>{FormatPercentage(GetSpeedReduction(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetSpeedReduction(GetLevel() + 1))}%</color>\n");
                if (m_values.m_runStaminaDrain is {Value: > 0}) 
                    stringBuilder.Append($"$se_runstamina: <color=orange>{FormatPercentage(GetRunStaminaDrain(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetRunStaminaDrain(GetLevel() + 1))}%</color>\n");
                if (m_values.m_attackSpeedReduction is { Value: > 0 }) 
                    stringBuilder.Append($"$almanac_attackspeedmod $almanac_reduction: <color=orange>{FormatPercentage(GetAttackSpeedReduction(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetAttackSpeedReduction(GetLevel() + 1))}%</color>\n");
                if (m_values.m_sneakStaminaUsage is { Value: > 0 }) 
                    stringBuilder.Append($"$se_sneakstamina: <color=orange>{FormatPercentage(GetSneakStaminaUsage(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetSneakStaminaUsage(GetLevel() + 1))}%</color>\n");
                if (m_values.m_attackStaminaUsage is { Value: > 0 }) 
                    stringBuilder.Append($"$se_attackstamina: <color=orange>{FormatPercentage(GetAttackStaminaUsage(GetLevel()))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetAttackStaminaUsage(GetLevel() + 1))}%</color>\n");
                if (m_values.m_armor is {Value: > 0})
                    stringBuilder.Append($"$inventory_armor: <color=orange>+{GetArmor(GetLevel())}</color> --> <color={m_prestigeColor}>+{GetArmor(GetLevel() + 1)}</color>\n");
            }

            if (m_damages != null)
            {
                HitData.DamageTypes damages = GetDamages(GetLevel());
                HitData.DamageTypes newDamages = GetDamages(GetLevel() + 1);
                if (m_damages.m_blunt is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_blunt: <color=orange>{damages.m_blunt}</color> --> <color={m_prestigeColor}>{newDamages.m_blunt}</color>\n");
                if (m_damages.m_pierce is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_pierce: <color=orange>{damages.m_pierce}</color> --> <color={m_prestigeColor}>{newDamages.m_pierce}</color>\n");
                if (m_damages.m_slash is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_slash: <color=orange>{damages.m_slash}</color> --> <color={m_prestigeColor}>{newDamages.m_slash}</color>\n");
                if (m_damages.m_pickaxe is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_pickaxe: <color=orange>{damages.m_pickaxe}</color> --> <color={m_prestigeColor}>{newDamages.m_pickaxe}</color>\n");
                if (m_damages.m_chop is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_chop: <color=orange>{damages.m_chop}</color> --> <color={m_prestigeColor}>{newDamages.m_chop}</color>\n");
                if (m_damages.m_fire is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_fire: <color=orange>{damages.m_fire}</color> --> <color={m_prestigeColor}>{newDamages.m_fire}</color>\n");
                if (m_damages.m_frost is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_frost: <color=orange>{damages.m_frost}</color> --> <color={m_prestigeColor}>{newDamages.m_frost}</color>\n");
                if (m_damages.m_lightning is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_lightning: <color=orange>{damages.m_lightning}</color> --> <color={m_prestigeColor}>{newDamages.m_lightning}</color>\n");
                if (m_damages.m_poison is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_poison: <color=orange>{damages.m_poison}</color> --> <color={m_prestigeColor}>{newDamages.m_poison}</color>\n");
                if (m_damages.m_spirit is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_spirit: <color=orange>{damages.m_spirit}</color> --> <color={m_prestigeColor}>{newDamages.m_spirit}</color>\n");
            }

            if (m_resistances != null)
            {
                if (m_resistances.m_blunt is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_blunt $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Blunt))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Blunt))}%</color>\n");
                if (m_resistances.m_pierce is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_pierce $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Pierce))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Pierce))}%</color>\n");
                if (m_resistances.m_slash is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_slash $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Slash))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Slash))}%</color>\n");
                if (m_resistances.m_fire is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_fire $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Fire))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Fire))}%</color>\n");
                if (m_resistances.m_frost is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_frost $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Frost))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Frost))}%</color>\n");
                if (m_resistances.m_lightning is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_lightning $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Lightning))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Lightning))}%</color>\n");
                if (m_resistances.m_poison is { Value: > 0 }) 
                    stringBuilder.Append($"$inventory_poison $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Poison))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Poison))}%</color>\n");
                if (m_resistances.m_spirit is {Value: > 0}) 
                    stringBuilder.Append($"$inventory_spirit $almanac_reduction: <color=orange>{FormatPercentage(GetResistance(GetLevel(), HitData.DamageType.Spirit))}%</color> --> <color={m_prestigeColor}>{FormatPercentage(GetResistance(GetLevel() + 1, HitData.DamageType.Spirit))}%</color>\n");
            }

            if (m_creaturesByLevel != null)
            {
                stringBuilder.Append(
                    $"$almanac_creature: <color=yellow>x{GetCreatureByLevelLevel(GetLevel())}</color> --> <color={m_prestigeColor}>x{GetCreatureByLevelLevel(GetLevel() + 1)}</color> <color=orange>{GetCreaturesByLevel(GetLevel())?.name ?? ""}</color> --> <color={m_prestigeColor}>{GetCreaturesByLevel(GetLevel() + 1)?.name ?? ""}</color> $text_level {GetCreatureByLevelLevel(GetLevel())} --> <color={m_prestigeColor}>{GetCreatureByLevelLevel(GetLevel() + 1)}</color>\n");
            }
            if (m_creatures != null)
                stringBuilder.Append($"{GetBiomeLocalized(GetCurrentBiome())} $almanac_creature: <color=orange>{GetCreaturesConfig()?.Value ?? ""}</color> $text_lvl {GetLevel()} --> <color={m_prestigeColor}>{GetLevel() + 1}</color> \n ({GetCreaturesLength(GetLevel())}$text_sec --> <color={m_prestigeColor}>{GetCreaturesLength(GetLevel() + 1)}$text_sec</color>)\n");
            if (m_key == "Trader")
                stringBuilder.Append($"$almanac_allows_to_tp <color=orange>{GetLevel()}</color> --> <color={m_prestigeColor}>{GetLevel() + 1}</color>\n");
            if (m_key == "AirBender")
                stringBuilder.Append($"$almanac_jumps: <color=orange>{GetLevel()}</color> --> <color={m_prestigeColor}>{GetLevel() + 1}</color>\n");
        }
        return Localization.instance.Localize(stringBuilder.ToString());
    }
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
        public ConfigEntry<float>? m_attackSpeedReduction;
        public ConfigEntry<float>? m_attackStaminaUsage;
        public ConfigEntry<float>? m_sneakStaminaUsage;
        public ConfigEntry<float>? m_armor;
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
    public class CreaturesByLevel
    {
        public ConfigEntry<string>? m_oneToThree;
        public ConfigEntry<string>? m_fourToSix;
        public ConfigEntry<string>? m_sevenToNine;
        public ConfigEntry<string>? m_ten;
    }
    public class ResistancePercentages
    {
        public ConfigEntry<float>? m_blunt;
        public ConfigEntry<float>? m_pierce;
        public ConfigEntry<float>? m_slash;
        public ConfigEntry<float>? m_fire;
        public ConfigEntry<float>? m_frost;
        public ConfigEntry<float>? m_lightning;
        public ConfigEntry<float>? m_poison;
        public ConfigEntry<float>? m_spirit;
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

    public Talent(string key, string buttonName, TalentType type, bool alt = false)
    {
        m_key = key;
        m_button = buttonName;
        m_type = type;
        TalentManager.m_talents[key] = this;
        if (alt) TalentManager.m_altTalentsByButton[m_button] = this;
        else TalentManager.m_talentsByButton[m_button] = this;
    }

    public void AddStatusEffect(StatusEffect effect, string name)
    {
        m_status = effect;
        m_status.name = name;
        TalentManager.m_talentsByStatusEffect[name.GetStableHashCode()] = this;
    }
}

public enum TalentType { Ability, Passive, StatusEffect, Characteristic }

public static class TalentManager
{
    private static bool m_initiated;
    public static readonly Dictionary<string, Talent> m_talents = new();
    public static readonly Dictionary<string, Talent> m_talentsByButton = new();
    public static readonly Dictionary<string, Talent> m_altTalentsByButton = new();
    public static readonly Dictionary<int, Talent> m_talentsByStatusEffect = new();

    public static void ResetTalentLevels()
    {
        foreach (KeyValuePair<string, Talent> kvp in m_talents) kvp.Value.SetLevel(1);
    }

    public static void InitializeTalents()
    {
        if (m_initiated) return;
        _Plugin.Config.SaveOnConfigSet = false;
        AlmanacClassesLogger.LogDebug("Initializing talents");
        FilePaths.CreateFolders();
        m_talents.Clear();
        m_talentsByButton.Clear();
        LoadTalents();
        LoadAltTalents();
        _Plugin.Config.SaveOnConfigSet = true;
        m_initiated = true;
    }

    private static void LoadTalents()
    {
        LoadCore();
        LoadRanger();
        LoadSage();
        LoadShaman();
        LoadBard();
        LoadRogue();
        LoadWarrior();
    }

    private static void LoadAltTalents()
    {
        LoadAltCore();
        LoadAltWarrior();
    }
    
    #region Data
    private static void LoadAltCore()
    {
        Talent TreasureHunter = new Talent("TreasureHunter", "$button_treasure", TalentType.StatusEffect, true);
        TreasureHunter.AddStatusEffect(ScriptableObject.CreateInstance<SE_TreasureHunter>(), "SE_TreasureHunter");
        TreasureHunter.m_sprite = SpriteManager.Wishbone_Icon;
        TreasureHunter.m_cost = _Plugin.config("Core - Treasure Hunter", "Purchase Cost", 3, new ConfigDescription("Set cost to unlock ability", new AcceptableValueRange<int>(1, 10)));
        TreasureHunter.m_cooldown = _Plugin.config("Core - Treasure Hunter", "Cooldown", 180f, new ConfigDescription("Set cooldown of ability", new AcceptableValueRange<float>(1f, 1000f)));
        TreasureHunter.m_length = _Plugin.config("Core - Treasure Hunter", "Length", 60f, new ConfigDescription("Set length of ability", new AcceptableValueRange<float>(1f, 1000f)));
        TreasureHunter.m_alt = _Plugin.config("Core - Treasure Hunter", "Enable", Toggle.Off, "If on, replaces forage talent");
        TreasureHunter.m_cap = _Plugin.config("Core - Treasure Hunter", "Prestige Cap", 1, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 10)));
        TreasureHunter.m_altButtonSprite = SpriteManager.ScrollMap;
        TreasureHunter.m_alt.SettingChanged += (_, _) =>
        {
            LoadUI.ChangeButton(TreasureHunter, TreasureHunter.m_alt.Value is Toggle.Off);
        };

        Talent Berzerk = new Talent("Berzerk", "$button_rain", TalentType.Passive, true);
        Berzerk.m_sprite = SpriteManager.WarriorIcon;
        Berzerk.m_cost = _Plugin.config("Core - Berzerk", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock ability", new AcceptableValueRange<int>(1, 10)));
        Berzerk.m_values = new Talent.TalentValues()
        {
            m_armor = _Plugin.config("Core - Berzerk", "Armor", 3f, new ConfigDescription("Set base added passive armor", new AcceptableValueRange<float>(0f, 10f))),
        };
        Berzerk.m_alt = _Plugin.config("Core - Berzerk", "Enable", Toggle.Off, "If on, replaces the rain proof talent");
        Berzerk.m_cap = _Plugin.config("Core - Berzerk", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 10)));
        Berzerk.m_altButtonSprite = SpriteManager.ShieldIcon;
        Berzerk.m_alt.SettingChanged += (_, _) =>
        {
            LoadUI.ChangeButton(Berzerk, Berzerk.m_alt.Value is Toggle.Off);
        };

        Talent Sailor = new Talent("Sailor", "$button_sail", TalentType.StatusEffect, true);
        Sailor.AddStatusEffect(ScriptableObject.CreateInstance<SE_Sailor>(), "SE_Sailor");
        Sailor.m_cost = _Plugin.config("Core - Sailor", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock ability", new AcceptableValueRange<int>(1, 10)));
        Sailor.m_alt = _Plugin.config("Core - Sailor", "Enable", Toggle.Off, "If on replaces the gypsy talent");
        Sailor.m_cap = _Plugin.config("Core - Sailor", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 10)));
        Sailor.m_startEffects = LoadedAssets.GP_Moder.m_startEffects;
        Sailor.m_animation = "point";
        Sailor.m_useAnimation = _Plugin.config("Core - Sailor", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        Sailor.m_length = _Plugin.config("Core - Sailor", "Length", 50f, new ConfigDescription("Set the duration of effect", new AcceptableValueRange<float>(1f, 1000f)));
        Sailor.m_cooldown = _Plugin.config("Core - Sailor", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(1f, 1000f)));
        Sailor.m_sprite = SpriteManager.Sail_Icon;
        Sailor.m_staminaCost = _Plugin.config("Core - Sailor", "Stamina Cost", 30f, new ConfigDescription("Set cost to activate", new AcceptableValueRange<float>(0f, 101f)));
        Sailor.m_altButtonSprite = SpriteManager.WindIcon;
        Sailor.m_alt.SettingChanged += (_, _) =>
        {
            LoadUI.ChangeButton(Sailor, Sailor.m_alt.Value is Toggle.Off);
        };

        Talent AirBenderAlt = new Talent("AirBenderAlt", "$button_lumberjack", TalentType.Passive, true);
        AirBenderAlt.m_sprite = SpriteManager.MedalIcon;
        AirBenderAlt.m_cost = _Plugin.config("Core - AirBender Alt", "Purchase Cost", 3, new ConfigDescription("Set cost to unlock ability", new AcceptableValueRange<int>(1, 10)));
        AirBenderAlt.m_alt = _Plugin.config("Core - AirBender Alt", "Enable", Toggle.Off, "If on, replaces the airbender talent");
        AirBenderAlt.m_cap = _Plugin.config("Core - AirBender Alt", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 10)));
        AirBenderAlt.m_eitrCost = _Plugin.config("Core - AirBender Alt", "Eitr Cost", 5f, new ConfigDescription("Set eitr cost", new AcceptableValueRange<float>(0f, 101f)));
        AirBenderAlt.m_eitrCostReduces = true;
        AirBenderAlt.m_alt.SettingChanged += (_, _) =>
        {
            LoadUI.ChangeButton(AirBenderAlt, AirBenderAlt.m_alt.Value is Toggle.Off);
        };
    }
    private static void LoadAltWarrior()
    {
        Talent survivor = new Talent("Survivor", "$button_warrior_talent_5", TalentType.Passive, true);
        survivor.AddStatusEffect(ScriptableObject.CreateInstance<SE_Survivor>(), "SE_Survivor");
        survivor.m_altButtonSprite = SpriteManager.CrownIcon;
        survivor.m_values = new Talent.TalentValues()
        {
            m_chance = _Plugin.config("Warrior - Survivor", "Chance", 20f, new ConfigDescription("Set the chance to not die and regain health", new AcceptableValueRange<float>(0f, 100f))),
        };
        survivor.m_cost = _Plugin.config("Warrior - Survivor", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock ability", new AcceptableValueRange<int>(1, 10)));
        survivor.m_alt = _Plugin.config("Warrior - Survivor", "Enable", Toggle.Off, "If on, replaces dual wield talent");
        survivor.m_startEffects = LoadedAssets.VFX_SongOfSpirit;
        survivor.m_cap = _Plugin.config("Warrior - Survivor", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        survivor.m_passiveActive = false;
        survivor.m_alt.SettingChanged += (_, _) =>
        {
            LoadUI.ChangeButton(survivor, survivor.m_alt.Value is Toggle.Off);
        };
        Talent battleFury = new Talent("BattleFury", "$button_warrior_talent_4", TalentType.Passive, true);
        battleFury.AddStatusEffect(ScriptableObject.CreateInstance<SE_BattleFury>(), "SE_BattleFury");
        battleFury.m_cost = _Plugin.config("Warrior - Battle Fury", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock talent", new AcceptableValueRange<int>(1, 10)));
        battleFury.m_alt = _Plugin.config("Warrior - Battle Fury", "Enable", Toggle.Off, "If on, replaces monkey wrench talent");
        battleFury.m_startEffects = LoadedAssets.FX_BattleFury;
        battleFury.m_cap = _Plugin.config("Warrior - Battle Fury", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        battleFury.m_values = new Talent.TalentValues()
        {
            m_chance = _Plugin.config("Warrior - Battle Fury", "Chance", 10f, new ConfigDescription("Set chance to trigger ability", new AcceptableValueRange<float>(0f, 100f))),
            m_stamina = _Plugin.config("Warrior - Battle Fury", "Stamina Gain", 10f, new ConfigDescription("Set amount gained per kill", new AcceptableValueRange<float>(1f, 50f)))
        };
        battleFury.m_alt.SettingChanged += (_, _) =>
        {
            LoadUI.ChangeButton(battleFury, battleFury.m_alt.Value is Toggle.Off);
        };
    }
    private static void LoadCore()
    {
        Talent Core1 = new Talent("Core1", "$button_core_1", TalentType.Characteristic);
        Core1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent Core2 = new Talent("Core2", "$button_core_2", TalentType.Characteristic);
        Core2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };
        Talent Core3 = new Talent("Core3", "$button_core_3", TalentType.Characteristic);
        Core3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10
        };
        Talent Core4 = new Talent("Core4", "$button_core_4", TalentType.Characteristic);
        Core4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };
        Talent Core5 = new Talent("Core5", "$button_core_5",TalentType.Characteristic);
        Core5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10
        };
        Talent Core6 = new Talent("Core6", "$button_core_6", TalentType.Characteristic);
        Core6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };
        Talent Core8 = new Talent("Core8", "$button_core_8", TalentType.Characteristic);
        Core8.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };
    
        Talent Core9 = new Talent("Core9", "$button_core_9", TalentType.Characteristic);
        Core9.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };
    
        Talent Core10 = new Talent("Core10", "$button_core_10", TalentType.Characteristic);
        Core10.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };
    
        Talent Core11 = new Talent("Core11", "$button_core_11", TalentType.Characteristic);
        Core11.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };
    
        Talent Core12 = new Talent("Core12", "$button_core_12", TalentType.Characteristic);
        Core12.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };
        Talent wise = new Talent("Wise", "$button_sneak", TalentType.Passive);
        wise.AddStatusEffect(ScriptableObject.CreateInstance<StatusEffect>(), "SE_Enlightened");
        wise.m_cost = _Plugin.config("Core - Wise", "Purchase Cost", 5, new ConfigDescription("Set the cost to purchase talent", new AcceptableValueRange<int>(1, 10)));
        wise.m_values = new Talent.TalentValues()
        {
            m_eitr = _Plugin.config("Core - Wise", "Eitr", 5f, new ConfigDescription("Set the amount of eitr", new AcceptableValueRange<float>(0f, 101f)))
        };
        wise.m_cap = _Plugin.config("Core - Wise", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent Airbender = new Talent("AirBender", "$button_lumberjack", TalentType.Passive);
        Airbender.m_sprite = SpriteManager.MedalIcon;
        Airbender.m_cost = _Plugin.config("Core - Air Bender", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        Airbender.m_eitrCost = _Plugin.config("Core - Air Bender", "Eitr Cost", 10f, new ConfigDescription("Set the eitr cost to jump in the air", new AcceptableValueRange<float>(0f, 1000f)));
        Airbender.m_cap = _Plugin.config("Core - Air Bender", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent comfort = new Talent("Comfort", "$button_comfort_2", TalentType.Passive);
        comfort.m_type = TalentType.Passive;
        comfort.m_cost = _Plugin.config("Core - Relax", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        comfort.m_values = new Talent.TalentValues()
        {
            m_comfort = _Plugin.config("Core - Relax", "Comfort", 1f, new ConfigDescription("Set comfort amount", new AcceptableValueRange<float>(0f, 10f)))
        };
        comfort.m_cap = _Plugin.config("Core - Relax", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent resourceful = new Talent("Resourceful", "$button_comfort_1", TalentType.Passive);
        resourceful.m_cost = _Plugin.config("Core - Resourceful", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        resourceful.AddStatusEffect(ScriptableObject.CreateInstance<SE_Resourceful>(), "SE_Resourceful");
        resourceful.m_damages = new Talent.TalentDamages()
        {
            m_chop = _Plugin.config("Core - Resourceful", "Chop Damage", 5f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 101f))),
            m_pickaxe = _Plugin.config("Core - Resourceful", "Pickaxe Damage", 5f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 101f)))
        };
        resourceful.m_cap = _Plugin.config("Core - Resourceful", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent MasterChef = new Talent("MasterChef", "$button_chef", TalentType.Passive);
        MasterChef.m_cost = _Plugin.config("Core - Chef", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        MasterChef.m_values = new Talent.TalentValues()
        {
            m_foodModifier = _Plugin.config("Core - Chef", "Modifier", 1.1f, new ConfigDescription("Set the modifier", new AcceptableValueRange<float>(1f, 2f)))
        };
        MasterChef.m_cap = _Plugin.config("Core - Chef", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent packMule = new Talent("PackMule", "$button_shield", TalentType.Passive);
        packMule.AddStatusEffect(ScriptableObject.CreateInstance<SE_PackMule>(), "SE_PackMule");
        packMule.m_cost = _Plugin.config("Core - Pack Mule", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        packMule.m_values = new Talent.TalentValues()
        {
            m_carryWeight = _Plugin.config("Core - Pack Mule", "Carry Weight", 30, new ConfigDescription("Set increase carry weight", new AcceptableValueRange<int>(0, 1000)))
        };
        packMule.m_cap = _Plugin.config("Core - Pack Mule", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent rainProof = new Talent("RainProof", "$button_rain", TalentType.Passive);
        rainProof.m_cost = _Plugin.config("Core - Rain Proof", "Purchase Cost", 10, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rainProof.m_cap = _Plugin.config("Core - Rain Proof", "Prestige Cap", 1, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent doubleLoot = new Talent("DoubleLoot", "$button_merchant", TalentType.Passive);
        doubleLoot.m_cost = _Plugin.config("Core - Double Loot", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        doubleLoot.m_values = new Talent.TalentValues()
        {
            m_chance = _Plugin.config("Core - Double Loot", "Chance", 20f, new ConfigDescription("Set the chance to get loot", new AcceptableValueRange<float>(0f, 100f)))
        };
        doubleLoot.m_cap = _Plugin.config("Core - Double Loot", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent trader = new Talent("Trader", "$button_sail", TalentType.Passive);
        trader.m_cost = _Plugin.config("Core - Trader", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        trader.m_cap = _Plugin.config("Core - Trader", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent forager = new Talent("Forager", "$button_treasure", TalentType.Passive);
        forager.m_cost = _Plugin.config("Core - Forager", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        forager.m_values = new Talent.TalentValues()
        {
            m_forageModifier = _Plugin.config("Core - Forager", "Modifier", 1.1f, new ConfigDescription("Set the multiplier of foraged amount", new AcceptableValueRange<float>(0f, 10f)))
        };
        forager.m_forageItems = _Plugin.config("Core - Forager", "Custom Forage Item Names", "Thistle:Dandelion:Turnip:Entrails:Barley:Flax",
            "Define custom forage item list, [prefabName]:[prefabName]:...");
        forager.m_cap = _Plugin.config("Core - Forager", "Prestige Cap", 10,
            new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
    }
    private static void LoadRanger()
    {
        Talent ranger1 = new Talent("Ranger1", "$button_ranger_1", TalentType.Characteristic);
        ranger1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent ranger2 = new Talent("Ranger2", "$button_ranger_2", TalentType.Characteristic);
        ranger2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent ranger3 = new Talent("Ranger3", "$button_ranger_3", TalentType.Characteristic);
        ranger3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent ranger4 = new Talent("Ranger4", "$button_ranger_4", TalentType.Characteristic);
        ranger4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent ranger5 = new Talent("Ranger5", "$button_ranger_5", TalentType.Characteristic);
        ranger5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };

        Talent ranger6 = new Talent("Ranger6", "$button_ranger_6", TalentType.Characteristic);
        ranger6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10
        };
        Talent rangerTamer = new Talent("RangerTamer", "$button_ranger_talent_3", TalentType.Ability);
        rangerTamer.m_ability = () =>
        {
            if (rangerTamer.GetCreatures(Player.m_localPlayer.GetCurrentBiome()) is not { } creature) return false;
            RangerSpawn.TriggerHunterSpawn(creature, rangerTamer);
            return true;
        };
        rangerTamer.m_cooldown = _Plugin.config("Ranger - Summon", "Cooldown", 165f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rangerTamer.m_cost = _Plugin.config("Ranger - Summon", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rangerTamer.m_creatures = new Talent.TalentCreatures()
        {
            m_meadows = _Plugin.config("Ranger - Summon", "Meadows", "Neck", "Set creature summon"),
            m_blackforest = _Plugin.config("Ranger - Summon", "Black Forest", "Greydwarf", "Set creature summon"),
            m_swamp = _Plugin.config("Ranger - Summon", "Swamp", "Draugr", "Set creature summon"),
            m_mountains = _Plugin.config("Ranger - Summon", "Mountains", "Ulv", "Set creature summon"),
            m_plains = _Plugin.config("Ranger - Summon", "Plains", "Deathsquito", "Set creature summon"),
            m_mistlands = _Plugin.config("Ranger - Summon", "Mistlands", "Seeker", "Set creature summon"),
            m_ashlands = _Plugin.config("Ranger - Summon", "Ashlands", "Seeker", "Set creature summon"),
            m_deepnorth = _Plugin.config("Ranger - Summon", "Deep North", "Lox", "Set creature summon"),
            m_ocean = _Plugin.config("Ranger - Summon", "Ocean", "Serpent", "Set creature summon"),
        };
        rangerTamer.m_length = _Plugin.config("Ranger - Summon", "Length", 75f, new ConfigDescription("Set the amount of time until de-spawn", new AcceptableValueRange<float>(1f, 1000f)));
        rangerTamer.m_sprite = SpriteManager.CreatureMask_Icon;
        rangerTamer.m_animation = "Summon";
        rangerTamer.m_useAnimation = _Plugin.config("Ranger - Summon", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rangerTamer.m_healthCost = _Plugin.config("Ranger - Summon", "Health Cost", 10f, new ConfigDescription("Set health cost to summon", new AcceptableValueRange<float>(0f, 100f)));
        rangerTamer.m_staminaCost = _Plugin.config("Ranger - Summon", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerTamer.m_eitrCost = _Plugin.config("Ranger - Summon", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerTamer.m_cap = _Plugin.config("Ranger - Summon", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent rangerHunter = new Talent("RangerHunter", "$button_ranger_talent_1", TalentType.StatusEffect);
        rangerHunter.AddStatusEffect(ScriptableObject.CreateInstance<SE_Hunter>(), "SE_Hunter");
        rangerHunter.m_cooldown = _Plugin.config("Ranger - Hunter", "Cooldown", 110f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rangerHunter.m_cost = _Plugin.config("Ranger - Hunter", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rangerHunter.m_values = new Talent.TalentValues()
        {
            m_speedReduction = _Plugin.config("Ranger - Hunter", "Speed Reduction", 0.15f, new ConfigDescription("Set the speed of creatures nearby", new AcceptableValueRange<float>(0f, 1f)))
        };
        rangerHunter.m_length = _Plugin.config("Ranger - Hunter", "Length", 10f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        rangerHunter.m_sprite = SpriteManager.DeerHunter_Icon;
        rangerHunter.m_startEffects = LoadedAssets.SoothEffects;
        rangerHunter.m_healthCost = _Plugin.config("Ranger - Hunter", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerHunter.m_staminaCost = _Plugin.config("Ranger - Hunter", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerHunter.m_eitrCost = _Plugin.config("Ranger - Hunter", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerHunter.m_cap = _Plugin.config("Ranger - Hunter", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        rangerHunter.m_effectsEnabled = _Plugin.config("Ranger - Hunter", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent luckyShot = new Talent("LuckyShot", "$button_ranger_talent_2", TalentType.Passive);
        luckyShot.m_sprite = SpriteManager.LuckyShot_Icon;
        luckyShot.m_cost = _Plugin.config("Ranger - Lucky Shot", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        luckyShot.m_values = new Talent.TalentValues()
        {
            m_chance = _Plugin.config("Ranger - Lucky Shot", "Chance", 10f, new ConfigDescription("Set the chance to not consume projectile", new AcceptableValueRange<float>(0f, 100f)))
        };
        luckyShot.AddStatusEffect(ScriptableObject.CreateInstance<SE_LuckyShot>(), "SE_LuckyShot");
        luckyShot.m_startEffects = LoadedAssets.SoothEffects;
        luckyShot.m_cap = _Plugin.config("Ranger - Lucky Shot", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent quickShot = new Talent("QuickShot", "$button_ranger_talent_5", TalentType.StatusEffect);
        quickShot.AddStatusEffect(ScriptableObject.CreateInstance<SE_QuickShot>(), "SE_QuickShot");
        quickShot.m_cooldown = _Plugin.config("Ranger - Quick Shot", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        quickShot.m_cost = _Plugin.config("Ranger - Quick Shot", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        quickShot.m_values = new Talent.TalentValues()
        {
            m_speed = _Plugin.config("Ranger - Quick Shot", "Draw Speed Modifier", 1.05f, new ConfigDescription("Set the draw speed multiplier", new AcceptableValueRange<float>(1f, 2f)))
        };
        quickShot.m_length = _Plugin.config("Ranger - Quick Shot", "Length", 15f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(0f, 1000f)));
        quickShot.m_sprite = SpriteManager.QuickShot_Icon;
        quickShot.m_startEffects = LoadedAssets.SoothEffects;
        quickShot.m_animation = "roar";
        quickShot.m_useAnimation = _Plugin.config("Ranger - Quick Shot", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        quickShot.m_healthCost = _Plugin.config("Ranger - Quick Shot", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        quickShot.m_staminaCost = _Plugin.config("Ranger - Quick Shot", "Stamina Cost", 25f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        quickShot.m_eitrCost = _Plugin.config("Ranger - Quick Shot", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        quickShot.m_cap = _Plugin.config("Ranger - Quick Shot", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        quickShot.m_effectsEnabled = _Plugin.config("Ranger - Quick Shot", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent rangerTrap = new Talent("RangerTrap", "$button_ranger_talent_4", TalentType.Ability);
        rangerTrap.m_ability = () =>
        {
            HitData.DamageTypes damages = rangerTrap.GetDamages(rangerTrap.GetLevel());
            RangerTrap.TriggerSpawnTrap(damages, rangerTrap.GetLength(rangerTrap.GetLevel()));
            return true;
        };
        rangerTrap.m_cooldown = _Plugin.config("Ranger - Trap", "Cooldown", 105f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rangerTrap.m_cost = _Plugin.config("Ranger - Trap", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rangerTrap.m_damages = new Talent.TalentDamages()
        {
            m_pierce = _Plugin.config("Ranger - Trap", "Pierce Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_poison = _Plugin.config("Ranger - Trap", "Poison Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        rangerTrap.m_length = _Plugin.config("Ranger - Trap", "Length", 75f, new ConfigDescription("Set the length until de-spawn", new AcceptableValueRange<float>(1f, 1000f)));
        rangerTrap.m_sprite = SpriteManager.RangerTrap_Icon;
        rangerTrap.m_animation = "SetTrap";
        rangerTrap.m_useAnimation = _Plugin.config("Ranger - Trap", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rangerTrap.m_healthCost = _Plugin.config("Ranger - Trap", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerTrap.m_staminaCost = _Plugin.config("Ranger - Trap", "Stamina Cost", 20f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerTrap.m_eitrCost = _Plugin.config("Ranger - Trap", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rangerTrap.m_cap = _Plugin.config("Ranger - Trap", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
    }
    private static void LoadSage()
    {
        Talent sage1 = new Talent("Sage1", "$button_sage_1", TalentType.Characteristic);
        sage1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent sage2 = new Talent("Sage2", "$button_sage_2", TalentType.Characteristic);
        sage2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent sage3 = new Talent("Sage3", "$button_sage_3", TalentType.Characteristic);
        sage3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };

        Talent sage4 = new Talent("Sage4", "$button_sage_4", TalentType.Characteristic);
        sage4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };

        Talent sage5 = new Talent("Sage5", "$button_sage_5", TalentType.Characteristic);
        sage5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };

        Talent sage6 = new Talent("Sage6", "$button_sage_6", TalentType.Characteristic);
        sage6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };

        Talent sage7 = new Talent("Sage7", "$button_sage_7", TalentType.Characteristic);
        sage7.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };
        Talent callOfLightning = new Talent("CallOfLightning", "$button_sage_talent_4", TalentType.Ability);
        callOfLightning.m_ability = () => CallOfLightning.TriggerLightningAOE(callOfLightning);
        callOfLightning.m_effectsEnabled = _Plugin.config("Sage - Lightning", "Start Effects", Toggle.On, "If on, electric effects will show up around player during spell");
        callOfLightning.m_cooldown = _Plugin.config("Sage - Lightning", "Cooldown", 90f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        callOfLightning.m_cost = _Plugin.config("Sage - Lightning", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        callOfLightning.m_damages = new Talent.TalentDamages()
        {
            m_pierce = _Plugin.config("Sage - Lightning", "Pierce Damage", 5f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_lightning = _Plugin.config("Sage - Lightning", "Lightning Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        callOfLightning.m_sprite = SpriteManager.LightningStrike_Icon;
        callOfLightning.m_animation = "LightningStrike";
        callOfLightning.m_useAnimation = _Plugin.config("Sage - Lightning", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        callOfLightning.m_healthCost = _Plugin.config("Sage - Lightning", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        callOfLightning.m_staminaCost = _Plugin.config("Sage - Lightning", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        callOfLightning.m_eitrCost = _Plugin.config("Sage - Lightning", "Eitr Cost", 20f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        callOfLightning.m_cap = _Plugin.config("Sage - Lightning", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent meteorStrike = new Talent("MeteorStrike", "$button_sage_talent_3", TalentType.Ability);
        meteorStrike.m_ability = () =>
        {
            HitData.DamageTypes damages = meteorStrike.GetDamages(meteorStrike.GetLevel());
            MeteorStrike.TriggerMeteor(damages);
            return true;
        };
        meteorStrike.m_cooldown = _Plugin.config("Sage - Meteor", "Cooldown", 100f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        meteorStrike.m_cost = _Plugin.config("Sage - Meteor", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        meteorStrike.m_damages = new Talent.TalentDamages()
        {
            m_blunt = _Plugin.config("Sage - Meteor", "Blunt Damage", 60f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_fire = _Plugin.config("Sage - Meteor", "Fire Damage", 30f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        meteorStrike.m_sprite = SpriteManager.MeteorStrike_Icon;
        meteorStrike.m_animation = "MeteorStrike";
        meteorStrike.m_useAnimation = _Plugin.config("Sage - Meteor", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        meteorStrike.m_healthCost = _Plugin.config("Sage - Meteor", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        meteorStrike.m_staminaCost = _Plugin.config("Sage - Meteor", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        meteorStrike.m_eitrCost = _Plugin.config("Sage - Meteor", "Eitr Cost", 25f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        meteorStrike.m_cap = _Plugin.config("Sage - Meteor", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent stoneThrow = new Talent("StoneThrow", "$button_sage_talent_1", TalentType.Ability);
        stoneThrow.m_ability = () =>
        {
            HitData.DamageTypes damages = stoneThrow.GetDamages(stoneThrow.GetLevel());
            StoneThrow.TriggerStoneThrow(damages);
            return true;
        };
        stoneThrow.m_cooldown = _Plugin.config("Sage - Stone", "Cooldown", 60f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        stoneThrow.m_cost = _Plugin.config("Sage - Stone", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        stoneThrow.m_damages = new Talent.TalentDamages()
        {
            m_blunt = _Plugin.config("Sage - Stone", "Blunt Damage", 15f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        stoneThrow.m_sprite = SpriteManager.BoulderStrike_Icon;
        stoneThrow.m_animation = "StoneThrow";
        stoneThrow.m_useAnimation = _Plugin.config("Sage - Stone", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        stoneThrow.m_healthCost = _Plugin.config("Sage - Stone", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        stoneThrow.m_staminaCost = _Plugin.config("Sage - Stone", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        stoneThrow.m_eitrCost = _Plugin.config("Sage - Stone", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        stoneThrow.m_cap = _Plugin.config("Sage - Stone", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent goblinBeam = new Talent("GoblinBeam", "$button_sage_talent_2", TalentType.Ability);
        goblinBeam.m_ability = () =>
        {
            HitData.DamageTypes damages = goblinBeam.GetDamages(goblinBeam.GetLevel());
            GoblinBeam.TriggerGoblinBeam(damages);
            return true;
        };
        goblinBeam.m_cooldown = _Plugin.config("Sage - Beam", "Cooldown", 105f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        goblinBeam.m_cost = _Plugin.config("Sage - Beam", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        goblinBeam.m_damages = new Talent.TalentDamages()
        {
            m_blunt = _Plugin.config("Sage - Beam", "Blunt Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_fire = _Plugin.config("Sage - Beam", "Fire Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_spirit = _Plugin.config("Sage - Beam", "Spirit Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        goblinBeam.m_sprite = SpriteManager.GoblinBeam_Icon;
        goblinBeam.m_animation = "roar";
        goblinBeam.m_useAnimation = _Plugin.config("Sage - Beam", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        goblinBeam.m_healthCost = _Plugin.config("Sage - Beam", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        goblinBeam.m_staminaCost = _Plugin.config("Sage - Beam", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        goblinBeam.m_eitrCost = _Plugin.config("Sage - Beam", "Eitr Cost", 25f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        goblinBeam.m_cap = _Plugin.config("Sage - Beam", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        Talent iceBreath = new Talent("IceBreath", "$button_sage_talent_5", TalentType.Ability);
        iceBreath.m_ability = () =>
        {
            HitData.DamageTypes damages = iceBreath.GetDamages(iceBreath.GetLevel());

            IceBreath.TriggerIceBreath(damages);
            return true;
        };
        iceBreath.m_cooldown = _Plugin.config("Sage - Ice Breath", "Cooldown", 105f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        iceBreath.m_cost = _Plugin.config("Sage - Ice Breath", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        iceBreath.m_damages = new Talent.TalentDamages()
        {
            m_slash = _Plugin.config("Sage - Ice Breath", "Slash Damage", 20f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_frost = _Plugin.config("Sage - Ice Breath", "Frost Damage", 20f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
        };
        iceBreath.m_sprite = SpriteManager.Blink_Icon;
        iceBreath.m_animation = "roar";
        iceBreath.m_useAnimation = _Plugin.config("Sage - Ice Breath", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        iceBreath.m_healthCost = _Plugin.config("Sage - Ice Breath", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        iceBreath.m_staminaCost = _Plugin.config("Sage - Ice Breath", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        iceBreath.m_eitrCost = _Plugin.config("Sage - Ice Breath", "Eitr Cost", 30f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        iceBreath.m_cap = _Plugin.config("Sage - Ice Breath", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
    }
    private static void LoadShaman()
    {
        Talent shaman1 = new Talent("Shaman1", "$button_shaman_1", TalentType.Characteristic);
        shaman1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent shaman2 = new Talent("Shaman2", "$button_shaman_2", TalentType.Characteristic);
        shaman2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent shaman3 = new Talent("Shaman3", "$button_shaman_3", TalentType.Characteristic);
        shaman3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };

        Talent shaman4 = new Talent("Shaman4", "$button_shaman_4", TalentType.Characteristic);
        shaman4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };

        Talent shaman5 = new Talent("Shaman5", "$button_shaman_5", TalentType.Characteristic);
        shaman5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent shaman6 = new Talent("Shaman6", "$button_shaman_6", TalentType.Characteristic);
        shaman6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };
        Talent shamanHeal = new Talent("ShamanHeal", "$button_shaman_talent_1", TalentType.Ability);
        shamanHeal.m_ability = () => ShamanHeal.TriggerHeal(shamanHeal.GetHealAmount(shamanHeal.GetLevel()));
        shamanHeal.m_cooldown = _Plugin.config("Shaman - Heal", "Cooldown", 70f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        shamanHeal.m_cost = _Plugin.config("Shaman - Heal", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        shamanHeal.m_values = new Talent.TalentValues()
        {
            m_heal = _Plugin.config("Shaman - Heal", "Heal", 25f, new ConfigDescription("Set heal", new AcceptableValueRange<float>(0f, 1000f)))
        };
        shamanHeal.m_sprite = SpriteManager.ShamanHeal_Icon;
        shamanHeal.m_animation = "Heal";
        shamanHeal.m_useAnimation = _Plugin.config("Shaman - Heal", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        shamanHeal.m_healthCost = _Plugin.config("Shaman - Heal", "Health Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanHeal.m_staminaCost = _Plugin.config("Shaman - Heal", "Stamina Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanHeal.m_eitrCost = _Plugin.config("Shaman - Heal", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanHeal.m_cap = _Plugin.config("Shaman - Heal", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent shamanShield = new Talent("ShamanShield", "$button_shaman_talent_5", TalentType.StatusEffect);
        shamanShield.AddStatusEffect(ScriptableObject.CreateInstance<SE_ShamanShield>(), "SE_ShamanShield");
        shamanShield.m_cooldown = _Plugin.config("Shaman - Shield", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        shamanShield.m_cost = _Plugin.config("Shaman - Shield", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        shamanShield.m_values = new Talent.TalentValues()
        {
            m_absorb = _Plugin.config("Shaman - Shield", "Absorb", 50f, new ConfigDescription("Set amount absorbed by shield", new AcceptableValueRange<float>(0f, 1000f)))
        };
        shamanShield.m_length = _Plugin.config("Shaman - Shield", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        shamanShield.m_sprite = SpriteManager.ShamanProtection_Icon;
        shamanShield.m_startEffects = LoadedAssets.FX_DvergerPower;
        shamanShield.m_animation = "LightningStrike";
        shamanShield.m_useAnimation = _Plugin.config("Shaman - Shield", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        shamanShield.m_healthCost = _Plugin.config("Shaman - Shield", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanShield.m_staminaCost = _Plugin.config("Shaman - Shield", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanShield.m_eitrCost = _Plugin.config("Shaman - Shield", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanShield.m_cap = _Plugin.config("Shaman - Shield", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        shamanShield.m_effectsEnabled = _Plugin.config("Shaman - Shield", "Effects Enabled", Toggle.On, "If on, start effects are enabled");
        Talent shamanRegeneration = new Talent("ShamanRegeneration", "$button_shaman_talent_4", TalentType.StatusEffect);
        shamanRegeneration.AddStatusEffect(ScriptableObject.CreateInstance<SE_ShamanRegeneration>(), "SE_ShamanRegeneration");
        shamanRegeneration.m_cooldown = _Plugin.config("Shaman - Regeneration", "Cooldown", 90f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        shamanRegeneration.m_cost = _Plugin.config("Shaman - Regeneration", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        shamanRegeneration.m_values = new Talent.TalentValues()
        {
            m_eitrRegen = _Plugin.config("Shaman - Regeneration", "Eitr Regeneration", 1.1f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(0f, 10f))),
            m_staminaRegen = _Plugin.config("Shaman - Regeneration", "Stamina Regeneration", 1.1f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(0f, 10f)))
        };
        shamanRegeneration.m_length = _Plugin.config("Shaman - Regeneration", "Length", 30f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        shamanRegeneration.m_sprite = SpriteManager.ShamanRegeneration;
        shamanRegeneration.m_startEffects = LoadedAssets.UnSummonEffects;
        shamanRegeneration.m_animation = "cheer";
        shamanRegeneration.m_useAnimation = _Plugin.config("Shaman - Regeneration", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        shamanRegeneration.m_healthCost = _Plugin.config("Shaman - Regeneration", "Health Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanRegeneration.m_staminaCost = _Plugin.config("Shaman - Regeneration", "Stamina Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanRegeneration.m_eitrCost = _Plugin.config("Shaman - Regeneration", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanRegeneration.m_cap = _Plugin.config("Shaman - Regeneration", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        shamanRegeneration.m_effectsEnabled = _Plugin.config("Shaman - Regeneration", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent shamanSpawn = new Talent("ShamanSpawn", "$button_shaman_talent_3", TalentType.Ability);
        shamanSpawn.m_ability = () =>
        {
            if (ZNetScene.instance.GetPrefab("Ghost") is not { } fallback) return false;
            ShamanSpawn.TriggerShamanSpawn(fallback, shamanSpawn);
            return true;
        };
        shamanSpawn.m_cooldown = _Plugin.config("Shaman - Summon", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        shamanSpawn.m_cost = _Plugin.config("Shaman - Summon", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        shamanSpawn.m_creaturesByLevel = new Talent.CreaturesByLevel()
        {
            m_oneToThree = _Plugin.config("Shaman - Summon", "One To Three", "Ghost", "Set creature spawn for talent level one to three"),
            m_fourToSix = _Plugin.config("Shaman - Summon", "Three To Six", "Wraith", "Set creature spawn for talent level four to six"),
            m_sevenToNine = _Plugin.config("Shaman - Summon", "Four To Nine", "BlobTar", "Set creature spawn for talent level six to nine"),
            m_ten = _Plugin.config("Shaman - Summon", "Ten Above", "FallenValkyrie", "Set creature spawn for talent level ten and beyond")
        };
        shamanSpawn.m_length = _Plugin.config("Shaman - Summon", "Length", 25f, new ConfigDescription("Set the length of time until de-spawn", new AcceptableValueRange<float>(1f, 1000f)));
        shamanSpawn.m_sprite = SpriteManager.ShamanGhosts_Icon;
        shamanSpawn.m_animation = "Summon";
        shamanSpawn.m_useAnimation = _Plugin.config("Shaman - Summon", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        shamanSpawn.m_healthCost = _Plugin.config("Shaman - Summon", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanSpawn.m_staminaCost = _Plugin.config("Shaman - Summon", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanSpawn.m_eitrCost = _Plugin.config("Shaman - Summon", "Eitr Cost", 25f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        shamanSpawn.m_cap = _Plugin.config("Shaman - Summon", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));

        Talent rootBeam = new Talent("RootBeam", "$button_shaman_talent_2", TalentType.Ability);
        rootBeam.m_ability = () =>
        {
            HitData.DamageTypes damages = rootBeam.GetDamages(rootBeam.GetLevel());
            RootBeam.TriggerRootBeam(damages);
            return true;
        };
        rootBeam.m_cooldown = _Plugin.config("Shaman - Roots", "Cooldown", 90f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rootBeam.m_cost = _Plugin.config("Shaman - Roots", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rootBeam.m_damages = new Talent.TalentDamages()
        {
            m_blunt = _Plugin.config("Shaman - Roots", "Blunt Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_poison = _Plugin.config("Shaman - Roots", "Poison Damage", 10f, new ConfigDescription("Set damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        rootBeam.m_sprite = SpriteManager.ShamanRoots_Icon;
        rootBeam.m_animation = "roar";
        rootBeam.m_useAnimation = _Plugin.config("Shaman - Roots", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rootBeam.m_healthCost = _Plugin.config("Shaman - Roots", "Health Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rootBeam.m_staminaCost = _Plugin.config("Shaman - Roots", "Stamina Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rootBeam.m_eitrCost = _Plugin.config("Shaman - Roots", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rootBeam.m_cap = _Plugin.config("Shaman - Roots", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
    }
    private static void LoadBard()
    {
        Talent bard1 = new Talent("Bard1", "$button_bard_1", TalentType.Characteristic);
        bard1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent bard2 = new Talent("Bard2", "$button_bard_2", TalentType.Characteristic);
        bard2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };

        Talent bard3 = new Talent("Bard3", "$button_bard_3", TalentType.Characteristic);
        bard3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Intelligence,
            m_amount = 10
        };

        Talent bard4 = new Talent("Bard4", "$button_bard_4", TalentType.Characteristic);
        bard4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent bard5 = new Talent("Bard5", "$button_bard_5", TalentType.Characteristic);
        bard5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent bard6 = new Talent("Bard6", "$button_bard_6", TalentType.Characteristic);
        bard6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };
        Talent songOfDamage = new Talent("SongOfDamage", "$button_bard_talent_3", TalentType.StatusEffect);
        songOfDamage.AddStatusEffect(ScriptableObject.CreateInstance<SE_SongOfDamage>(), "SE_SongOfDamage");
        songOfDamage.m_cooldown = _Plugin.config("Bard - Song of Damage", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        songOfDamage.m_cost = _Plugin.config("Bard - Song of Damage", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        songOfDamage.m_values = new Talent.TalentValues()
        {
            m_modifyAttack = _Plugin.config("Bard - Song of Damage", "Attack Increase", 1.05f, new ConfigDescription("Set the attack multiplier", new AcceptableValueRange<float>(0f, 10f)))
        };
        songOfDamage.m_length = _Plugin.config("Bard - Song of Damage", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        songOfDamage.m_sprite = SpriteManager.SongOfDamage_Icon;
        songOfDamage.m_startEffects = LoadedAssets.AddBardFX(Color.red, "FX_Bard_Music_Red", true);
        songOfDamage.m_animation = "dance";
        songOfDamage.m_useAnimation = _Plugin.config("Bard - Song of Damage", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        songOfDamage.m_healthCost = _Plugin.config("Bard - Song of Damage", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfDamage.m_staminaCost = _Plugin.config("Bard - Song of Damage", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfDamage.m_eitrCost = _Plugin.config("Bard - Song of Damage", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfDamage.m_cap = _Plugin.config("Bard - Song of Damage", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        songOfDamage.m_effectsEnabled = _Plugin.config("Bard - Song of Damage", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent songOfHealing = new Talent("SongOfHealing", "$button_bard_talent_4", TalentType.StatusEffect);
        songOfHealing.AddStatusEffect(ScriptableObject.CreateInstance<SE_SongOfHealing>(), "SE_SongOfHealing");
        songOfHealing.m_cooldown = _Plugin.config("Bard - Song of Healing", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        songOfHealing.m_cost = _Plugin.config("Bard - Song of Healing", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        songOfHealing.m_values = new Talent.TalentValues()
        {
            m_heal = _Plugin.config("Bard - Song of Healing", "Heal", 5f, new ConfigDescription("Set the amount healed per tick", new AcceptableValueRange<float>(0f, 101f)))
        };
        songOfHealing.m_length = _Plugin.config("Bard - Song of Healing", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        songOfHealing.m_sprite = SpriteManager.SongOfHealing_Icon;
        songOfHealing.m_startEffects = LoadedAssets.AddBardFX(Color.yellow, "FX_Bard_Music_Yellow");
        songOfHealing.m_animation = "dance";
        songOfHealing.m_useAnimation = _Plugin.config("Bard - Song of Healing", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        songOfHealing.m_healthCost = _Plugin.config("Bard - Song of Healing", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfHealing.m_staminaCost = _Plugin.config("Bard - Song of Healing", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfHealing.m_eitrCost = _Plugin.config("Bard - Song of Healing", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfHealing.m_cap = _Plugin.config("Bard - Song of Healing", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        songOfHealing.m_effectsEnabled = _Plugin.config("Bard - Song of Healing", "Effects Enabled", Toggle.On, "If on, start effects are enabled");
        Talent songOfVitality = new Talent("SongOfVitality", "$button_bard_talent_2", TalentType.StatusEffect);
        songOfVitality.AddStatusEffect(ScriptableObject.CreateInstance<SE_SongOfVitality>(), "SE_SongOfVitality");
        songOfVitality.m_cooldown = _Plugin.config("Bard - Song of Vitality", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        songOfVitality.m_cost = _Plugin.config("Bard - Song of Vitality", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        songOfVitality.m_values = new Talent.TalentValues()
        {
            m_health = _Plugin.config("Bard - Song of Vitality", "Health", 10f, new ConfigDescription("Set the health gained from effect", new AcceptableValueRange<float>(0f, 100f)))
        };
        songOfVitality.m_length = _Plugin.config("Bard - Song of Vitality", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        songOfVitality.m_sprite = SpriteManager.SongOfVitality_Icon;
        songOfVitality.m_startEffects = LoadedAssets.AddBardFX(Color.blue, "FX_Bard_Music_Blue");
        songOfVitality.m_animation = "dance";
        songOfVitality.m_useAnimation = _Plugin.config("Bard - Song of Vitality", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        songOfVitality.m_healthCost = _Plugin.config("Bard - Song of Vitality", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfVitality.m_staminaCost = _Plugin.config("Bard - Song of Vitality", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfVitality.m_eitrCost = _Plugin.config("Bard - Song of Vitality", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfVitality.m_cap = _Plugin.config("Bard - Song of Vitality", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        songOfVitality.m_effectsEnabled = _Plugin.config("Bard - Song of Vitality", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent songOfSpeed = new Talent("SongOfSpeed", "$button_bard_talent_1", TalentType.StatusEffect);
        songOfSpeed.AddStatusEffect(ScriptableObject.CreateInstance<SE_SongOfSpeed>(), "SE_SongOfSpeed");
        songOfSpeed.m_cooldown = _Plugin.config("Bard - Song of Speed", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        songOfSpeed.m_cost = _Plugin.config("Bard - Song of Speed", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        songOfSpeed.m_values = new Talent.TalentValues()
        {
            m_speed = _Plugin.config("Bard - Song of Speed", "Speed", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(1f, 2f)))
        };
        songOfSpeed.m_length = _Plugin.config("Bard - Song of Speed", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        songOfSpeed.m_sprite = SpriteManager.SongOfSpeed_Icon;
        songOfSpeed.m_startEffects = LoadedAssets.AddBardFX(Color.green, "FX_Bard_Music_Green");
        songOfSpeed.m_animation = "dance";
        songOfSpeed.m_useAnimation = _Plugin.config("Bard - Song of Speed", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        songOfSpeed.m_healthCost = _Plugin.config("Bard - Song of Speed", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfSpeed.m_staminaCost = _Plugin.config("Bard - Song of Speed", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfSpeed.m_eitrCost = _Plugin.config("Bard - Song of Speed", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfSpeed.m_cap = _Plugin.config("Bard - Song of Speed", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        songOfSpeed.m_effectsEnabled = _Plugin.config("Bard - Song of Speed", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent songOfAttrition = new Talent("SongOfAttrition", "$button_bard_talent_5", TalentType.StatusEffect);
        songOfAttrition.AddStatusEffect(ScriptableObject.CreateInstance<SE_SongOfAttrition>(), "SE_SongOfAttrition");
        songOfAttrition.m_cooldown = _Plugin.config("Bard - Song of Attrition", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        songOfAttrition.m_cost = _Plugin.config("Bard - Song of Attrition", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        songOfAttrition.m_damages = new Talent.TalentDamages()
        {
            m_spirit = _Plugin.config("Bard - Song of Attrition", "Spirit Damage", 10f, new ConfigDescription("Set the damages", new AcceptableValueRange<float>(0f, 1000f))),
            m_slash = _Plugin.config("Bard - Song of Attrition", "Slash Damage", 1f, new ConfigDescription("Set the damages", new AcceptableValueRange<float>(0f, 1000f)))
        };
        songOfAttrition.m_length = _Plugin.config("Bard - Song of Attrition", "Length", 10f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        songOfAttrition.m_sprite = SpriteManager.SongOfSpirit_Icon;
        songOfAttrition.m_startEffects = LoadedAssets.VFX_SongOfSpirit;
        songOfAttrition.m_animation = "dance";
        songOfAttrition.m_useAnimation = _Plugin.config("Bard - Song of Attrition", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        songOfAttrition.m_healthCost = _Plugin.config("Bard - Song of Attrition", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfAttrition.m_staminaCost = _Plugin.config("Bard - Song of Attrition", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfAttrition.m_eitrCost = _Plugin.config("Bard - Song of Attrition", "Eitr Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        songOfAttrition.m_cap = _Plugin.config("Bard - Song of Attrition", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        songOfAttrition.m_effectsEnabled = _Plugin.config("Bard - Song of Attrition", "Effects Enabled", Toggle.On, "If on, start effects are enabled");
    }

    private static void LoadRogue()
    {
        Talent rogue1 = new Talent("Rogue1", "$button_rogue_1", TalentType.Characteristic);
        rogue1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Wisdom,
            m_amount = 10
        };

        Talent rogue2 = new Talent("Rogue2", "$button_rogue_2", TalentType.Characteristic);
        rogue2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent rogue3 = new Talent("Rogue3", "$button_rogue_3", TalentType.Characteristic);
        rogue3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10
        };

        Talent rogue4 = new Talent("Rogue4", "$button_rogue_4", TalentType.Characteristic);
        rogue4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10
        };

        Talent rogue5 = new Talent("Rogue5", "$button_rogue_5", TalentType.Characteristic);
        rogue5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };

        Talent rogue6 = new Talent("Rogue6", "$button_rogue_6", TalentType.Characteristic);
        rogue6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10
        };
        Talent rogueSpeed = new Talent("RogueSpeed", "$button_rogue_talent_1", TalentType.StatusEffect);
        rogueSpeed.AddStatusEffect(ScriptableObject.CreateInstance<SE_RogueSpeed>(), "SE_RogueSpeed");
        rogueSpeed.m_cooldown = _Plugin.config("Rogue - Quick Step", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rogueSpeed.m_cost = _Plugin.config("Rogue - Quick Step", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rogueSpeed.m_values = new Talent.TalentValues()
        {
            m_speed = _Plugin.config("Rogue - Quick Step", "Speed", 1.05f, new ConfigDescription("Set the speed multiplier", new AcceptableValueRange<float>(1f, 2f))),
            m_runStaminaDrain = _Plugin.config("Rogue - Quick Step", "Run Stamina Drain", 0.1f, new ConfigDescription("Set the drain modifier", new AcceptableValueRange<float>(0f, 1f))),
        };
        rogueSpeed.m_length = _Plugin.config("Rogue - Quick Step", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        rogueSpeed.m_sprite = SpriteManager.QuickStep_Icon;
        rogueSpeed.m_startEffects = LoadedAssets.FX_DvergerPower;
        rogueSpeed.m_animation = "flex";
        rogueSpeed.m_useAnimation = _Plugin.config("Rogue - Quick Step", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rogueSpeed.m_healthCost = _Plugin.config("Rogue - Quick Step", "Health Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueSpeed.m_staminaCost = _Plugin.config("Rogue - Quick Step", "Stamina Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueSpeed.m_eitrCost = _Plugin.config("Rogue - Quick Step", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueSpeed.m_cap = _Plugin.config("Rogue - Quick Step", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        rogueSpeed.m_effectsEnabled = _Plugin.config("Rogue - Quick Step", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent rogueStamina = new Talent("RogueStamina", "$button_rogue_talent_4", TalentType.StatusEffect);
        rogueStamina.AddStatusEffect(ScriptableObject.CreateInstance<SE_RogueStamina>(), "SE_RogueStamina");
        rogueStamina.m_cooldown = _Plugin.config("Rogue - Swift", "Cooldown", 135f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rogueStamina.m_cost = _Plugin.config("Rogue - Swift", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rogueStamina.m_values = new Talent.TalentValues()
        {
            m_staminaRegen = _Plugin.config("Rogue - Swift", "Stamina Regeneration", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(1f, 2f))),
            m_stamina = _Plugin.config("Rogue - Swift", "Stamina", 10f, new ConfigDescription("Set the amount", new AcceptableValueRange<float>(0f, 100f))),
            m_attackStaminaUsage = _Plugin.config("Rogue - Swift", "Attack Stamina Usage", 0.1f, new ConfigDescription("Set the modifier", new AcceptableValueRange<float>(0f, 1f))),
            m_sneakStaminaUsage = _Plugin.config("Rogue - Swift", "Sneak Stamina Usage", 0.2f, new ConfigDescription("Set the modifier", new AcceptableValueRange<float>(0f, 1f)))
        };
        rogueStamina.m_length = _Plugin.config("Rogue - Swift", "Length", 25f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        rogueStamina.m_sprite = SpriteManager.Relentless_Icon;
        rogueStamina.m_startEffects = LoadedAssets.FX_DvergerPower;
        rogueStamina.m_animation = "flex";
        rogueStamina.m_useAnimation = _Plugin.config("Rogue - Swift", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rogueStamina.m_healthCost = _Plugin.config("Rogue - Swift", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueStamina.m_staminaCost = _Plugin.config("Rogue - Swift", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueStamina.m_eitrCost = _Plugin.config("Rogue - Swift", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueStamina.m_cap = _Plugin.config("Rogue - Swift", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        rogueStamina.m_effectsEnabled = _Plugin.config("Rogue - Swift", "Effects Enabled", Toggle.On, "If on, start effects are enabled");
        Talent rogueReflect = new Talent("RogueReflect", "$button_rogue_talent_2", TalentType.StatusEffect);
        rogueReflect.AddStatusEffect(ScriptableObject.CreateInstance<SE_RogueReflect>(), "SE_RogueReflect");
        rogueReflect.m_cooldown = _Plugin.config("Rogue - Retaliation", "Cooldown", 155f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rogueReflect.m_cost = _Plugin.config("Rogue - Retaliation", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rogueReflect.m_values = new Talent.TalentValues()
        {
            m_reflect = _Plugin.config("Rogue - Retaliation", "Reflect", 0.05f, new ConfigDescription("Set the amount reflected back", new AcceptableValueRange<float>(0f, 1f)))
        };
        rogueReflect.m_length = _Plugin.config("Rogue - Retaliation", "Length", 15f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        rogueReflect.m_sprite = SpriteManager.Reflect_Icon;
        rogueReflect.m_startEffects = LoadedAssets.FX_DvergerPower;
        rogueReflect.m_animation = "flex";
        rogueReflect.m_useAnimation = _Plugin.config("Rogue Retaliation", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rogueReflect.m_healthCost = _Plugin.config("Rogue - Retaliation", "Health Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueReflect.m_staminaCost = _Plugin.config("Rogue - Retaliation", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueReflect.m_eitrCost = _Plugin.config("Rogue - Retaliation", "Eitr Cost", 5f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueReflect.m_cap = _Plugin.config("Rogue - Retaliation", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        rogueReflect.m_effectsEnabled = _Plugin.config("Rogue - Retaliation", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent rogueBackstab = new Talent("RogueBackstab", "$button_rogue_talent_3", TalentType.StatusEffect);
        rogueBackstab.AddStatusEffect(ScriptableObject.CreateInstance<SE_RogueBackstab>(), "SE_RogueBackstab");
        rogueBackstab.m_cooldown = _Plugin.config("Rogue - Backstab", "Cooldown", 135f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rogueBackstab.m_cost = _Plugin.config("Rogue - Backstab", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rogueBackstab.m_values = new Talent.TalentValues()
        {
            m_chance = _Plugin.config("Rogue - Backstab", "Chance", 1.5f, new ConfigDescription("Set the chance to backstab", new AcceptableValueRange<float>(0f, 100f)))
        };
        rogueBackstab.m_length = _Plugin.config("Rogue - Backstab", "Length", 45f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        rogueBackstab.m_sprite = SpriteManager.Backstab_Icon;
        rogueBackstab.m_startEffects = LoadedAssets.FX_DvergerPower;
        rogueBackstab.m_animation = "flex";
        rogueBackstab.m_useAnimation = _Plugin.config("Rogue - Backstab", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rogueBackstab.m_healthCost = _Plugin.config("Rogue - Backstab", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueBackstab.m_staminaCost = _Plugin.config("Rogue - Backstab", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueBackstab.m_eitrCost = _Plugin.config("Rogue - Backstab", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueBackstab.m_cap = _Plugin.config("Rogue - Backstab", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        rogueBackstab.m_effectsEnabled = _Plugin.config("Rogue - Backstab", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent rogueBleed = new Talent("RogueBleed", "$button_rogue_talent_5", TalentType.StatusEffect);
        rogueBleed.AddStatusEffect(ScriptableObject.CreateInstance<SE_RogueBleed>(), "SE_RogueBleed");
        rogueBleed.m_cooldown = _Plugin.config("Rogue - Bleed", "Cooldown", 135f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        rogueBleed.m_cost = _Plugin.config("Rogue - Bleed", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        rogueBleed.m_values = new Talent.TalentValues()
        {
            m_bleed = _Plugin.config("Rogue - Bleed", "Damage Per Tick", 1f, new ConfigDescription("Set the damage per tick, stackable", new AcceptableValueRange<float>(1f, 10f)))
        };
        rogueBleed.m_length = _Plugin.config("Rogue - Bleed", "Length", 15f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        rogueBleed.m_sprite = SpriteManager.Bleeding_Icon;
        rogueBleed.m_startEffects = LoadedAssets.FX_RogueBleed;
        rogueBleed.m_animation = "flex";
        rogueBleed.m_useAnimation = _Plugin.config("Rogue - Bleed", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        rogueBleed.m_healthCost = _Plugin.config("Rogue - Bleed", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueBleed.m_staminaCost = _Plugin.config("Rogue - Bleed", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueBleed.m_eitrCost = _Plugin.config("Rogue - Bleed", "Eitr Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        rogueBleed.m_cap = _Plugin.config("Rogue - Bleed", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        rogueBleed.m_effectsEnabled = _Plugin.config("Rogue - Bleed", "Effects Enabled", Toggle.On, "If on, start effects are enabled");
    }

    private static void LoadWarrior()
    {
        Talent warrior1 = new Talent("Warrior1", "$button_warrior_1", TalentType.Characteristic);
        warrior1.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10,
        };

        Talent warrior2 = new Talent("Warrior2", "$button_warrior_2", TalentType.Characteristic);
        warrior2.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10,
        };

        Talent warrior3 = new Talent("Warrior3", "$button_warrior_3", TalentType.Characteristic);
        warrior3.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Dexterity,
            m_amount = 10,
        };

        Talent warrior4 = new Talent("Warrior4", "$button_warrior_4", TalentType.Characteristic);
        warrior4.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10,
        };

        Talent warrior5 = new Talent("Warrior5", "$button_warrior_5", TalentType.Characteristic);
        warrior5.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Strength,
            m_amount = 10,
        };

        Talent warrior6 = new Talent("Warrior6", "$button_warrior_6", TalentType.Characteristic);
        warrior6.m_characteristic = new Talent.TalentCharacteristics()
        {
            m_type = Characteristic.Constitution,
            m_amount = 10,
        };
        Talent warriorStrength = new Talent("WarriorStrength", "$button_warrior_talent_1", TalentType.StatusEffect);
        warriorStrength.AddStatusEffect(ScriptableObject.CreateInstance<SE_WarriorStrength>(), "SE_WarriorStrength");
        warriorStrength.m_cooldown = _Plugin.config("Warrior - Power", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        warriorStrength.m_cost = _Plugin.config("Warrior - Power", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        warriorStrength.m_values = new Talent.TalentValues()
        {
            m_modifyAttack = _Plugin.config("Warrior - Power", "Attack", 1.05f, new ConfigDescription("Set the multiplier", new AcceptableValueRange<float>(1f, 10f)))
        };
        warriorStrength.m_length = _Plugin.config("Warrior - Power", "Length", 60f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        warriorStrength.m_sprite = SpriteManager.HardHitter_Icon;
        warriorStrength.m_startEffects = LoadedAssets.FX_DvergerPower;
        warriorStrength.m_animation = "flex";
        warriorStrength.m_useAnimation = _Plugin.config("Warrior - Power", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        warriorStrength.m_healthCost = _Plugin.config("Warrior - Power", "Health Cost", 15f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorStrength.m_staminaCost = _Plugin.config("Warrior - Power", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorStrength.m_eitrCost = _Plugin.config("Warrior - Power", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorStrength.m_cap = _Plugin.config("Warrior - Power", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        warriorStrength.m_effectsEnabled = _Plugin.config("Warrior - Power", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent warriorVitality = new Talent("WarriorVitality", "$button_warrior_talent_2", TalentType.StatusEffect);
        warriorVitality.AddStatusEffect(ScriptableObject.CreateInstance<SE_WarriorVitality>(), "SE_WarriorVitality");
        warriorVitality.m_cooldown = _Plugin.config("Warrior - Vitality", "Cooldown", 120f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        warriorVitality.m_cost = _Plugin.config("Warrior - Vitality", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        warriorVitality.m_values = new Talent.TalentValues()
        {
            m_health = _Plugin.config("Warrior - Vitality", "Health", 10f, new ConfigDescription("Set the health increase", new AcceptableValueRange<float>(0f, 101f))),
            m_healthRegen = _Plugin.config("Warrior - Vitality", "Health Regeneration", 1.1f, new ConfigDescription("Set health regeneration modifier", new AcceptableValueRange<float>(0f, 101f)))
        };
        warriorVitality.m_length = _Plugin.config("Warrior - Vitality", "Length", 30f, new ConfigDescription("Set the length of effect", new AcceptableValueRange<float>(1f, 1000f)));
        warriorVitality.m_sprite = SpriteManager.BulkUp_Icon;
        warriorVitality.m_startEffects = LoadedAssets.FX_DvergerPower;
        warriorVitality.m_animation = "flex";
        warriorVitality.m_useAnimation = _Plugin.config("Warrior - Vitality", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        warriorVitality.m_healthCost = _Plugin.config("Warrior - Vitality", "Health Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorVitality.m_staminaCost = _Plugin.config("Warrior - Vitality", "Stamina Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorVitality.m_eitrCost = _Plugin.config("Warrior - Vitality", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorVitality.m_cap = _Plugin.config("Warrior - Vitality", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        warriorVitality.m_effectsEnabled = _Plugin.config("Warrior - Vitality", "Effects Enabled", Toggle.On, "If on, start effects are enabled");
        Talent monkeyWrench = new Talent("MonkeyWrench", "$button_warrior_talent_4", TalentType.Passive);
        monkeyWrench.m_sprite = SpriteManager.WarriorIcon;
        monkeyWrench.AddStatusEffect(ScriptableObject.CreateInstance<SE_MonkeyWrench>(), "SE_MonkeyWrench");
        monkeyWrench.m_cost = _Plugin.config("Warrior - Monkey Wrench", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        monkeyWrench.m_values = new Talent.TalentValues()
        {
            m_damageReduction = _Plugin.config("Warrior - Monkey Wrench", "Damage Decrease", 0f, new ConfigDescription("Set the damage reduction", new AcceptableValueRange<float>(0f, 1f))),
            m_attackSpeedReduction = _Plugin.config("Warrior - Monkey Wrench", "Attack Speed Reduction", 0.5f, new ConfigDescription("Set the attack speed reduction modifier", new AcceptableValueRange<float>(0f, 1f)))
        };
        monkeyWrench.m_cap = _Plugin.config("Warrior - Monkey Wrench", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        monkeyWrench.m_passiveActive = true;
        monkeyWrench.m_onClickPassive = () =>
        {
            var passiveIsActive = monkeyWrench.m_passiveActive;
            if (passiveIsActive)
                MonkeyWrench.ResetTwoHandedWeapons();
            else
                MonkeyWrench.ModifyTwoHandedWeapons();
                    
            PlayerManager.RefreshCurrentWeapon();
            monkeyWrench.m_passiveActive = !passiveIsActive;
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"MonkeyWrench was {(monkeyWrench.m_passiveActive ? "activated" : "deactivated")}");
            return true;
        };
        monkeyWrench.m_addToPassiveBar = true;

        Talent warriorResistance = new Talent("WarriorResistance", "$button_warrior_talent_3", TalentType.StatusEffect);
        warriorResistance.AddStatusEffect(ScriptableObject.CreateInstance<SE_WarriorResistance>(), "SE_WarriorResistance");
        warriorResistance.m_cooldown = _Plugin.config("Warrior - Fortification", "Cooldown", 180f, new ConfigDescription("Set the cooldown", new AcceptableValueRange<float>(0f, 1000f)));
        warriorResistance.m_cost = _Plugin.config("Warrior - Fortification", "Purchase Cost", 3, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        warriorResistance.m_resistances = new Talent.ResistancePercentages()
        {
            m_blunt = _Plugin.config("Warrior - Fortification", "Blunt Resistance", 0.9f, new ConfigDescription("Set resistance modifier", new AcceptableValueRange<float>(0f, 1f))),
            m_pierce = _Plugin.config("Warrior - Fortification", "Pierce Resistance", 0.9f, new ConfigDescription("Set resistance modifier", new AcceptableValueRange<float>(0f, 1f))),
            m_slash = _Plugin.config("Warrior - Fortification", "Slash Resistance", 0.9f, new ConfigDescription("Set resistance modifier", new AcceptableValueRange<float>(0f, 1f))),
        };
        warriorResistance.m_length = _Plugin.config("Warrior - Fortification", "Effect Length", 30f, new ConfigDescription("Set the length of the talent", new AcceptableValueRange<float>(0f, 1000f)));
        warriorResistance.m_sprite = SpriteManager.Resistant_Icon;
        warriorResistance.m_startEffects = LoadedAssets.FX_DvergerPower;
        warriorResistance.m_animation = "flex";
        warriorResistance.m_useAnimation = _Plugin.config("Warrior - Fortification", "Use Animation", Toggle.On, "If on, casting ability triggers animation");
        warriorResistance.m_healthCost = _Plugin.config("Warrior - Fortification", "Health Cost", 10f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorResistance.m_staminaCost = _Plugin.config("Warrior - Fortification", "Stamina Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorResistance.m_eitrCost = _Plugin.config("Warrior - Fortification", "Eitr Cost", 0f, new ConfigDescription("Set the cost to trigger talent", new AcceptableValueRange<float>(0f, 101f)));
        warriorResistance.m_cap = _Plugin.config("Warrior - Fortification", "Prestige Cap", 10, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        warriorResistance.m_effectsEnabled = _Plugin.config("Warrior - Fortification", "Effects Enabled", Toggle.On, "If on, start effects are enabled");

        Talent dualWield = new Talent("DualWield", "$button_warrior_talent_5", TalentType.Passive);
        dualWield.m_sprite = SpriteManager.WarriorIcon;
        dualWield.AddStatusEffect(ScriptableObject.CreateInstance<SE_DualWield>(), "SE_DualWield");
        dualWield.m_cost = _Plugin.config("Warrior - Dual Wield", "Purchase Cost", 5, new ConfigDescription("Set the cost to unlock the talent", new AcceptableValueRange<int>(1, 10)));
        dualWield.m_values = new Talent.TalentValues()
        {
            m_damageReduction = _Plugin.config("Warrior - Dual Wield", "Damage Decrease", 0.5f, new ConfigDescription("Set the damage reduction", new AcceptableValueRange<float>(0f, 1f)))
        };
        dualWield.m_cap = _Plugin.config("Warrior - Dual Wield", "Prestige Cap", 5, new ConfigDescription("Set the prestige cap", new AcceptableValueRange<int>(1, 101)));
        dualWield.m_passiveActive = false;
        dualWield.m_addToPassiveBar = true;
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