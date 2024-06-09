using System;
using System.Reflection;
using AlmanacClasses.Classes;
using AlmanacClasses.Data;

namespace AlmanacClasses.API;

public static class ClassesAPI
{
    private static readonly MethodInfo? API_AddExperience;
    private static readonly MethodInfo? API_GetLevel;
    private static readonly MethodInfo? API_GetCharacteristic;
    private static readonly MethodInfo? API_GetPrestigeLevel;
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

    public static int GetPrestigeLevel()
    {
        return (int)(API_GetPrestigeLevel?.Invoke(null, null) ?? 1);
    }

    static ClassesAPI()
    {
        if (Type.GetType("AlmanacClasses.API.API, AlmanacClasses") is not { } api)
        {
            return;
        }
        
        API_AddExperience = api.GetMethod("AddExperience", BindingFlags.Public | BindingFlags.Static);
        API_GetLevel = api.GetMethod("GetLevel", BindingFlags.Public | BindingFlags.Static);
        API_GetCharacteristic = api.GetMethod("GetCharacteristic", BindingFlags.Public | BindingFlags.Static);
        API_GetPrestigeLevel = api.GetMethod("GetPrestigeLevel", BindingFlags.Public | BindingFlags.Static);
    }
}

public static class API
{
    public static void AddExperience(int amount) => PlayerManager.AddExperience(amount);
    public static int GetLevel() => PlayerManager.GetPlayerLevel(PlayerManager.GetExperience());
    public static int GetCharacteristic(string type)
    {
        switch (type)
        {
            case "Constitution":
                return CharacteristicManager.GetCharacteristic(Characteristic.Constitution);
            case "Dexterity":
                return CharacteristicManager.GetCharacteristic(Characteristic.Dexterity);
            case "Intelligence":
                return CharacteristicManager.GetCharacteristic(Characteristic.Intelligence);
            case "Wisdom":
                return CharacteristicManager.GetCharacteristic(Characteristic.Wisdom);
            case "Strength":
                return CharacteristicManager.GetCharacteristic(Characteristic.Strength);
            default:
                return 0;
        }
    }
}