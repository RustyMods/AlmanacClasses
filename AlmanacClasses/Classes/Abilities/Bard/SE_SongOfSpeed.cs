using System.Collections.Generic;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfSpeed : StatusEffect
{
    private readonly string m_key = "SongOfSpeed";
    
    private float m_searchTimer;
    private float m_boostTimer;

    private readonly List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        FindPlayers(dt);
        BoostPlayers(dt);
    }

    private void FindPlayers(float dt)
    {
        m_searchTimer += dt;
        if (m_searchTimer < 1f) return;
        m_searchTimer = 0.0f;
        
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
    }

    private void BoostPlayers(float dt)
    {
        m_boostTimer += dt;
        if (m_boostTimer < 1f) return;
        m_boostTimer = 0.0f;

        foreach (var player in m_players)
        {
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
        }
    }

    public override void ModifySpeed(float baseSpeed, ref float speed, Character character, Vector3 dir)
    {
        speed *= 1.1f;
    }
}