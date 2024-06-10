using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Data;

namespace AlmanacClasses.Classes;

public static class CharacteristicManager
{
    public static Dictionary<Characteristic, int> m_tempCharacteristics = new(DefaultData.defaultCharacteristics);
    public static void ResetCharacteristics() => m_tempCharacteristics = new Dictionary<Characteristic, int>(DefaultData.defaultCharacteristics);
    public static void AddCharacteristic(Characteristic type, int value) => m_tempCharacteristics[type] += value;

    public static void UpdateCharacteristics()
    {
        ResetCharacteristics();
        foreach (KeyValuePair<string, Talent> kvp in PlayerManager.m_playerTalents.Where(kvp => kvp.Value.m_type is TalentType.Characteristic))
        {
            AddCharacteristic(kvp.Value.GetCharacteristicType(), kvp.Value.GetCharacteristic(kvp.Value.GetLevel()));
        }
    }
    public static int GetCharacteristic(Characteristic type) => m_tempCharacteristics.TryGetValue(type, out int value) ? value : 0;

    public static float GetHealthRatio() => GetCharacteristic(Characteristic.Constitution) / AlmanacClassesPlugin._HealthRatio.Value;
    public static float GetStaminaRatio() => GetCharacteristic(Characteristic.Dexterity) / AlmanacClassesPlugin._StaminaRatio.Value;
    public static float GetEitrRatio() => GetCharacteristic(Characteristic.Wisdom) / AlmanacClassesPlugin._EitrRatio.Value;

    public static float GetStrengthModifier()
    {
        int characteristic = GetCharacteristic(Characteristic.Strength);
        float output = characteristic / AlmanacClassesPlugin._PhysicalRatio.Value;
        return 1 + output / 100f;
    }

    public static float GetIntelligenceModifier()
    {
        int characteristic = GetCharacteristic(Characteristic.Intelligence);
        float output = characteristic / AlmanacClassesPlugin._ElementalRatio.Value;
        return 1 + output / 100f;
    }
    public static float GetDexterityModifier()
    {
        int characteristic = GetCharacteristic(Characteristic.Dexterity);
        float output = characteristic / AlmanacClassesPlugin._SpeedRatio.Value;
        return 1 + output / 100f;
    }
}

public enum Characteristic
{
    None,
    Constitution,
    Intelligence,
    Strength,
    Dexterity,
    Wisdom,
}