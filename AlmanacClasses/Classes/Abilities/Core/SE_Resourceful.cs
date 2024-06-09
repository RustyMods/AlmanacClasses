namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_Resourceful : StatusEffect
{
    private readonly string m_key = "Resourceful";

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        HitData.DamageTypes damages = talent.GetDamages(talent.GetLevel());
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