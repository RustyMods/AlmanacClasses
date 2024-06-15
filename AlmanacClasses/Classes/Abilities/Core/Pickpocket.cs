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
        if (!PlayerManager.m_playerTalents.TryGetValue("DoubleLoot", out Talent ability)) return;
        Character? attacker = instance.m_lastHit.GetAttacker();
        if (attacker == null) return;
        if (instance.m_baseAI == null) return;
        if (instance.m_baseAI.CanSeeTarget(attacker)) return;
        if (!instance.TryGetComponent(out CharacterDrop characterDrop)) return;
        int random = Random.Range(0, 101);
        if (random > ability.GetChance(ability.GetLevel())) return;
        CharacterDrop.DropItems(characterDrop.GenerateDropList(), instance.GetCenterPoint() + characterDrop.transform.TransformVector(characterDrop.m_spawnOffset), 0.5f);
        Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "<color=yellow>$msg_doubleloot</color>");
    }
}