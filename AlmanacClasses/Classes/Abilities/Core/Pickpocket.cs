using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class Pickpocket
{
    public static void CheckDoubleLoot(Character instance)
    {
        if (instance.m_lastHit == null || !instance.m_localPlayerHasHit) return;
        if (!PlayerManager.m_playerTalents.TryGetValue("CoreMerchant", out Talent ability)) return;
        var attacker = instance.m_lastHit.GetAttacker();
        if (attacker == null) return;
        if (instance.m_baseAI == null || instance.m_baseAI.CanSeeTarget(attacker)) return;
        if (!instance.TryGetComponent(out CharacterDrop characterDrop)) return;
        if (ability.m_values == null) return;
        
        int percentage = (int)(ability.GetChance(ability.GetLevel()));
        int random = Random.Range(0, 101);
        if (random <= percentage)
        {
            CharacterDrop.DropItems(characterDrop.GenerateDropList(), instance.GetCenterPoint() + characterDrop.transform.TransformVector(characterDrop.m_spawnOffset), 0.5f);
        }
    }
}