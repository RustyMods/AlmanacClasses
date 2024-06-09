using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueSpeed : StatusEffect
{
    private readonly string m_key = "RogueSpeed";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void ModifySpeed(float baseSpeed, ref float speed, Character character, Vector3 dir)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;

        speed *= talent.GetSpeedModifier(talent.GetLevel());
    }

    public override void ModifyRunStaminaDrain(float baseDrain, ref float drain)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        drain *= talent.GetRunStaminaDrain(talent.GetLevel());
    }
}