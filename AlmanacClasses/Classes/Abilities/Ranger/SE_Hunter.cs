using System.Collections.Generic;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public class SE_Hunter : StatusEffect
{
    private readonly string m_key = "RangerHunter";
    private float m_searchTimer;
    private float m_affectTimer;
    
    private readonly List<Character> m_characters = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        FindCharacters(dt);
        AffectCharacters(dt);
    }

    private void FindCharacters(float dt)
    {
        m_searchTimer += dt;
        if (m_searchTimer < 1f) return;
        m_searchTimer = 0.0f;
        
        Character.GetCharactersInRange(m_character.transform.position, 10f, m_characters);
    }

    private void AffectCharacters(float dt)
    {
        m_affectTimer += dt;
        if (m_affectTimer < 1f) return;
        m_affectTimer = 0.0f;

        foreach (var character in m_characters)
        {
            if (character is Player) continue;
            if (character.GetSEMan().HaveStatusEffect("SE_SlowDown".GetStableHashCode())) continue;
            character.GetSEMan().AddStatusEffect("SE_SlowDown".GetStableHashCode());
        }
    }
}