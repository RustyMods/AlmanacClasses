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
        StatusEffect IceBreaker = ScriptableObject.CreateInstance<StatusEffect>();
        IceBreaker.name = "SE_IceBreaker";
        IceBreaker.m_ttl = 10f;
        IceBreaker.m_name = "Iced";
        IceBreaker.m_startEffects = LoadedAssets.DragonBreathHit;

        SE_Characteristics characteristic = ScriptableObject.CreateInstance<SE_Characteristics>();
        characteristic.name = "SE_Characteristic";

        SE_Resourceful resourceful = ScriptableObject.CreateInstance<SE_Resourceful>();
        resourceful.name = "SE_Resourceful";

        StatusEffect enlightened = ScriptableObject.CreateInstance<StatusEffect>();
        enlightened.name = "SE_Enlightened";

        return new List<StatusEffect> { IceBreaker, characteristic, resourceful, enlightened };
    }

    private static List<StatusEffect> LoadBard()
    {
        SE_SongOfAttrition SongOfAttrition = ScriptableObject.CreateInstance<SE_SongOfAttrition>();
        SongOfAttrition.name = "SE_SongOfAttrition";
        
        SE_SongOfDamage SongOfDamage = ScriptableObject.CreateInstance<SE_SongOfDamage>();
        SongOfDamage.name = "SE_SongOfDamage";
        
        SE_SongOfHealing SongOfHealing = ScriptableObject.CreateInstance<SE_SongOfHealing>();
        SongOfHealing.name = "SE_SongOfHealing";
        
        SE_SongOfSpeed SongOfSpeed = ScriptableObject.CreateInstance<SE_SongOfSpeed>();
        SongOfSpeed.name = "SE_SongOfSpeed";
        
        SE_SongOfVitality SongOfVitality = ScriptableObject.CreateInstance<SE_SongOfVitality>();
        SongOfVitality.name = "SE_SongOfVitality";

        return new List<StatusEffect>
        {
            SongOfAttrition, SongOfDamage, SongOfHealing, SongOfSpeed, SongOfVitality
        };
    }

    private static List<StatusEffect> LoadRanger()
    {
        SE_Hunter Hunter = ScriptableObject.CreateInstance<SE_Hunter>();
        Hunter.name = "SE_Hunter";
        SE_LuckyShot LuckyShot = ScriptableObject.CreateInstance<SE_LuckyShot>();
        LuckyShot.name = "SE_LuckyShot";
        SE_QuickShot QuickShot = ScriptableObject.CreateInstance<SE_QuickShot>();
        QuickShot.name = "SE_QuickShot";
        SE_SlowDown SlowDown = ScriptableObject.CreateInstance<SE_SlowDown>();
        SlowDown.name = "SE_SlowDown";
        
        return new List<StatusEffect> { Hunter, LuckyShot, QuickShot, SlowDown };
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
        SE_WarriorStrength Strength = ScriptableObject.CreateInstance<SE_WarriorStrength>();
        Strength.name = "SE_WarriorStrength";
        
        SE_WarriorVitality vitality = ScriptableObject.CreateInstance<SE_WarriorVitality>();
        vitality.name = "SE_WarriorVitality";
        
        SE_MonkeyWrench wrench = ScriptableObject.CreateInstance<SE_MonkeyWrench>();
        wrench.name = "SE_MonkeyWrench";
        
        SE_DualWield dual = ScriptableObject.CreateInstance<SE_DualWield>();
        dual.name = "SE_DualWield";
        
        SE_WarriorResistance resistance = ScriptableObject.CreateInstance<SE_WarriorResistance>();
        resistance.name = "SE_WarriorResistance";

        SE_Survivor survivor = ScriptableObject.CreateInstance<SE_Survivor>();
        survivor.name = "SE_Survivor";

        SE_BattleFury battleFury = ScriptableObject.CreateInstance<SE_BattleFury>();
        battleFury.name = "SE_BattleFury";

        return new List<StatusEffect> { Strength, vitality, wrench, dual, resistance, survivor, battleFury };
    }

    private static List<StatusEffect> LoadShaman()
    {
        SE_ShamanShield shield = ScriptableObject.CreateInstance<SE_ShamanShield>();
        shield.name = "SE_ShamanShield";
        
        SE_ShamanRegeneration regen = ScriptableObject.CreateInstance<SE_ShamanRegeneration>();
        regen.name = "SE_ShamanRegeneration";

        return new List<StatusEffect> {shield, regen};
    }
}