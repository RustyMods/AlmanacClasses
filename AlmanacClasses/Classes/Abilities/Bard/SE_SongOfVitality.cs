using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfVitality : StatusEffect
{
    private readonly string m_key = "SongOfVitality";
    private Talent? m_talent;
    private readonly List<Player> m_players = new();
    public float m_modifier;
    private float m_searchTimer;
    private float m_boostTimer;
    private GameObject[]? m_customEffects;

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent) && m_talent == null)
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        if (m_ttl == 0f) m_ttl = 10f;

        base.Setup(character);
        Transform transform = m_character.transform;
        m_customEffects = VFX.SFX_Dverger_Shot.Create(transform.position, transform.rotation, transform);
        
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
            if (player.GetSEMan().HaveStatusEffect(NameHash())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(NameHash());
            if (effect is SE_SongOfVitality song)
            {
                song.m_modifier = m_talent?.GetHealthRegen(m_talent.GetLevel()) ?? 1f;
                song.m_talent = m_talent;
                song.m_ttl = m_ttl;
                song.m_startEffects = m_startEffects;
            }
        }
    }

    public override void ModifyHealthRegen(ref float regenMultiplier)
    {
        regenMultiplier *= m_modifier == 0f ? m_talent?.GetHealthRegen(m_talent.GetLevel()) ?? 1f : m_modifier;
    }
}