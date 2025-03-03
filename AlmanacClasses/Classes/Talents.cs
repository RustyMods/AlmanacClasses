using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AlmanacClasses.UI;
using BepInEx.Configuration;
using UnityEngine;
using static AlmanacClasses.AlmanacClassesPlugin;

namespace AlmanacClasses.Classes;

[Description("An ability accessible through the skill tree UI")]
public class Talent
{
    public const string m_prestigeColor = "#FF5733";
    public readonly string m_key;
    public readonly string m_button;
    public readonly StatusEffect? m_status;
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
    public readonly int? m_characteristic;
    public SpellBook.AbilityData? m_abilityData;
    public Action? m_onPurchase;
    public Func<string>? m_tooltip;
    public Func<string>? m_prestigeTooltip;
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
    public EffectList GetEffectList() => UseEffects() ? m_startEffects ?? new EffectList() : new EffectList();
    public Sprite? GetSprite() => m_sprite;
    public int GetCost() => m_cost?.Value ?? _StatsCost.Value;
    public float GetCooldown(int level) => Mathf.Clamp((m_cooldown?.Value ?? 0f) - (level - 1) * 5f, GetLength(level), float.MaxValue);
    public float GetLength(int level) => (m_length?.Value ?? 0f) + (level - 1) * 5f;
    public float GetEitrCost(bool reduces, int level = 1) => reduces ? Mathf.Max((m_eitrCost?.Value ?? 0f) - level * 5f, 0) : m_eitrCost?.Value ?? 0f;
    public float GetStaminaCost() => m_staminaCost?.Value ?? 0f;
    public float GetHealthCost() => m_healthCost?.Value ?? 0f;
    public HitData.DamageTypes GetDamages(int level) => m_damages?.GetDamages(level) ?? new HitData.DamageTypes();
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
    public float GetLeechModifier(int level) => m_values == null ? 0f : Mathf.Clamp01((m_values.m_leech?.Value ?? 0f) + (level - 1) * 0.1f);
    public float GetAddedComfort(int level) => m_values == null ? 0f : (m_values.m_comfort?.Value ?? 0f) * level;
    public float GetDamageReduction(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_damageReduction?.Value ?? 1f) - 0.1f * (level - 1)));
    public float GetFoodModifier(int level) => m_values == null ? 1f : (m_values.m_foodModifier?.Value ?? 1f) * level - (level - 1);
    public float GetForageModifier(int level) => m_values == null ? 1f : (m_values.m_forageModifier?.Value ?? 1f) * level - (level - 1);
    public float GetSpeedReduction(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_speedReduction?.Value ?? 1f) + 0.1f * (level - 1)));
    public float GetRunStaminaDrain(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - (m_values.m_runStaminaDrain?.Value ?? 0f) * level);
    public float GetAttackSpeedReduction(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_attackSpeedReduction?.Value ?? 1f) - 0.1f * (level - 1)));
    public float GetAttackStaminaUsage(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_attackStaminaUsage?.Value ?? 0f) + (level - 1) * 0.1f));
    public float GetSneakStaminaUsage(int level) => m_values == null ? 1f : Mathf.Clamp01(1 - ((m_values.m_sneakStaminaUsage?.Value ?? 0f) + (level - 1) * 0.1f));
    public int GetProjectileCount(int level) => m_values == null ? 0 : (m_values.m_projectileCount?.Value ?? 0) + (level - 1);
    public float GetResistance(int level, HitData.DamageType type) => m_resistances?.GetResistance(level, type) ?? 1f;
    public GameObject? GetCreatures(Heightmap.Biome biome) => m_creatures?.GetCreatures(biome) ?? null;
    public GameObject? GetCreaturesByLevel(int level) => m_creaturesByLevel?.GetCreaturesByLevel(level) ?? null;
    private string GetCreatureName(GameObject? prefab) => prefab != null ? prefab.TryGetComponent(out Humanoid component) ? component.m_name : "Unknown" : "Invalid";
    public int GetCharacteristic(int level) => (m_characteristic ?? 0) + (level - 1) * 5;
    public string GetName() => m_type is TalentType.Characteristic ? GetTalentType() : $"$talent_{m_key.ToLower()}";
    private string GetDescription() => $"$talent_{m_key.ToLower()}_desc";
    private float GetCreaturesLength(int level) => m_creatures == null ? 0f : GetLength(level);
    public float GetCreaturesByLevelLength(int level) => m_creaturesByLevel == null ? 0f : GetLength(level);
    public int GetCreatureByLevelLevel(int level) => level switch { 2 => 2, 3 => 3, 4 => 1, 5 => 2, 6 => 3, 7 => 1, 8 => 2, 9 => 3, _ => 1, };
    public float GetArmor(int level) => m_values == null ? 0f : (m_values.m_armor?.Value ?? 0f) + (level - 1) * 2f;
    public string GetTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (m_type is TalentType.Characteristic)
        {
            stringBuilder.Append($"$text_unlocks <color=orange>{GetCharacteristic(GetLevel())}</color> $almanac_characteristic $text_points");
        }
        else
        {
            stringBuilder.Append(GetDescription() + "\n\n");
            if (m_tooltip != null) stringBuilder.Append(m_tooltip.Invoke() + "\n");
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
    private ConfigEntry<string>? GetCreaturesConfig() => m_creatures?.GetConfig(GetCurrentBiome());
    private Heightmap.Biome GetCurrentBiome() => Player.m_localPlayer == null ? Heightmap.Biome.None : Player.m_localPlayer.GetCurrentBiome();
    private string GetBiomeLocalized(Heightmap.Biome biome) => $"$biome_{biome.ToString().ToLower()}";
    public int FormatPercentage(float value) => (int)(value * 100 - 100);
    public string GetPrestigeTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (m_type is TalentType.Characteristic)
        {
            var difference = GetCharacteristic(GetLevel() + 1) - GetCharacteristic(GetLevel());
            stringBuilder.Append($"$text_unlocks_additional <color={m_prestigeColor}>{difference}</color> $almanac_characteristic $text_points");
        }
        else
        {
            stringBuilder.Append(GetDescription() + "\n\n");
            if (m_prestigeTooltip != null) stringBuilder.Append(m_prestigeTooltip.Invoke() + "\n");
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
        public ConfigEntry<int>? m_projectileCount;
        public ConfigEntry<float>? m_leech;
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
        public ConfigEntry<string>? GetConfig(Heightmap.Biome biome)
        {
            return biome switch
            {
                Heightmap.Biome.Meadows => m_meadows,
                Heightmap.Biome.BlackForest => m_blackforest,
                Heightmap.Biome.Swamp => m_swamp,
                Heightmap.Biome.Mountain => m_mountains,
                Heightmap.Biome.Plains => m_plains,
                Heightmap.Biome.Mistlands => m_mistlands,
                Heightmap.Biome.AshLands => m_ashlands,
                Heightmap.Biome.DeepNorth => m_deepnorth,
                Heightmap.Biome.Ocean => m_ocean,
                _ => null
            };
        }
        public GameObject? GetCreatures(Heightmap.Biome biome)
        {
            if (!ZNetScene.instance) return null;
            var scene = ZNetScene.instance;
            var creature = biome switch
            {
                Heightmap.Biome.Meadows => scene.GetPrefab(m_meadows?.Value ?? "Neck") ?? scene.GetPrefab("Neck"),
                Heightmap.Biome.BlackForest => scene.GetPrefab(m_blackforest?.Value ?? "Greydwarf") ?? scene.GetPrefab("Greydwarf"),
                Heightmap.Biome.Swamp => scene.GetPrefab(m_swamp?.Value ?? "Draugr") ?? scene.GetPrefab("Draugr"),
                Heightmap.Biome.Mountain => scene.GetPrefab(m_mountains?.Value ?? "Ulv") ?? scene.GetPrefab("Ulv"),
                Heightmap.Biome.Plains => scene.GetPrefab(m_plains?.Value ?? "Deathsquito") ?? scene.GetPrefab("Deathsquito"),
                Heightmap.Biome.Mistlands => scene.GetPrefab(m_mistlands?.Value ?? "Seeker") ?? scene.GetPrefab("Seeker"),
                Heightmap.Biome.AshLands => scene.GetPrefab(m_ashlands?.Value ?? "Surtling") ?? scene.GetPrefab("Surtling"),
                Heightmap.Biome.DeepNorth => scene.GetPrefab(m_deepnorth?.Value ?? "Lox") ?? scene.GetPrefab("Lox"),
                Heightmap.Biome.Ocean => scene.GetPrefab(m_ocean?.Value ?? "Serpent") ?? scene.GetPrefab("Serpent"),
                _ => scene.GetPrefab("Neck")
            };
            return creature.TryGetComponent(out Humanoid _) ? creature : null;
        }
    }
    public class CreaturesByLevel
    {
        public ConfigEntry<string>? m_oneToThree;
        public ConfigEntry<string>? m_fourToSix;
        public ConfigEntry<string>? m_sevenToNine;
        public ConfigEntry<string>? m_ten;
        
        public GameObject? GetCreaturesByLevel(int level)
        {
            if (!ZNetScene.instance) return null;
            ZNetScene scene = ZNetScene.instance;
            bool defeatedBonemass = ZoneSystem.instance.CheckKey("defeated_bonemass", GameKeyType.Player);
            bool defeatedKing = ZoneSystem.instance.CheckKey("defeated_goblinking", GameKeyType.Player);
            bool defeatedQueen = ZoneSystem.instance.CheckKey("defeated_queen", GameKeyType.Player);
            return level switch
            {
                4 => defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                5 => defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                6 => defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                7 => defeatedKing ? scene.GetPrefab(m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                8 => defeatedKing ? scene.GetPrefab(m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                9 => defeatedKing ? scene.GetPrefab(m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                >= 10 => defeatedQueen ? scene.GetPrefab(m_ten?.Value ?? "FallenValkyrie") : defeatedKing ? scene.GetPrefab(m_sevenToNine?.Value ?? "BlobTar") : defeatedBonemass ? scene.GetPrefab(m_fourToSix?.Value ?? "Wraith") : scene.GetPrefab(m_oneToThree?.Value ?? "Ghost"),
                _ => scene.GetPrefab(m_oneToThree?.Value ?? "Ghost")
            };
        }
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
        private float GetValue(HitData.DamageType type)
        {
            var input = type switch
            {
                HitData.DamageType.Blunt => m_blunt,
                HitData.DamageType.Slash => m_slash,
                HitData.DamageType.Pierce => m_pierce,
                HitData.DamageType.Fire => m_fire,
                HitData.DamageType.Frost => m_frost,
                HitData.DamageType.Lightning => m_lightning,
                HitData.DamageType.Poison => m_poison,
                HitData.DamageType.Spirit => m_spirit,
                _ => null
            };
            return input?.Value ?? 1f;
        }
        public float GetResistance(int level, HitData.DamageType type) => Mathf.Clamp01(GetValue(type) - (level - 1) * 0.05f);
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
        public HitData.DamageTypes GetDamages(int level)
        {
            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_blunt = m_blunt?.Value ?? 0f,
                m_pierce = m_pierce?.Value ?? 0f,
                m_slash = m_slash?.Value ?? 0f,
                m_chop = m_chop?.Value ?? 0f,
                m_pickaxe = m_pickaxe?.Value ?? 0f,
                m_fire = m_fire?.Value ?? 0f,
                m_frost = m_frost?.Value ?? 0f,
                m_lightning = m_lightning?.Value ?? 0f,
                m_poison = m_poison?.Value ?? 0f,
                m_spirit = m_spirit?.Value ?? 0f
            };
            damages.Modify(level);
            return damages;
        }
    }

    [Description("Register new talent with a unique key, map it to a button key")]
    public Talent(string key, string buttonName, TalentType type, bool alt = false)
    {
        m_key = key;
        m_button = buttonName;
        m_type = type;
        TalentManager.m_talents[key] = this;
        if (alt) TalentManager.m_altTalentsByButton[m_button] = this;
        else TalentManager.m_talentsByButton[m_button] = this;
    }

    [Description("Register a new talent wtih a unique key, map it to a button key, which awards characteristic points")]
    public Talent(string key, string buttonName, int amount, bool alt = false) : this(key, buttonName, TalentType.Characteristic, alt)
    {
        m_characteristic = amount;
    }

    [Description("Register a new talent with a unique key, map it to a button key, which has a status effect")]
    public Talent(string key, string buttonName, TalentType type, StatusEffect effect, string name, bool alt = false) : this(key, buttonName, type, alt)
    {
        m_status = effect;
        m_status.name = name;
        TalentManager.m_talentsByStatusEffect[name.GetStableHashCode()] = this;
    }
}

public enum TalentType { Ability, Passive, StatusEffect, Characteristic }