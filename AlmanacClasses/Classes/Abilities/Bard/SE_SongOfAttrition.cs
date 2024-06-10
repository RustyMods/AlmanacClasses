using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Bard;

public class SE_SongOfAttrition : StatusEffect
{
    private readonly string m_key = "SongOfAttrition";
    private Talent m_talent = null!;
    private float m_searchTimer;
    private float m_damageTimer;
    private readonly List<Character> m_characters = new();

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_name = talent.GetName();
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
        Transform transform = m_character.transform;
        LoadedAssets.SFX_Dverger_Shot.Create(transform.position, transform.rotation, transform);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        FindCharacters(dt);
        DamageCharacters(dt);
    }

    private void FindCharacters(float dt)
    {
        m_searchTimer += dt;
        if (m_searchTimer < 1f) return;
        m_searchTimer = 0.0f;
        
        m_characters.Clear();
        Character.GetCharactersInRange(m_character.transform.position, 10f, m_characters);
    }

    private void DamageCharacters(float dt)
    {
        m_damageTimer += dt;
        if (m_damageTimer < 0.5f) return;
        m_damageTimer = 0.0f;
        
        foreach (Character character in m_characters)
        {
            character.Damage(new HitData()
            {
                m_damage = m_talent.GetDamages(m_talent.GetLevel()),
                m_attacker = m_character.GetZDOID(),
                m_dodgeable = false,
                m_blockable = false,
                m_skill = Skills.SkillType.ElementalMagic
            });
        }
    }
}