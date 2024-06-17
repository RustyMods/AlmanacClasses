using System;
using System.Collections.Generic;
using AlmanacClasses.Classes.Abilities.Warrior;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using HarmonyLib;
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
    private static bool m_initiatedPlayerData;
    private static bool m_initiatedPlayerTalents;
    
    public static void OnLogout()
    {
        m_initiatedPlayerData = false;
        m_initiatedPlayerTalents = false;
        ClearPlayerData();
        ClearPlayerTalents();
    }
    private static readonly string m_oldKey = "AlmanacClassesPlayerData";
    private static readonly string m_playerDataKey = "AlmanacClassesPlayerData_New";
    public static PlayerData m_tempPlayerData = new();
    public static readonly Dictionary<string, Talent> m_playerTalents = new();

    public static void ResetPlayerData()
    {
        m_tempPlayerData.m_boughtTalents.Clear();
        m_tempPlayerData.m_spellBook.Clear();
        m_playerTalents.Clear();
    }
    private static void ClearPlayerTalents() => m_playerTalents.Clear();
    private static void ClearPlayerData() => m_tempPlayerData = new();
    private static void InitPlayerData()
    {
        if (m_initiatedPlayerData) return;
        IDeserializer deserializer = new DeserializerBuilder().Build();
        try
        {
            if (!Player.m_localPlayer.m_customData.TryGetValue(m_playerDataKey, out string data))
            {
                try
                {
                    if (Player.m_localPlayer.m_customData.TryGetValue(m_oldKey, out string old))
                    {
                        PlayerDataOld oldData = deserializer.Deserialize<PlayerDataOld>(old);
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
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Loaded player data");
            }
            m_initiatedPlayerData = true;
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

    private static void AddPassiveStatusEffects(Player instance)
    {
        foreach (KeyValuePair<string, Talent> talent in m_playerTalents)
        {
            if (talent.Value.m_type is not TalentType.Passive) continue;
            if (talent.Value.m_statusEffectHash == 0) continue;
            if (!talent.Value.m_passiveActive) continue;
            instance.GetSEMan().AddStatusEffect(talent.Value.m_statusEffectHash);
        }

        instance.GetSEMan().AddStatusEffect("SE_Characteristic".GetStableHashCode());
    }

    private static void InitPlayerTalents()
    {
        if (m_initiatedPlayerTalents) return;
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Loaded player talents");
        LoadSpellBook();
        LoadPlayerTalents();
        m_initiatedPlayerTalents = true;
    }

    private static void LoadSpellBook()
    {
        foreach (KeyValuePair<int, string> kvp in m_tempPlayerData.m_spellBook)
        {
            if (!TalentManager.m_talents.TryGetValue(kvp.Value, out Talent match)) continue;
            if (match == null) continue;
            SpellBook.m_abilities[kvp.Key] = new AbilityData() { m_data = match };
        }
    }

    private static void LoadPlayerTalents()
    {
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
        foreach (KeyValuePair<string, Talent> kvp in TalentManager.m_altTalentsByButton)
        {
            Talent talent = kvp.Value;
            if (talent.m_alt?.Value is AlmanacClassesPlugin.Toggle.Off) continue;
            LoadUI.ChangeButton(talent, false, talent.GetFillLine());
        }
    }
    private static void SaveSpellBook()
    {
        foreach (KeyValuePair<int, AbilityData> kvp in SpellBook.m_abilities)
        {
            m_tempPlayerData.m_spellBook[kvp.Key] = kvp.Value.m_data.m_key;
        }
    }

    private static void SavePlayerData()
    {
        if (!Player.m_localPlayer) return;
        SaveSpellBook();
        ISerializer serializer = new SerializerBuilder().Build();
        string data = serializer.Serialize(m_tempPlayerData);
        Player.m_localPlayer.m_customData[m_playerDataKey] = data;
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Saved player data");
    }

    private static int GetTotalAddedHealth()
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

    private static int GetTotalAddedStamina()
    {
        int output = (int)(CharacteristicManager.GetCharacteristic(Characteristic.Dexterity) / AlmanacClassesPlugin._StaminaRatio.Value);

        foreach (StatusEffect? status in Player.m_localPlayer.GetSEMan().GetStatusEffects())
        {
            if (!StatusEffectManager.IsClassEffect(status.name)) continue;
            if (!TalentManager.m_talentsByStatusEffect.TryGetValue(status.m_nameHash, out Talent talent)) continue;
            if (talent.m_values == null) continue;
            output += (int)talent.GetStamina(talent.GetLevel());
        }
        return output;
    }

    private static int GetTotalAddedEitr()
    {
        if (!Player.m_localPlayer) return 0;
        int output = (int)(CharacteristicManager.GetCharacteristic(Characteristic.Wisdom) / AlmanacClassesPlugin._EitrRatio.Value);

        foreach (StatusEffect? status in Player.m_localPlayer.GetSEMan().GetStatusEffects())
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
    
    [HarmonyPatch(typeof(Character), nameof(Character.SetMaxHealth))]
    private static class Character_SetMaxHealth_Patch
    {
        private static void Prefix(Character __instance, ref float health)
        {
            if (!__instance) return;
            if (!__instance.IsPlayer()) return;
            if (__instance != Player.m_localPlayer) return;
            health += GetTotalAddedHealth();
        }
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.Save))]
    private static class Player_Save_Patch
    {
        private static void Prefix() => SavePlayerData();
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.Load))]
    private static class Player_Load_Postfix
    {
        private static void Postfix() => InitPlayerData();
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    private static class Player_OnSpawned_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return;
            InitPlayerTalents();
            if (m_playerTalents.ContainsKey("MonkeyWrench")) MonkeyWrench.ModifyTwoHandedWeapons();
            LoadUI.SetHUDVisibility(AlmanacClassesPlugin._HudVisible.Value is AlmanacClassesPlugin.Toggle.On);
        } 
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.SetMaxEitr))]
    private static class Player_SetMaxEitr_Patch
    {
        private static void Prefix(Player __instance, ref float eitr)
        {
            if (__instance != Player.m_localPlayer) return;
            eitr += GetTotalAddedEitr();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.SetMaxStamina))]
    private static class Player_SetMaxStamina_Patch
    {
        private static void Prefix(Player __instance, ref float stamina)
        {
            if (__instance != Player.m_localPlayer) return;
            stamina += GetTotalAddedStamina();
        }
    }
}