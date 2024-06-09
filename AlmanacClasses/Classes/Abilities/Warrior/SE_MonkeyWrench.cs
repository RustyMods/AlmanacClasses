using AlmanacClasses.Data;
using BepInEx.Configuration;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_MonkeyWrench : StatusEffect
{
    private readonly string m_key = "MonkeyWrench";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        hitData.ApplyModifier(talent.GetDamageReduction(talent.GetLevel()));
    }
}