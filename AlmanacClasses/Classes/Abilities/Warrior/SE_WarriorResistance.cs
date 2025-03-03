namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorResistance : StatusEffect
{
    public string m_key = "WarriorResistance";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        base.Setup(character);
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        hit.m_damage.m_blunt *= m_talent.GetResistance(m_talent.GetLevel(), HitData.DamageType.Blunt);
        hit.m_damage.m_pierce *= m_talent.GetResistance(m_talent.GetLevel(), HitData.DamageType.Pierce);
        hit.m_damage.m_slash *= m_talent.GetResistance(m_talent.GetLevel(), HitData.DamageType.Slash);
    }
}