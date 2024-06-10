namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueReflect : StatusEffect
{
    private readonly string m_key = "RogueReflect";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        HitData reflect = hit.Clone();
        reflect.ApplyModifier(m_talent.GetReflect(m_talent.GetLevel()));
        attacker.Damage(reflect);
    }
}