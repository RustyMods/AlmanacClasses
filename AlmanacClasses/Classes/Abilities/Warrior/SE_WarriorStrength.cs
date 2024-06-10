
namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorStrength : StatusEffect
{
    private readonly string m_key = "WarriorStrength";
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
        hitData.ApplyModifier(m_talent.GetAttack(m_talent.GetLevel()));
    }

    public override void ModifyHealthRegen(ref float regenMultiplier)
    {
        regenMultiplier *= m_talent.GetHealthRegen(m_talent.GetLevel());
    }
}