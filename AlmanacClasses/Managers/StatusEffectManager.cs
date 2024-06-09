using System.Collections.Generic;
using AlmanacClasses.Classes.Abilities.Bard;
using AlmanacClasses.Classes.Abilities.Core;
using AlmanacClasses.Classes.Abilities.Ranger;
using AlmanacClasses.Classes.Abilities.Rogue;
using AlmanacClasses.Classes.Abilities.Shaman;
using AlmanacClasses.Classes.Abilities.Warrior;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Managers;

public static class StatusEffectManager
{
    private static readonly Dictionary<string, StatusEffect> m_statusEffects = new();

    public static bool IsClassEffect(string name) => m_statusEffects.ContainsKey(name);
    public static void InitStatusEffects(ObjectDB __instance)
    {
        List<StatusEffect> effects = new();
        effects.AddRange(LoadCore());
        effects.AddRange(LoadBard());
        effects.AddRange(LoadRanger());
        effects.AddRange(LoadRogue());
        effects.AddRange(LoadWarrior());
        effects.AddRange(LoadShaman());
        foreach (var effect in effects)
        {
            m_statusEffects[effect.name] = effect;
            if (__instance.m_StatusEffects.Contains(effect)) continue;
            __instance.m_StatusEffects.Add(effect);
        }
    }

    private static List<StatusEffect> LoadCore()
    {
        var IceBreaker = ScriptableObject.CreateInstance<StatusEffect>();
        IceBreaker.name = "SE_IceBreaker";
        IceBreaker.m_ttl = 10f;
        IceBreaker.m_name = "Iced";
        IceBreaker.m_startEffects = LoadedAssets.DragonBreathHit;

        var characteristic = ScriptableObject.CreateInstance<SE_Characteristics>();
        characteristic.name = "SE_Characteristic";

        var resourceful = ScriptableObject.CreateInstance<SE_Resourceful>();
        resourceful.name = "SE_Resourceful";

        var enlightened = ScriptableObject.CreateInstance<StatusEffect>();
        enlightened.name = "SE_Enlightened";

        return new() { IceBreaker, characteristic, resourceful, enlightened };
    }

    private static List<StatusEffect> LoadBard()
    {
        var SongOfAttrition = ScriptableObject.CreateInstance<SE_SongOfAttrition>();
        SongOfAttrition.name = "SE_SongOfAttrition";
        var SongOfDamage = ScriptableObject.CreateInstance<SE_SongOfDamage>();
        SongOfDamage.name = "SE_SongOfDamage";
        var SongOfHealing = ScriptableObject.CreateInstance<SE_SongOfHealing>();
        SongOfHealing.name = "SE_SongOfHealing";
        var SongOfSpeed = ScriptableObject.CreateInstance<SE_SongOfSpeed>();
        SongOfSpeed.name = "SE_SongOfSpeed";
        var SongOfVitality = ScriptableObject.CreateInstance<SE_SongOfVitality>();
        SongOfVitality.name = "SE_SongOfVitality";

        return new()
        {
            SongOfAttrition, SongOfDamage, SongOfHealing, SongOfSpeed, SongOfVitality
        };
    }

    private static List<StatusEffect> LoadRanger()
    {
        var Hunter = ScriptableObject.CreateInstance<SE_Hunter>();
        Hunter.name = "SE_Hunter";
        var LuckyShot = ScriptableObject.CreateInstance<SE_LuckyShot>();
        LuckyShot.name = "SE_LuckyShot";
        var QuickShot = ScriptableObject.CreateInstance<SE_QuickShot>();
        QuickShot.name = "SE_QuickShot";
        var SlowDown = ScriptableObject.CreateInstance<SE_SlowDown>();
        SlowDown.name = "SE_SlowDown";
        
        return new() { Hunter, LuckyShot, QuickShot, SlowDown };
    }

    private static List<StatusEffect> LoadRogue()
    {
        SE_Bleed Bleed = ScriptableObject.CreateInstance<SE_Bleed>();
        Bleed.name = "SE_Bleed";
        SE_RogueBleed RogueBleed = ScriptableObject.CreateInstance<SE_RogueBleed>();
        RogueBleed.name = "SE_RogueBleed";
        SE_RogueBackstab RogueBackstab = ScriptableObject.CreateInstance<SE_RogueBackstab>();
        RogueBackstab.name = "SE_Backstab";
        SE_RogueReflect RogueReflect = ScriptableObject.CreateInstance<SE_RogueReflect>();
        RogueReflect.name = "SE_RogueReflect";
        SE_RogueSpeed RogueSpeed = ScriptableObject.CreateInstance<SE_RogueSpeed>();
        RogueSpeed.name = "SE_RogueSpeed";
        SE_RogueStamina RogueStamina = ScriptableObject.CreateInstance<SE_RogueStamina>();
        RogueStamina.name = "SE_RogueStamina";

        return new() { Bleed, RogueBleed, RogueBackstab, RogueBackstab, RogueReflect, RogueSpeed, RogueStamina};
    }

    private static List<StatusEffect> LoadWarrior()
    {
        var Strength = ScriptableObject.CreateInstance<SE_WarriorStrength>();
        Strength.name = "SE_WarriorStrength";
        
        var vitality = ScriptableObject.CreateInstance<SE_WarriorVitality>();
        vitality.name = "SE_WarriorVitality";
        
        var wrench = ScriptableObject.CreateInstance<SE_MonkeyWrench>();
        wrench.name = "SE_MonkeyWrench";
        
        var dual = ScriptableObject.CreateInstance<SE_DualWield>();
        dual.name = "SE_DualWield";
        
        var resistance = ScriptableObject.CreateInstance<SE_WarriorResistance>();
        resistance.name = "SE_WarriorResistance";

        var survivor = ScriptableObject.CreateInstance<SE_Survivor>();
        survivor.name = "SE_Survivor";

        var battleFury = ScriptableObject.CreateInstance<SE_BattleFury>();
        battleFury.name = "SE_BattleFury";

        return new List<StatusEffect> { Strength, vitality, wrench, dual, resistance, survivor, battleFury };
    }

    private static List<StatusEffect> LoadShaman()
    {
        var shield = ScriptableObject.CreateInstance<SE_ShamanShield>();
        shield.name = "SE_ShamanShield";
        
        var regen = ScriptableObject.CreateInstance<SE_ShamanRegeneration>();
        regen.name = "SE_ShamanRegeneration";

        return new() {shield, regen};
    }
}