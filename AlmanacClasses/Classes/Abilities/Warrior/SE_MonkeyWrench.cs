namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_MonkeyWrench : StatusEffect
{
    private readonly string m_key = "MonkeyWrench";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.ApplyModifier(m_talent.GetDamageReduction(m_talent.GetLevel()));
    }
}