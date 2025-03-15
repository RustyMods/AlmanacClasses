namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_Resourceful : StatusEffect
{
    private const string m_key = "Resourceful";
    private Talent? m_talent;

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_talent = talent;
        m_startEffects = talent.GetEffectList();
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        HitData.DamageTypes damages = m_talent?.GetDamages(m_talent.GetLevel()) ?? new();
        if (hitData.m_damage.m_chop > 0f)
        {
            hitData.m_damage.m_chop += damages.m_chop;
        }

        if (hitData.m_damage.m_pickaxe > 0f)
        {
            hitData.m_damage.m_pickaxe += damages.m_pickaxe;
        }
    }
}