using AlmanacClasses.LoadAssets;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public class SE_QuickShot : StatusEffect
{
    private readonly string m_key = "QuickShot";
    
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }
}