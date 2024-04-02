using System;
using System.Collections.Generic;
using AlmanacClasses.Data;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx.Configuration;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AlmanacClasses.Classes;

public static class PlayerManager
{
    public static readonly string m_playerDataKey = "AlmanacClassesPlayerData";
    public static PlayerData m_tempPlayerData = new();
    public static readonly Dictionary<string, Talent> m_playerTalents = new();
    public static void InitPlayerData()
    {
        if (!Player.m_localPlayer) return;
        if (!Player.m_localPlayer.m_customData.TryGetValue(m_playerDataKey, out string data))
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Failed to get classes player data");
        }
        else
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            m_tempPlayerData = deserializer.Deserialize<PlayerData>(data);
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Loaded classes data");
        }
    }

    public static void AddPassiveStatusEffects(Player instance)
    {
        foreach (KeyValuePair<string, Talent> talent in m_playerTalents)
        {
            if (talent.Value.m_triggerNow)
            {
                instance.GetSEMan().AddStatusEffect(talent.Value.m_statusEffect.name.GetStableHashCode());
            }
        }
    }

    public static void InitPlayerTalents()
    {
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Loaded saved player talents");
        foreach (string talent in m_tempPlayerData.m_boughtTalents)
        {
            if (!TalentManager.AllTalents.TryGetValue(talent, out Talent match)) continue;
            if (match == null) continue;
            m_playerTalents[talent] = match;
            if (match.m_isAbility)
            {
                SpellBook.m_abilities.Add(new AbilityData(){m_data = match});
            }
        }
        CharacteristicManager.ReloadCharacteristics();
    }

    public static void SavePlayerData()
    {
        if (!Player.m_localPlayer) return;
        ISerializer serializer = new SerializerBuilder().Build();
        string data = serializer.Serialize(m_tempPlayerData);
        Player.m_localPlayer.m_customData[m_playerDataKey] = data;
    }

    public static int GetTotalAddedHealth()
    {
        int output = CharacteristicManager.GetCharacteristic(Characteristic.Constitution) / AlmanacClassesPlugin._HealthRatio.Value;
        output = Mathf.Clamp(output, 0, AlmanacClassesPlugin._MaxHealth.Value);
        List<StatusEffect> effects = Player.m_localPlayer.GetSEMan().GetStatusEffects().FindAll(x => x is StatusEffectManager.Data.TalentEffect);
        foreach (StatusEffect? effect in effects)
        {
            StatusEffectManager.Data.TalentEffect? talentEffect = effect as StatusEffectManager.Data.TalentEffect;
            if (talentEffect == null) continue;
            if (!talentEffect.data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Vitality, out ConfigEntry<float> config)) continue;
            
            output += (int)config.Value * talentEffect.data.talent.m_level;
        }
        
        return output;
    }

    public static int GetTotalAddedStamina()
    {
        int output = CharacteristicManager.GetCharacteristic(Characteristic.Dexterity) / AlmanacClassesPlugin._StaminaRatio.Value;
        output = Mathf.Clamp(output, 0, AlmanacClassesPlugin._MaxStamina.Value);
        List<StatusEffect> effects = Player.m_localPlayer.GetSEMan().GetStatusEffects().FindAll(x => x is StatusEffectManager.Data.TalentEffect);
        foreach (StatusEffect? effect in effects)
        {
            StatusEffectManager.Data.TalentEffect? talentEffect = effect as StatusEffectManager.Data.TalentEffect;
            if (talentEffect == null) continue;
            if (!talentEffect.data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Stamina, out ConfigEntry<float> config)) continue;
            
            output += (int)config.Value * talentEffect.data.talent.m_level;
        }

        return output;
    }

    public static int GetTotalAddedEitr()
    {
        int output = CharacteristicManager.GetCharacteristic(Characteristic.Wisdom) / AlmanacClassesPlugin._EitrRatio.Value;
        output = Mathf.Clamp(output, 0, AlmanacClassesPlugin._MaxEitr.Value);
        List<StatusEffect> effects = Player.m_localPlayer.GetSEMan().GetStatusEffects().FindAll(x => x is StatusEffectManager.Data.TalentEffect);
        foreach (StatusEffect? effect in effects)
        {
            StatusEffectManager.Data.TalentEffect? talentEffect = effect as StatusEffectManager.Data.TalentEffect;
            if (talentEffect == null) continue;
            if (!talentEffect.data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Eitr, out ConfigEntry<float> config)) continue;
            
            output += (int)config.Value * talentEffect.data.talent.m_level;
        }

        return output;
    }

    public static int GetDamageRatio(Characteristic type)
    {
        int characteristic = CharacteristicManager.GetCharacteristic(type);
        int output = characteristic / AlmanacClassesPlugin._DamageRatio.Value;
        return 1 + output / 100;
    }
    
    public static int GetPlayerLevel(int experience) => (int)Math.Pow(experience / 100f, 0.5f);
    public static int GetRequiredExperience(int level) => (int)Math.Pow(level, 2) * 100;
}