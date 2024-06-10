using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class ShamanHeal
{
    public static bool TriggerHeal(float amount)
    {
        Character character = Player.m_localPlayer.GetHoverCreature();
        character.Heal(amount);
        Transform transform = character.transform;
        LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        // if (character is not Player player) return false;
        //
        // player.Heal(amount);
        //
        // Transform transform = player.transform;
        // LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        
        return true;
    }

}