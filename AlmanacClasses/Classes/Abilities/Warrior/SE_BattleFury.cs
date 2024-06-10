using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_BattleFury : StatusEffect
{
    private readonly string m_key = "BattleFury";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_startEffects = talent.GetEffectList();
        m_name = talent.GetName();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        int random = Random.Range(0, 101);
        if (random < m_talent.GetChance(m_talent.GetLevel())) return;
        if (m_character is Player player) player.AddStamina(10f);
    }
}

public static class BattleFury
{
    public static void CheckBattleFury(Character instance)
    {
        if (!PlayerManager.m_playerTalents.TryGetValue("BattleFury", out Talent talent)) return;
        if (instance.m_lastHit is null or { m_ranged: true }) return;
        float chance = talent.GetChance(talent.GetLevel());
        float random = Random.Range(0, 101);
        if (random > chance) return;
        Player.m_localPlayer.AddStamina(random);
        // if (AlmanacClassesPlugin._BattleFuryFX.Value is AlmanacClassesPlugin.Toggle.Off) return;
        Player.m_localPlayer.GetSEMan().AddStatusEffect("SE_BattleFury".GetStableHashCode());
    }
}