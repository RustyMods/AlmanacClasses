using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public class SE_ShamanRegeneration : StatusEffect
{
    public string m_key = "ShamanRegeneration";
    public List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
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
        foreach (var player in m_players)
        {
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
        }
    }

    public override void ModifyEitrRegen(ref float eitrRegen)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        eitrRegen *= talent.GetEitrRegen(talent.GetLevel());
    }
}