using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public class SE_ShamanRegeneration : StatusEffect
{
    public string m_key = "ShamanRegeneration";
    private Talent m_talent = null!;

    public float m_modifier;
    public List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        
        base.Setup(character);
        FindPlayers();
        BoostPlayers();
    }

    private void FindPlayers()
    {
        Player.GetPlayersInRange(m_character.transform.position, 10f, m_players);
    }

    private void BoostPlayers()
    {
        foreach (Player player in m_players)
        {
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is SE_ShamanRegeneration regen)
            {
                regen.m_modifier = m_talent.GetEitrRegen(m_talent.GetLevel());
            }
        }
    }

    public override void ModifyEitrRegen(ref float eitrRegen)
    {
        eitrRegen *= m_modifier == 0f ? m_talent.GetEitrRegen(m_talent.GetLevel()) : m_modifier;
    }
}