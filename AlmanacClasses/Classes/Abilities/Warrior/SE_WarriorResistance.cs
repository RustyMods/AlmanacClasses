namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorResistance : StatusEffect
{
    public string m_key = "WarriorResistance";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        hit.ApplyResistance(talent.GetDamageModifiers(), out HitData.DamageModifier _);
    }
}