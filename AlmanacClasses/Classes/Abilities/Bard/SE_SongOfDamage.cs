using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfDamage : StatusEffect
{
    private readonly string m_key = "SongOfDamage";
    
    private float m_searchTimer;
    private float m_applyTimer;
    
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
        m_applyTimer += dt;
        if (m_applyTimer < 1f) return;
        m_applyTimer = 0.0f;
        
        foreach (var player in m_players)
        {
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
        }
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.ApplyModifier(1.1f);
    }
}