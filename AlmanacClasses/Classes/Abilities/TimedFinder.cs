using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

public static class TimedFinder
{
    public static StatusEffect? CreateCustomFinder(string name, string displayName, string tooltip, float ttl, bool isCharacter = false)
    {
        if (LoadedAssets.SE_Finder == null) return null;
        
        StatusEffectManager.CustomFinder TimedFinder = ScriptableObject.CreateInstance<StatusEffectManager.CustomFinder>();
        TimedFinder.name = name;
        TimedFinder.m_ttl = ttl;
        TimedFinder.m_icon = SpriteManager.Wishbone_Icon;
        TimedFinder.m_name = displayName;
        TimedFinder.m_pingEffectNear = LoadedAssets.SE_Finder.m_pingEffectNear;
        TimedFinder.m_pingEffectMed = LoadedAssets.SE_Finder.m_pingEffectMed;
        TimedFinder.m_pingEffectFar = LoadedAssets.SE_Finder.m_pingEffectFar;
        TimedFinder.m_findCharacters = isCharacter;
        TimedFinder.m_tooltip = tooltip;

        StatusEffect? match = ObjectDB.instance.GetStatusEffect(name.GetStableHashCode());
        if (match)
        {
            ObjectDB.instance.m_StatusEffects.Remove(match);
        }
        ObjectDB.instance.m_StatusEffects.Add(TimedFinder);
        return TimedFinder;
    }
}