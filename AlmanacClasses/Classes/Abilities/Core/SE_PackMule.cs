namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_PackMule : StatusEffect
{
    private readonly string m_key = "PackMule";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        base.Setup(character);
    }

    public override void ModifyMaxCarryWeight(float baseLimit, ref float limit)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        limit += talent.GetCarryWeight(talent.GetLevel());
    }
}