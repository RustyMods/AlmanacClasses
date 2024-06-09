namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueBleed : StatusEffect
{
    private readonly string m_key = "RogueBleed";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }
}