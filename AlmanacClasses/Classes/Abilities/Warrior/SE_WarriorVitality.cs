
namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorVitality : StatusEffect
{
    private readonly string m_key = "WarriorVitality";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        base.Setup(character);
    }

    public override void ModifyHealthRegen(ref float regenMultiplier)
    {
        regenMultiplier *= m_talent.GetHealthRegen(m_talent.GetLevel());
    }
}