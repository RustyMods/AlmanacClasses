using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueSpeed : StatusEffect
{
    private readonly string m_key = "RogueSpeed";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifySpeed(float baseSpeed, ref float speed, Character character, Vector3 dir)
    {
        speed *= m_talent.GetSpeedModifier(m_talent.GetLevel());
    }

    public override void ModifyRunStaminaDrain(float baseDrain, ref float drain)
    {
        drain *= m_talent.GetRunStaminaDrain(m_talent.GetLevel());
    }
}