using AlmanacClasses.LoadAssets;
using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_Bleed : StatusEffect
{
    private readonly string m_key = "RogueBleed";
    private Talent m_talent = null!;
    
    public float m_damageInterval = 1f;
    public float m_bleedTimer;
    public float m_stack = 1f;

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = 5f;
        m_startEffects = LoadedAssets.BleedEffects;
        m_talent = talent;
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        m_time += dt;
        m_bleedTimer += dt;
        if (m_bleedTimer < m_damageInterval) return;
        m_bleedTimer = 0.0f;
        
        HitData hit = new()
        {
            m_point = m_character.GetCenterPoint()
        };
        hit.m_damage.m_pierce = m_stack * m_talent.GetBleed(m_talent.GetLevel());
        hit.m_hitType = HitData.HitType.PlayerHit;
        hit.m_blockable = false;
        hit.m_dodgeable = false;
        hit.m_skill = Skills.SkillType.BloodMagic;
        hit.m_attacker = Player.m_localPlayer.GetZDOID();
        m_character.ApplyDamage(hit, true, true);
    }

    public override void Stop()
    {
        RemoveStartEffects();
    }
}

public static class BleedTrigger
{
    [HarmonyPatch(typeof(Character),nameof(Character.Damage))]
    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    private static class Character_RPC_Damage_Patch
    {
        private static void Prefix(Character __instance, HitData hit)
        {
            if (!__instance || __instance.IsPlayer() || !hit.HaveAttacker()) return;
            if (!hit.GetAttacker().IsPlayer()) return;
            if (hit.GetAttacker() != Player.m_localPlayer) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("RogueBleed", out Talent ability)) return;
            if (ability.m_status is { } status && !hit.GetAttacker().GetSEMan().HaveStatusEffect(status.NameHash())) return;

            if (__instance.m_nview.IsValid()) __instance.m_nview.ClaimOwnership();
            TriggerBleeding(__instance);
        }
    }

    private static void TriggerBleeding(Character __instance)
    {
        if (__instance.GetSEMan().HaveStatusEffect("SE_Bleed".GetStableHashCode()))
        {
            StatusEffect effect = __instance.GetSEMan().GetStatusEffect("SE_Bleed".GetStableHashCode());
            effect.ResetTime();
            if (effect is SE_Bleed bleed) ++bleed.m_stack;
        }
        else
        {
            __instance.GetSEMan().AddStatusEffect("SE_Bleed".GetStableHashCode());
        }
    }
}