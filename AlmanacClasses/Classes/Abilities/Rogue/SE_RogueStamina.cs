namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueStamina : StatusEffect
{
    private readonly string m_key = "RogueStamina";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void ModifyStaminaRegen(ref float staminaRegen)
    {
        staminaRegen *= 1.1f;
    }

    public override void ModifyAttackStaminaUsage(float baseStaminaUse, ref float staminaUse)
    {
        staminaUse *= 0.9f;
    }

    public override void ModifySneakStaminaUsage(float baseStaminaUse, ref float staminaUse)
    {
        staminaUse *= 0.5f;
    }
}