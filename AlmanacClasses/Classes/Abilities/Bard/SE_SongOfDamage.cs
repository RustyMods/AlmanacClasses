using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfDamage : StatusEffect
{
    private readonly string m_key = "SongOfDamage";
    private Talent m_talent = null!;
    private float m_searchTimer;
    private float m_applyTimer;
    public float m_modifier;
    public GameObject[]? m_customEffects;
    
    private readonly List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        base.Setup(character);
        Transform transform = m_character.transform;
        m_customEffects = LoadedAssets.SFX_Dverger_Shot.Create(transform.position, transform.rotation, transform);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        FindPlayers(dt);
        BoostPlayers(dt);
    }
    
    public override void Stop()
    {
        base.Stop();
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

    private void BoostPlayers(float dt)
    {
        m_applyTimer += dt;
        if (m_applyTimer < 1f) return;
        m_applyTimer = 0.0f;
        
        foreach (Player player in m_players)
        {
            if (player == null || player.IsDead()) continue;
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is SE_SongOfDamage song)
            {
                song.m_modifier = m_talent.GetAttack(m_talent.GetLevel());
                song.m_ttl = m_ttl;
                song.m_startEffects = m_startEffects;
                song.m_talent = m_talent;
            }
        }
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.ApplyModifier(m_modifier == 0f ? m_talent.GetAttack(m_talent.GetLevel()) : m_modifier);
    }
}