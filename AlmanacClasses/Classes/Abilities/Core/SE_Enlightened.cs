namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_Enlightened : StatusEffect
{
    private readonly string m_key = "Wise";
    private Talent? m_talent;

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifyEitrRegen(ref float eitrRegen)
    {
        eitrRegen *= m_talent?.GetEitrRegen(m_talent.GetLevel()) ?? 1f;
    }
}