﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AlmanacClasses.Classes;
using HarmonyLib;

namespace AlmanacClasses.API;

public static class ClassesAPI
{
    // Add this section to your solution
    // It will give you methods that references my methods in the class API below
    private static readonly MethodInfo? API_AddExperience;
    private static readonly MethodInfo? API_GetLevel;
    private static readonly MethodInfo? API_GetCharacteristic;
    public static void AddEXP(int amount)
    {
        API_AddExperience?.Invoke(null, new object[] { amount });
    }
    public static int GetLevel()
    {
        return (int)(API_GetLevel?.Invoke(null, null) ?? 0);
    }
    public static int GetCharacteristic(string type)
    {
        return (int)(API_GetCharacteristic?.Invoke(null, new object[] { type }) ?? 0);
    }
    public static int GetConstitution() => GetCharacteristic("Constitution");
    public static int GetDexterity() => GetCharacteristic("Dexterity");
    public static int GetStrength() => GetCharacteristic("Strength");
    public static int GetIntelligence() => GetCharacteristic("Intelligence");
    public static int GetWisdom() => GetCharacteristic("Wisdom");
    static ClassesAPI()
    {
        if (Type.GetType("AlmanacClasses.API.API, AlmanacClasses") is not { } api)
        {
            return;
        }
        
        API_AddExperience = api.GetMethod("AddExperience", BindingFlags.Public | BindingFlags.Static);
        API_GetLevel = api.GetMethod("GetLevel", BindingFlags.Public | BindingFlags.Static);
        API_GetCharacteristic = api.GetMethod("GetCharacteristic", BindingFlags.Public | BindingFlags.Static);
    }
}

public static class API
{
    // Do not copy this section
    // These are the methods that the ClassAPI references
    public static void AddExperience(int amount) => PlayerManager.AddExperience(amount);
    public static int GetLevel() => PlayerManager.GetPlayerLevel(PlayerManager.GetExperience());
    public static int GetCharacteristic(string type)
    {
        return type switch
        {
            "Constitution" => CharacteristicManager.GetCharacteristic(Characteristic.Constitution),
            "Dexterity" => CharacteristicManager.GetCharacteristic(Characteristic.Dexterity),
            "Intelligence" => CharacteristicManager.GetCharacteristic(Characteristic.Intelligence),
            "Wisdom" => CharacteristicManager.GetCharacteristic(Characteristic.Wisdom),
            "Strength" => CharacteristicManager.GetCharacteristic(Characteristic.Strength),
            _ => 0
        };
    }

    // Another way to add experience is to call Player.m_localPlayer.Message(..., "Added 10 experience")
    [HarmonyPatch(typeof(Player), nameof(Player.Message))]
    private static class Player_Message_Patch
    {
        private static void Postfix(MessageHud.MessageType type, string msg)
        {
            if (type is not MessageHud.MessageType.TopLeft) return;
            if (!msg.StartsWith("Added") || !msg.EndsWith("experience")) return;
            string[] input = msg.Split(' ');
            if (!int.TryParse(input[1], out int experience)) return;
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Received experience from external source");
            PlayerManager.m_tempPlayerData.m_experience += experience;
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug($"Added {experience} class experience");
        }
    }
}