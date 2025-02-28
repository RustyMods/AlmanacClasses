using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_RogueBackstab : StatusEffect
{
    private readonly string m_key = "RogueBackstab";
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

public static class RogueBackstab
{
    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    private static class RogueBackstab_ApplyDamage
    {
        private static void Prefix(Character __instance, HitData hit)
        {
            if (!Player.m_localPlayer) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("RogueBackstab", out Talent talent)) return;
            if (talent.m_status is { } status &&
                !Player.m_localPlayer.GetSEMan().HaveStatusEffect(status.NameHash())) return;
            if (!hit.GetAttacker()) return;
            if (!__instance.m_baseAI || !__instance.m_baseAI.IsAlerted()) return;
            if (hit.m_backstabBonus <= 1.0) return;
            if (!__instance.m_baseAI.CanSeeTarget(hit.GetAttacker())) return;
            int random = Random.Range(0, 101);
            if (talent.GetChance(talent.GetLevel()) < random) return;
            hit.ApplyModifier(hit.m_backstabBonus);
            __instance.m_backstabHitEffects.Create(hit.m_point, Quaternion.identity, __instance.transform);
        }
    }
}