namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_Survivor : StatusEffect
{
    private readonly string m_key = "Survivor";
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = 10f;
        m_startEffects = talent.GetEffectList();
        m_name = talent.GetName();
        base.Setup(character);
    }
    public override void OnDamaged(HitData hit, Character attacker)
    {
        float timePercentage = GetRemaningTime() / m_ttl;
        hit.ApplyModifier(1f - timePercentage);
    }
}