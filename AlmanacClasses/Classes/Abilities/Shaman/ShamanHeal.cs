using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class ShamanHeal
{
    public static bool TriggerHeal(float amount)
    {
        Character character = Player.m_localPlayer.GetHoverCreature();
        character.Heal(amount);
        LoadedAssets.FX_Heal.Create(character.transform.position, character.transform.rotation, character.transform);
        // if (character is not Player player) return false;
        //
        // player.Heal(amount);
        //
        // Transform transform = player.transform;
        // LoadedAssets.FX_Heal.Create(transform.position, transform.rotation, transform);
        
        return true;
    }

}