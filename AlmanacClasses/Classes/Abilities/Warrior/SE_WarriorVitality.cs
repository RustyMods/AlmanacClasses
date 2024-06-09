
namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_WarriorVitality : StatusEffect
{
    private readonly string m_key = "WarriorVitality";
    
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void ModifyHealthRegen(ref float regenMultiplier)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        regenMultiplier *= talent.GetHealthRegen(talent.GetLevel());
    }
}