using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfVitality : StatusEffect
{
    private readonly string m_key = "SongOfVitality";
    private Talent m_talent = null!;
    public float m_modifier;
    
    private float m_searchTimer;
    private float m_boostTimer;

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
        m_boostTimer += dt;
        if (m_boostTimer < 1f) return;
        m_boostTimer = 0.0f;

        foreach (Player player in m_players)
        {
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is SE_SongOfVitality song)
            {
                song.m_modifier = m_talent.GetHealthRegen(m_talent.GetLevel());
            }
        }
    }

    public override void ModifyHealthRegen(ref float regenMultiplier)
    {
        regenMultiplier *= m_modifier == 0f ? m_talent.GetHealthRegen(m_talent.GetLevel()) : m_modifier;
    }
}