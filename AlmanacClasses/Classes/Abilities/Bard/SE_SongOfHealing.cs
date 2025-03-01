using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfHealing : StatusEffect
{
    private readonly string m_key = "SongOfHealing";
    private Talent? m_talent;
    private readonly List<Player> m_players = new();
    private float m_healTimer;
    private float m_searchTimer;
    private GameObject[]? m_customEffects;

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        if (m_ttl == 0f) m_ttl = 10f;
        
        base.Setup(character);
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
        Transform transform = m_character.transform;
        m_customEffects = VFX.SFX_Dverger_Shot.Create(transform.position, transform.rotation, transform);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        HealPlayers(dt);
        FindPlayers(dt);
    }
    
    public override void Stop()
    {
        base.Stop();
        RemoveCustomEffects();
    }

    private void RemoveCustomEffects()
    {
        if (m_customEffects == null || ZNetScene.instance == null) return;
        foreach (GameObject instance in m_customEffects)
        {
            if (instance == null) continue;
            if (!instance.TryGetComponent(out ZNetView component)) continue;
            if (!component.IsValid()) continue;
            component.ClaimOwnership();
            component.Destroy();
        }
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
            if (player == null || player.IsDead()) continue;
            player.Heal(m_talent?.GetHealAmount(m_talent.GetLevel()) ?? 0f);
            if (!m_talent?.UseEffects() ?? false) continue;
            Transform transform = player.transform;
            m_startEffects.Create(transform.position, transform.rotation, transform);
        }
    }
}