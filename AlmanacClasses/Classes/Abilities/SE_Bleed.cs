using System;
using AlmanacClasses.Data;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

public class SE_Bleed : StatusEffect
{
    public float m_damageInterval = 1f;
    public float m_bleedTimer;
    public float m_stack = 1f;
    
    public override void UpdateStatusEffect(float dt)
    {
        m_time += dt;
        m_bleedTimer += dt;
        if (m_bleedTimer < m_damageInterval) return;
        m_bleedTimer = 0.0f;
        HitData hit = new()
        {
            m_point = m_character.GetCenterPoint()
        };
        float damage = Mathf.Clamp(
            Spells.ApplySkill(Skills.SkillType.BloodMagic,
                TalentManager.AllTalents.TryGetValue("RogueBleed", out Talent talent)
                    ? talent.m_talentDamages?.pierce?.Value ?? 1f
                    : 1f), 1f, Single.MaxValue);
        hit.m_damage.m_pierce = damage * m_stack;
        hit.m_hitType = HitData.HitType.PlayerHit;
        hit.m_blockable = false;
        hit.m_dodgeable = false;
        hit.m_skill = Skills.SkillType.BloodMagic;
        hit.m_attacker = Player.m_localPlayer.GetZDOID();
        m_character.ApplyDamage(hit, true, true);
    }

    public override void Stop()
    {
        RemoveStartEffects();
    }
}