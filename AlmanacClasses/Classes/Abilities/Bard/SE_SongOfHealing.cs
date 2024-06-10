using System.Collections.Generic;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfHealing : StatusEffect
{
    private readonly string m_key = "SongOfHealing";
    private Talent m_talent = null!;
    private float m_healTimer;

    private readonly List<Player> m_players = new();
    private float m_searchTimer;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        HealPlayers(dt);
        FindPlayers(dt);
    }
    
    private void FindPlayers(float dt)
    {
        m_searchTimer += dt;
        if (m_searchTimer < 1f) return;
        m_searchTimer = 0.0f;
        
        m_players.Clear();
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
    }

    private void HealPlayers(float dt)
    {
        m_healTimer += dt;
        if (m_healTimer < 1f) return;
        m_healTimer = 0.0f;
        
        foreach (Player? player in m_players)
        {
            player.Heal(m_talent.GetHealAmount(m_talent.GetLevel()));
            if (!m_talent.UseEffects()) continue;
            Transform transform = player.transform;
            m_startEffects.Create(transform.position, transform.rotation, transform);
        }
    }
}