using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_BattleFury : StatusEffect
{
    private readonly string m_key = "BattleFury";
    
    public int m_chance = 20;

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        m_name = talent.GetName();
        
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        int random = Random.Range(0, 101);
        if (random < m_chance) return;
        m_character.AddStamina(10f);
    }
}

