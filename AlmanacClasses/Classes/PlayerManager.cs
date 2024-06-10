using System;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AlmanacClasses.Classes;

[Serializable]
public class PlayerData
{
    public int m_experience;
    public Dictionary<string, int> m_boughtTalents = new();
    public Dictionary<int, string> m_spellBook = new();
}

[Serializable]
public class PlayerDataOld
{
    public int m_experience = 0;
    public Dictionary<int, string> m_spellBook = new();
    public HashSet<string> m_boughtTalents = new();
    public int m_prestige = 1;
    public int m_prestigePoints = 0;
}
public static class PlayerManager
{
    private static readonly string m_oldKey = "AlmanacClassesPlayerData";
    private static readonly string m_playerDataKey = "AlmanacClassesPlayerData_New";
    public static PlayerData m_tempPlayerData = new();
    public static readonly Dictionary<string, Talent> m_playerTalents = new();
    public static void InitPlayerData()
    {
        if (!Player.m_localPlayer) return;
        IDeserializer deserializer = new DeserializerBuilder().Build();
        try
        {
            if (!Player.m_localPlayer.m_customData.TryGetValue(m_playerDataKey, out string data))
            {
                try
                {
                    if (Player.m_localPlayer.m_customData.TryGetValue(m_oldKey, out string old))
                    {
                        var oldData = deserializer.Deserialize<PlayerDataOld>(old);
                        m_tempPlayerData.m_experience = oldData.m_experience;
                        Player.m_localPlayer.m_customData.Remove(m_oldKey);
                    }
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                m_tempPlayerData = deserializer.Deserialize<PlayerData>(data);
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Loaded classes data");
            }
        }
        catch
        {
            // ignored
        }
    }

    private static float m_updateTimer;
    public static void UpdatePassiveEffects(float dt)
    {
        if (!Player.m_localPlayer) return;
        m_updateTimer += dt;
        if (m_updateTimer < 1f) return;
        m_updateTimer = 0.0f;
        AddPassiveStatusEffects(Player.m_localPlayer);
    }

    public static void AddPassiveStatusEffects(Player instance)
    {
        foreach (KeyValuePair<string, Talent> talent in m_playerTalents)
        {
            if (talent.Value.m_type is not TalentType.Passive) continue;
            if (talent.Value.m_statusEffectHash == 0) continue;
            if (talent.Key == "Survivor") continue;
            instance.GetSEMan().AddStatusEffect(talent.Value.m_statusEffectHash);
        }

        instance.GetSEMan().AddStatusEffect("SE_Characteristic".GetStableHashCode());
    }

    public static void InitPlayerTalents()
    {
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Loaded saved player talents");
        foreach (KeyValuePair<int, string> kvp in m_tempPlayerData.m_spellBook)
        {
            if (!TalentManager.m_talents.TryGetValue(kvp.Value, out Talent match)) continue;
            if (match == null) continue;
            SpellBook.m_abilities[kvp.Key] = new AbilityData() { m_data = match };
        }

        foreach (KeyValuePair<string, int> kvp in m_tempPlayerData.m_boughtTalents)
        {
            if (!TalentManager.m_talents.TryGetValue(kvp.Key, out Talent match)) continue;
            if (match == null) continue;
            match.SetLevel(kvp.Value);
            m_playerTalents[kvp.Key] = match;
            if (match.m_type is not TalentType.Characteristic) continue;
            if (match.m_characteristic == null) continue;
            CharacteristicManager.AddCharacteristic(match.GetCharacteristicType(), match.GetCharacteristic(match.GetLevel()));
        }
        CheckAltTalents();
    }

    private static void CheckAltTalents()
    {
        foreach (var kvp in TalentManager.m_altTalentsByButton)
        {
            Talent talent = kvp.Value;
            string button = kvp.Key;

            if (talent.m_alt?.Value is AlmanacClassesPlugin.Toggle.Off) continue;

            RemoveOriginalTalent(button);
            
            m_playerTalents[talent.m_key] = talent;
            
            LoadUI.ChangeButton(talent);
        }
    }

    private static void RemoveOriginalTalent(string button)
    {
        Talent? original = m_playerTalents.Values.ToList().Find(x => x.m_button == button);
        if (original == null) return;
        m_playerTalents.Remove(original.m_key);
        if (m_tempPlayerData.m_boughtTalents.ContainsKey(original.m_key))
        {
            m_tempPlayerData.m_boughtTalents.Remove(original.m_key);
        }
    }

    private static void SaveSpellBook()
    {
        foreach (var kvp in SpellBook.m_abilities)
        {
            m_tempPlayerData.m_spellBook[kvp.Key] = kvp.Value.m_data.m_key;
        }
    }

    public static void SavePlayerData()
    {
        if (!Player.m_localPlayer) return;

        SaveSpellBook();
        
        ISerializer serializer = new SerializerBuilder().Build();
        string data = serializer.Serialize(m_tempPlayerData);
        Player.m_localPlayer.m_customData[m_playerDataKey] = data;
    }

    public static int GetTotalAddedHealth()
    {
        int output = (int)(CharacteristicManager.GetCharacteristic(Characteristic.Constitution) /
                      AlmanacClassesPlugin._HealthRatio.Value);
        foreach (var status in Player.m_localPlayer.GetSEMan().GetStatusEffects())
        {
            if (!StatusEffectManager.IsClassEffect(status.name)) continue;
            if (!TalentManager.m_talentsByStatusEffect.TryGetValue(status.m_nameHash, out Talent talent)) continue;
            if (talent.m_values == null) continue;
            output += (int)talent.GetHealth(talent.GetLevel());
        }
        return output;
    }

    public static int GetTotalAddedStamina()
    {
        int output = (int)(CharacteristicManager.GetCharacteristic(Characteristic.Dexterity) /
                      AlmanacClassesPlugin._StaminaRatio.Value);

        foreach (var status in Player.m_localPlayer.GetSEMan().GetStatusEffects())
        {
            if (!StatusEffectManager.IsClassEffect(status.name)) continue;
            if (!TalentManager.m_talentsByStatusEffect.TryGetValue(status.m_nameHash, out Talent talent)) continue;
            if (talent.m_values == null) continue;
            output += (int)talent.GetStamina(talent.GetLevel());
        }
        return output;
    }

    public static int GetTotalAddedEitr()
    {
        int output = (int)(CharacteristicManager.GetCharacteristic(Characteristic.Wisdom) /
                      AlmanacClassesPlugin._EitrRatio.Value);

        foreach (var status in Player.m_localPlayer.GetSEMan().GetStatusEffects())
        {
            if (!StatusEffectManager.IsClassEffect(status.name)) continue;
            if (!TalentManager.m_talentsByStatusEffect.TryGetValue(status.m_nameHash, out Talent talent)) continue;
            if (talent.m_values == null) continue;
            output += (int)talent.GetEitr(talent.GetLevel());
        }

        return output;
    }

    public static int GetPlayerLevel(int experience) => Mathf.Clamp((int)Math.Pow(experience / 100f, 0.5f), 1, AlmanacClassesPlugin._MaxLevel.Value);
    public static int GetRequiredExperience(int level) => (int)Math.Pow(level, 2) * 100;

    public static void AddExperience(int amount) => m_tempPlayerData.m_experience += amount;
    public static int GetExperience() => m_tempPlayerData.m_experience;

    public static float CalculateModifier(float input, int level) => input + (input - 1) * (level - 1);
}