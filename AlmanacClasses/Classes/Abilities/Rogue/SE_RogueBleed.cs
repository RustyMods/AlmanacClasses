namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueBleed : StatusEffect
{
    private readonly string m_key = "RogueBleed";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        
        base.Setup(character);
    }
}