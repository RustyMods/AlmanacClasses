using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class ShamanHeal
{
    public static bool TriggerHeal(float amount)
    {
        Character character = Player.m_localPlayer.GetHoverCreature();
        if (character == null)
        {
            Player.m_localPlayer.Heal(amount);
            Transform transform = Player.m_localPlayer.transform;
            LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        }
        else
        {
            if (character is not Player) return false;
            character.Heal(amount);
            Transform transform = character.transform;
            LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        }
        
        return true;
    }

}