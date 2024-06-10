namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorResistance : StatusEffect
{
    public string m_key = "WarriorResistance";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        hit.ApplyResistance(m_talent.GetDamageModifiers(), out HitData.DamageModifier _);
    }
}