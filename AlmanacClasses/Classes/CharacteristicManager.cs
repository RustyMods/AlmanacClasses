using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlmanacClasses.Classes;

public static class CharacteristicManager
{
    private static readonly Dictionary<Characteristic, int> m_defaults = new()
    {
        [Characteristic.None] = 0,
        [Characteristic.Constitution] = 0,
        [Characteristic.Intelligence] = 0,
        [Characteristic.Strength] = 0,
        [Characteristic.Dexterity] = 0,
        [Characteristic.Wisdom] = 0,
    };

    private static Dictionary<Characteristic, int> m_tempCharacteristics = new(m_defaults);
    public static void ResetCharacteristics() => m_tempCharacteristics = new Dictionary<Characteristic, int>(m_defaults);
    public static void AddCharacteristic(Characteristic type, int value) => m_tempCharacteristics[type] += value;
    public static void UpdateCharacteristics()
    {
        ResetCharacteristics();
        foreach (KeyValuePair<string, Talent> kvp in PlayerManager.m_playerTalents.Where(kvp => kvp.Value.m_type is TalentType.Characteristic))
        {
            AddCharacteristic(kvp.Value.GetCharacteristicType(), kvp.Value.GetCharacteristic(kvp.Value.GetLevel()));
        }
    }

    public static string GetCharacteristicKey(Characteristic type) => $"$almanac_{type.ToString().ToLower()}";

    public static string GetTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<color=orange>$almanac_characteristic</color>\n");
        foreach (var kvp in m_tempCharacteristics)
        {
            if (kvp.Key is Characteristic.None) continue;
            switch (kvp.Key)
            {
                case Characteristic.Constitution:
                    stringBuilder.Append($"$se_health: <color=orange>+{(int)GetHealthRatio()}</color>\n");
                    break;
                case Characteristic.Strength:
                    stringBuilder.Append($"$se_max_carryweight: <color=orange>+{kvp.Value}</color>\n");
                    stringBuilder.Append($"$almanac_physical: <color=orange>+{FormatPercentage(GetStrengthModifier())}%</color>\n");
                    break;
                case Characteristic.Intelligence:
                    stringBuilder.Append($"$almanac_elemental: <color=orange>+{FormatPercentage(GetIntelligenceModifier())}%</color>\n");
                    break;
                case Characteristic.Dexterity:
                    stringBuilder.Append($"$se_stamina: <color=orange>+{(int)GetStaminaRatio()}</color>\n");
                    stringBuilder.Append(
                        $"$almanac_attackspeedmod: <color=orange>+{FormatPercentage(GetDexterityModifier())}%</color>\n");
                    break;
                case Characteristic.Wisdom:
                    stringBuilder.Append($"$se_eitr: <color=orange>+{(int)GetEitrRatio()}</color>\n");
                    break;
            }
        }

        return Localization.instance.Localize(stringBuilder.ToString());
    }
    private static int FormatPercentage(float value) => (int)(value * 100 - 100);
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