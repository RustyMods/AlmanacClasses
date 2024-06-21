using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public class SE_ShamanShield : StatusEffect
{
    private readonly string m_key = "ShamanShield";
    private Talent? m_talent;
    
    private float m_absorbDamage;

    private readonly List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_absorbDamage = m_absorbDamage == 0f ? talent.GetAbsorb(talent.GetLevel()) : m_absorbDamage;
            m_talent = talent;
        }
        if (m_ttl == 0f) m_ttl = 10f;
        base.Setup(character);
        if (!PlayerManager.m_playerTalents.ContainsKey(m_key)) return;
        FindPlayers();
        BoostPlayers();
    }

    private void BoostPlayers()
    {
        foreach (Player player in m_players)
        {
            if (player == null || player.IsDead()) continue;
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is SE_ShamanShield shield)
            {
                shield.m_absorbDamage = m_talent?.GetAbsorb(m_talent.GetLevel()) ?? 0f;
                shield.m_talent = m_talent;
                shield.m_ttl = m_ttl;
                shield.m_startEffects = m_startEffects;
            }
        }
    }
    
    private void FindPlayers()
    {
        m_players.Clear();
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