using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public class SE_ShamanShield : StatusEffect
{
    private readonly string m_key = "ShamanShield";
    
    private float m_absorbDamage = 100f;

    private readonly List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
        FindPlayers();
        BoostPlayers();
    }

    private void BoostPlayers()
    {
        foreach (var player in m_players)
        {
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
        }
    }
    
    private void FindPlayers()
    {
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        m_absorbDamage -= hit.GetTotalDamage();
        hit.ApplyModifier(0f);
        LoadedAssets.ShieldHitEffects.Create(hit.m_point, Quaternion.LookRotation(-hit.m_dir), m_character.transform);
    }

    public override bool IsDone()
    {
        if (m_absorbDamage > 0f) return m_ttl > 0.0 && m_time > m_ttl;
        Transform transform = m_character.transform;
        LoadedAssets.ShieldBreakEffects.Create(m_character.GetCenterPoint(), transform.rotation, transform,
            m_character.GetRadius() * 2f);
        
        return true;
    }
}