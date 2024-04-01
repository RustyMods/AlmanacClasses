using System.Collections.Generic;

namespace AlmanacClasses.Data;

public static class DefaultData
{
    public static readonly Dictionary<StatusEffectData.Modifier, float> defaultModifiers = new()
    {
        { StatusEffectData.Modifier.Attack, 1f },
        { StatusEffectData.Modifier.HealthRegen , 1f },
        { StatusEffectData.Modifier.StaminaRegen , 1f },
        { StatusEffectData.Modifier.RaiseSkills , 1f },
        { StatusEffectData.Modifier.Speed , 1f },
        { StatusEffectData.Modifier.Noise , 1f },
        { StatusEffectData.Modifier.Stealth , 1f },
        { StatusEffectData.Modifier.RunStaminaDrain , 1f },
        { StatusEffectData.Modifier.JumpStaminaDrain , 1f },
        { StatusEffectData.Modifier.FallDamage , 1f },
        { StatusEffectData.Modifier.EitrRegen , 1f },
        { StatusEffectData.Modifier.DamageReduction , 0f },
        { StatusEffectData.Modifier.Reflect , 0f },
        { StatusEffectData.Modifier.MaxCarryWeight , 0f },
        { StatusEffectData.Modifier.Vitality , 0f },
        { StatusEffectData.Modifier.Stamina , 0f },
        { StatusEffectData.Modifier.Eitr , 0f },
        { StatusEffectData.Modifier.Heal , 0f },
        { StatusEffectData.Modifier.DamageAbsorb , 0f },
    };

    public static readonly Dictionary<Characteristic, int> defaultCharacteristics = new()
    {
        { Characteristic.Constitution, 0 },
        { Characteristic.Intelligence, 0 },
        { Characteristic.Strength, 0 },
        { Characteristic.Dexterity, 0 },
        { Characteristic.Wisdom, 0 }
    };

    public static readonly Dictionary<Characteristic, string> LocalizeCharacteristics = new()
    {
        { Characteristic.Constitution, "$almanac_constitution" },
        { Characteristic.Intelligence, "$almanac_intelligence" },
        { Characteristic.Strength, "$almanac_strength" },
        { Characteristic.Dexterity, "$almanac_dexterity" },
        { Characteristic.Wisdom, "$almanac_wisdom" }
    };

    public static readonly Dictionary<StatusEffectData.Modifier, string> LocalizeModifiers = new()
    {
        { StatusEffectData.Modifier.Attack, "$almanac_attack" },
        { StatusEffectData.Modifier.HealthRegen, "$se_healthregen" },
        { StatusEffectData.Modifier.StaminaRegen, "$se_staminaregen" },
        { StatusEffectData.Modifier.RaiseSkills, "$almanac_raiseskill" },
        { StatusEffectData.Modifier.Speed, "$almanac_speed" },
        { StatusEffectData.Modifier.Noise, "$se_noisemod" },
        { StatusEffectData.Modifier.Stealth, "$se_sneakmod" },
        { StatusEffectData.Modifier.RunStaminaDrain, "$se_runstamina" },
        { StatusEffectData.Modifier.JumpStaminaDrain, "$se_jumpstamina" },
        { StatusEffectData.Modifier.FallDamage, "$se_falldamage" },
        { StatusEffectData.Modifier.EitrRegen, "$se_eitrregen" },
        { StatusEffectData.Modifier.DamageReduction, "$almanac_damage_reduction" },
        { StatusEffectData.Modifier.Reflect, "$almanac_reflect" },
        { StatusEffectData.Modifier.MaxCarryWeight, "$se_max_carryweight" },
        { StatusEffectData.Modifier.Vitality, "$se_health" },
        { StatusEffectData.Modifier.Stamina, "$se_stamina" },
        { StatusEffectData.Modifier.Eitr, "$se_eitr" },
        { StatusEffectData.Modifier.Heal, "$almanac_heal" },
        { StatusEffectData.Modifier.DamageAbsorb, "$almanac_damage_absorb" },
    };

    public static readonly Dictionary<HitData.DamageType, string> LocalizeDamageType = new()
    {
        { HitData.DamageType.Blunt , "$inventory_blunt"},
        { HitData.DamageType.Slash , "$inventory_slash"},
        { HitData.DamageType.Pierce , "$inventory_pierce"},
        { HitData.DamageType.Chop , "$inventory_chop"},
        { HitData.DamageType.Pickaxe , "$inventory_pickaxe"},
        { HitData.DamageType.Fire , "$inventory_fire"},
        { HitData.DamageType.Frost , "$inventory_frost"},
        { HitData.DamageType.Lightning , "$inventory_lightning"},
        { HitData.DamageType.Poison , "$inventory_poison"},
        { HitData.DamageType.Spirit , "$inventory_spirit"}
    };

    public static readonly Dictionary<HitData.DamageModifier, string> LocalizeDamageModifier = new()
    {
        { HitData.DamageModifier.Resistant, "$inventory_resistant" },
        { HitData.DamageModifier.Weak, "$inventory_weak" },
        { HitData.DamageModifier.Immune, "$inventory_immune" },
        { HitData.DamageModifier.VeryResistant, "$inventory_veryresistant" },
        { HitData.DamageModifier.VeryWeak, "$inventory_veryweak" },
        { HitData.DamageModifier.Normal, "$inventory_normal" }
    };
}