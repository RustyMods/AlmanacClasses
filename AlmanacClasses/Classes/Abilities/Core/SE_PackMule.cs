namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_PackMule : StatusEffect
{
    private readonly string m_key = "PackMule";
    private Talent? m_talent;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifyMaxCarryWeight(float baseLimit, ref float limit)
    {
        limit += m_talent?.GetCarryWeight(m_talent.GetLevel()) ?? 0;
    }
}