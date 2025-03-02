using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.UI;

namespace AlmanacClasses.Classes;

public static class CharacteristicManager
{
    public static readonly Dictionary<Characteristic, int> m_defaults = new()
    {
        [Characteristic.None] = 0,
        [Characteristic.Constitution] = 0,
        [Characteristic.Intelligence] = 0,
        [Characteristic.Strength] = 0,
        [Characteristic.Dexterity] = 0,
        [Characteristic.Wisdom] = 0,
    };

    public static Dictionary<Characteristic, int> m_tempCharacteristics = new(m_defaults);
    public static void OnLogout()
    {
        ResetCharacteristics();
    }
    public static void ResetCharacteristics() => m_tempCharacteristics = new Dictionary<Characteristic, int>(m_defaults);

    public static void Add(Characteristic type, int value)
    {
        if (m_tempCharacteristics.ContainsKey(type)) m_tempCharacteristics[type] += value;
        else m_tempCharacteristics[type] = value;
        Update();
    }

    public static void Remove(Characteristic type, int value)
    {
        if (m_tempCharacteristics.ContainsKey(type)) m_tempCharacteristics[type] -= value;
        else m_tempCharacteristics[type] = 0;
        Update();
    }

    public static int GetRemainingPoints()
    {
        return GetTotalPoints() - GetUsedPoints();
    }

    public static int GetUsedPoints()
    {
        return m_tempCharacteristics.Values.Sum();
    }

    public static int GetTotalPoints()
    {
        List<Talent> talents = PlayerManager.m_playerTalents.Where(kvp => kvp.Value.m_type is TalentType.Characteristic).Select(kvp => kvp.Value).ToList();
        int sum = 0;
        foreach (var talent in talents)
        {
            sum += talent.GetCharacteristic(talent.GetLevel());
        }

        return sum;
    }

    public static void Update()
    {
        UpdateCharacteristicPoints();
        CharacteristicButtons.UpdateAllButtons();
        SkillTree.m_instance.UpdateStatsBonus();
    }

    public static void UpdateCharacteristicPoints() => SkillTree.m_instance.CharacteristicAmount.text =
        $"<color=orange>{GetRemainingPoints()}</color>";
    public static string GetTooltip()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<color=orange>$almanac_characteristic</color>\n");
        foreach (KeyValuePair<Characteristic, int> kvp in m_tempCharacteristics)
        {
            if (kvp.Key is Characteristic.None) continue;
            switch (kvp.Key)
            {
                case Characteristic.Constitution:
                    stringBuilder.Append($"$se_health: <color=orange>+{(int)GetHealthRatio()}</color>\n");
                    break;
                case Characteristic.Strength:
                    stringBuilder.Append($"$se_max_carryweight: <color=orange>+{(int)GetCarryWeightRatio()}</color>\n");
                    stringBuilder.Append($"$almanac_physical: <color=orange>+{FormatPercentage(GetStrengthModifier())}%</color>\n");
                    break;
                case Characteristic.Intelligence:
                    stringBuilder.Append($"$almanac_elemental: <color=orange>+{FormatPercentage(GetIntelligenceModifier())}%</color>\n");
                    break;
                case Characteristic.Dexterity:
                    stringBuilder.Append($"$se_stamina: <color=orange>+{(int)GetStaminaRatio()}</color>\n");
                    stringBuilder.Append($"$almanac_attackspeedmod: <color=orange>+{FormatPercentage(GetDexterityModifier())}%</color>\n");
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
    public static float GetCarryWeightRatio(int extra = 0) => (GetCharacteristic(Characteristic.Strength) + extra) / AlmanacClassesPlugin._CarryWeightRatio.Value;
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