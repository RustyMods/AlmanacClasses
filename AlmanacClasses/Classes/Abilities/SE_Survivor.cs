namespace AlmanacClasses.Classes.Abilities;

public class SE_Survivor : StatusEffect
{
    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        m_ttl = AlmanacClassesPlugin._SurvivorLength.Value;
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        float timePercentage = GetRemaningTime() / m_ttl;
        hit.ApplyModifier(1f - timePercentage);
    }
}