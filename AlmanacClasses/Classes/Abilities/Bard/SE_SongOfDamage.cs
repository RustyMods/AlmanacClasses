using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfDamage : StatusEffect
{
    private readonly string m_key = "SongOfDamage";
    private Talent m_talent = null!;
    private float m_searchTimer;
    private float m_applyTimer;
    public float m_modifier;
    
    private readonly List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
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
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is SE_SongOfDamage song)
            {
                song.m_modifier = m_talent.GetAttack(m_talent.GetLevel());
            }
        }
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.ApplyModifier(m_modifier == 0f ? m_talent.GetAttack(m_talent.GetLevel()) : m_modifier);
    }
}