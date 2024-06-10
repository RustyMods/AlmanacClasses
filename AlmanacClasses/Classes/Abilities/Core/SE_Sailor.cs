namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_Sailor : StatusEffect
{
    private readonly string m_key = "Sailor";
    private Talent m_talent = null!;
    
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_attributes = StatusAttribute.SailingPower;
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        m_ttl = talent.GetLength(talent.GetLevel());
        base.Setup(character);
    }
}