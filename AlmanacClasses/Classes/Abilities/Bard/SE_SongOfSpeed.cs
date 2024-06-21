using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfSpeed : StatusEffect
{
    private readonly string m_key = "SongOfSpeed";
    private Talent? m_talent;
    private readonly List<Player> m_players = new();
    private float m_searchTimer;
    private float m_boostTimer;
    private float m_modifier;
    private GameObject[]? m_customEffects;

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent) && m_talent == null)
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        };
        if (m_ttl == 0f) m_ttl = 10f;

        base.Setup(character);
        Transform transform = m_character.transform;
        m_customEffects = LoadedAssets.SFX_Dverger_Shot.Create(transform.position, transform.rotation, transform);

        if (!PlayerManager.m_playerTalents.ContainsKey(m_key))
        {
            AnimationManager.DoAnimation(m_talent?.m_animation ?? "");
        }
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        if (!PlayerManager.m_playerTalents.ContainsKey(m_key)) return;
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
        m_boostTimer += dt;
        if (m_boostTimer < 1f) return;
        m_boostTimer = 0.0f;

        foreach (Player player in m_players)
        {
            if (player == null || player.IsDead()) continue;
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is SE_SongOfSpeed speed)
            {
                speed.m_modifier = m_talent?.GetSpeedModifier(m_talent.GetLevel()) ?? 1f;
                speed.m_talent = m_talent;
                speed.m_ttl = m_ttl;
                speed.m_startEffects = m_startEffects;
            }
        }
    }

    public override void ModifySpeed(float baseSpeed, ref float speed, Character character, Vector3 dir)
    {
        speed *= m_modifier == 0f ? m_talent?.GetSpeedModifier(m_talent.GetLevel()) ?? 1f : m_modifier;
    }
}