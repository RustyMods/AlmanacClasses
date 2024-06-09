namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_Sneaky : StatusEffect
{
    public override void ModifySneakStaminaUsage(float baseStaminaUse, ref float staminaUse)
    {
        staminaUse *= 0.9f;
    }
}