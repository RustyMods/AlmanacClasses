namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueStamina : StatusEffect
{
    private readonly string m_key = "RogueStamina";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifyStaminaRegen(ref float staminaRegen)
    {
        staminaRegen *= m_talent.GetStaminaRegen(m_talent.GetLevel());
    }

    public override void ModifyAttackStaminaUsage(float baseStaminaUse, ref float staminaUse)
    {
        staminaUse *= m_talent.GetAttackStaminaUsage(m_talent.GetLevel());
    }

    public override void ModifySneakStaminaUsage(float baseStaminaUse, ref float staminaUse)
    {
        staminaUse *= m_talent.GetSneakStaminaUsage(m_talent.GetLevel());
    }
}