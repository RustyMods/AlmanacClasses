using System.Collections.Generic;
using AlmanacClasses.Data;
using AlmanacClasses.Managers;

namespace AlmanacClasses.Classes;

public static class CharacteristicManager
{
    public static Dictionary<Characteristic, int> m_tempCharacteristics = new(DefaultData.defaultCharacteristics);
    
    public static bool InitCharacteristics(out StatusEffect? effect)
    {
        effect = null;
        StatusEffectManager.Data SE_Characteristic = new()
        {
            name = "SE_Characteristic",
            m_name = "Characteristics",
            m_icon = SpriteManager.HourGlass_Icon,
            m_isCharacteristic = true,
        };
        SE_Characteristic.talent = new();
        if (!SE_Characteristic.Init(out StatusEffect? statusEffect))
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to initialize SE_Characteristics");
            return false;
        }

        effect = statusEffect;
        
        return true;
    }

    public static void AddCharacteristicsEffect(Player player)
    {
        SEMan SE_Man = player.GetSEMan();
        if (SE_Man.HaveStatusEffect("SE_Characteristic")) return;
        SE_Man.AddStatusEffect("SE_Characteristic".GetStableHashCode());
    }

    public static void ReloadCharacteristics()
    {
        if (!ObjectDB.instance || !Player.m_localPlayer) return;
        m_tempCharacteristics.Clear();
        m_tempCharacteristics = GetCharacteristics();
        StatusEffect characteristics = ObjectDB.instance.GetStatusEffect("SE_Characteristic".GetStableHashCode());
        if (!characteristics) return;
        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(characteristics.name))
        {
            Player.m_localPlayer.GetSEMan().RemoveStatusEffect(characteristics);
        }

        if (!InitCharacteristics(out StatusEffect? effect)) return;
        Player.m_localPlayer.GetSEMan().AddStatusEffect(effect);
    }

    private static Dictionary<Characteristic, int> GetCharacteristics()
    {
        Dictionary<Characteristic, int> characteristics = new(DefaultData.defaultCharacteristics);
        foreach (KeyValuePair<string, Talent> talent in PlayerManager.m_playerTalents)
        {
            if (talent.Value.m_type is not TalentType.Characteristic) continue;
            if (talent.Value.m_characteristic is Characteristic.None) continue;
            characteristics[talent.Value.m_characteristic] += talent.Value.m_characteristicValue;
        }

        return characteristics;
    }

    public static int GetCharacteristic(Characteristic type) => m_tempCharacteristics[type];

}