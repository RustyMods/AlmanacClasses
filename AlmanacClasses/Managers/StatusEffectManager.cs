using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes;
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
    public static bool Loaded() => m_statusEffects.Count > 0;
    public static bool IsClassEffect(string name) => m_statusEffects.ContainsKey(name);
    public static void InitStatusEffects(ObjectDB __instance)
    {
        SE_Bleed Bleed = ScriptableObject.CreateInstance<SE_Bleed>();
        Bleed.name = "SE_Bleed";
        StatusEffect IceBreaker = ScriptableObject.CreateInstance<StatusEffect>();
        IceBreaker.name = "SE_IceBreaker";
        IceBreaker.m_ttl = 10f;
        IceBreaker.m_name = "Iced";
        IceBreaker.m_startEffects = LoadedAssets.DragonBreathHit;
        SE_Characteristics characteristic = ScriptableObject.CreateInstance<SE_Characteristics>();
        characteristic.name = "SE_Characteristic";
        SE_SlowDown SlowDown = ScriptableObject.CreateInstance<SE_SlowDown>();
        SlowDown.name = "SE_SlowDown";
        SE_Stats SE_LightningResist = ScriptableObject.CreateInstance<SE_Stats>();
        SE_LightningResist.name = "SE_LightningResist";
        SE_LightningResist.m_name = "Call Of Lightning";
        SE_LightningResist.m_ttl = 3f;
        SE_LightningResist.m_mods = new List<HitData.DamageModPair>()
        {
            new HitData.DamageModPair()
                { m_type = HitData.DamageType.Lightning, m_modifier = HitData.DamageModifier.Ignore },
            new HitData.DamageModPair()
                { m_type = HitData.DamageType.Pierce, m_modifier = HitData.DamageModifier.Resistant }
        };
        List<StatusEffect> effects = new()
        {
            Bleed, IceBreaker, characteristic, SlowDown, SE_LightningResist
        };
        foreach (var talent in TalentManager.m_talents.Values)
        {
            if (talent.m_status is not { } status) continue;
            effects.Add(status);
        }
        foreach (StatusEffect? effect in effects)
        {
            m_statusEffects[effect.name] = effect;
            if (__instance.m_StatusEffects.Contains(effect)) continue;
            __instance.m_StatusEffects.Add(effect);
        }
    }
}