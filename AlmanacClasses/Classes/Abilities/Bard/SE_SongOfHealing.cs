using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfHealing : StatusEffect
{
    private readonly string m_key = "SongOfHealing";
    private float m_searchTimer;
    private float m_healTimer;

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
        HealPlayers(dt);
    }

    private void FindPlayers(float dt)
    {
        m_searchTimer += dt;
        if (m_searchTimer < 1f) return;
        m_searchTimer = 0.0f;
        
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
    }

    private void HealPlayers(float dt)
    {
        m_healTimer += dt;
        if (m_healTimer < 1f) return;
        m_healTimer = 0.0f;

        foreach (var player in m_players)
        {
            player.Heal(1f);
        }
    }
}