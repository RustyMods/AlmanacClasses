using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueBackstab : StatusEffect
{
    private readonly string m_key = "RogueBackstab";
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
        float chance = m_talent.GetChance(m_talent.GetLevel());
        int random = Random.Range(0, 101);
        if (chance < random) return;
        float bonus = hitData.m_backstabBonus;
        hitData.ApplyModifier(bonus);
    }
}