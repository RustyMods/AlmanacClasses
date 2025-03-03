using System.Collections.Generic;
namespace AlmanacClasses.Classes.Abilities.Shaman;

public class SE_ShamanRegeneration : StatusEffect
{
    public string m_key = "ShamanRegeneration";
    private Talent m_talent = null!;

    public float m_eitrModifier;
    public float m_staminaModifier;
    public List<Player> m_players = new();

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        
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
            if (player == null || player.IsDead()) continue;
            if (player.GetSEMan().HaveStatusEffect(name.GetStableHashCode())) continue;
            StatusEffect effect = player.GetSEMan().AddStatusEffect(name.GetStableHashCode());
            if (effect is not SE_ShamanRegeneration regen) continue;
            regen.m_eitrModifier = m_talent.GetEitrRegen(m_talent.GetLevel());
            regen.m_staminaModifier = m_talent.GetStaminaRegen(m_talent.GetLevel());
        }
    }

    public override void ModifyEitrRegen(ref float eitrRegen)
    {
        eitrRegen *= m_eitrModifier == 0f ? m_talent.GetEitrRegen(m_talent.GetLevel()) : m_eitrModifier;
    }

    public override void ModifyStaminaRegen(ref float staminaRegen)
    {
        staminaRegen *= m_staminaModifier == 0f ? m_talent.GetStaminaRegen(m_talent.GetLevel()) : m_staminaModifier;
    }
}