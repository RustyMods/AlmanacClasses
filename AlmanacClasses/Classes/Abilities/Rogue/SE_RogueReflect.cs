using AlmanacClasses.LoadAssets;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueReflect : StatusEffect
{
    private readonly string m_key = "RogueReflect";
    
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void OnDamaged(HitData hit, Character attacker)
    {
        var reflect = hit.Clone();
        reflect.ApplyModifier(0.5f);
        attacker.Damage(reflect);
    }
}