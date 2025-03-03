using AlmanacClasses.LoadAssets;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public class SE_Leech : StatusEffect
{
    private const string m_key = "Leech";
    private Talent m_talent = null!;

    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        base.Setup(character);
    }

    public void OnHit(Character character, HitData hit)
    {
        VFX.LeechEffects.Create(hit.m_point, Quaternion.LookRotation(-hit.m_dir), character.transform);
        var totalDamage = hit.GetTotalDamage();
        var healAmount = totalDamage * m_talent.GetLeechModifier(m_talent.GetLevel());
        m_character.Heal(healAmount);
    }

    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    private static class Character_RPC_Damage_Patch
    {
        private static void Postfix(Character __instance, HitData hit)
        {
            if (!TalentManager.m_talents.TryGetValue("Leech", out Talent talent)) return;
            if (hit.GetAttacker() is not { } attacker || attacker != Player.m_localPlayer) return;
            if (talent.m_status is not {} status || attacker.GetSEMan().GetStatusEffect(status.NameHash()) is not SE_Leech statusEffect) return;
            statusEffect.OnHit(__instance, hit);
        }
    }
}