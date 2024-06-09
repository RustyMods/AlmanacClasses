using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public class SE_LuckyShot : StatusEffect
{
    private readonly string m_key = "LuckyShot";

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength();
        m_startEffects = talent.GetEffectList();
        
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        if (m_character is not Player player) return;
        if (!hitData.m_ranged) return;

        if (player is not Humanoid humanoid) return;

        var ammo = humanoid.GetAmmoItem();

        var random = Random.Range(0, 101);

        if (random < 20) return;

        var clone = ammo.Clone();
        clone.m_stack = 1;
        player.GetInventory().AddItem(clone);

    }
}