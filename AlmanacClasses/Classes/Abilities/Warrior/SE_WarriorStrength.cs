using AlmanacClasses.LoadAssets;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorStrength : StatusEffect
{
    private readonly string m_key = "WarriorStrength";
    
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);  
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.ApplyModifier(1.1f);
    }

    public override void ModifyHealthRegen(ref float regenMultiplier)
    {
        regenMultiplier *= 1.1f;
    }
}