using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class ShamanHeal
{
    public static bool TriggerHeal(float amount)
    {
        if (!Player.m_localPlayer) return false;
        Character character = Player.m_localPlayer.GetHoverCreature();
        if (character == null)
        {
            if (!Player.m_localPlayer || Player.m_localPlayer.IsDead()) return false;
            Player.m_localPlayer.Heal(amount);
            Transform transform = Player.m_localPlayer.transform;
            LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        }
        else
        {
            if (character == null || character.IsDead()) return false;
            if (character is not Player) return false;
            character.Heal(amount);
            Transform transform = character.transform;
            LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        }
        
        return true;
    }

}