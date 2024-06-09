using AlmanacClasses.LoadAssets;

namespace AlmanacClasses.Classes.Abilities.Rogue;

public class SE_Bleed : StatusEffect
{
    private readonly string m_key = "RogueBleed";
    
    public float m_damageInterval = 1f;
    public float m_bleedTimer;
    public float m_stack = 1f;

    public override void Setup(Character character)
    {
        m_ttl = 5f;
        m_startEffects = LoadedAssets.BleedEffects;
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        m_time += dt;
        m_bleedTimer += dt;
        if (m_bleedTimer < m_damageInterval) return;
        m_bleedTimer = 0.0f;

        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        
        HitData hit = new()
        {
            m_point = m_character.GetCenterPoint()
        };
        hit.m_damage.m_pierce = m_stack * talent.GetBleed(talent.GetLevel());
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
    public static void TriggerBleeding(Character __instance)
    {
        if (__instance.GetSEMan().HaveStatusEffect("SE_Bleed".GetStableHashCode()))
        {
            StatusEffect bleed = __instance.GetSEMan().GetStatusEffect("SE_Bleed".GetStableHashCode());
            bleed.ResetTime();
            SE_Bleed? seBleed = bleed as SE_Bleed;
            if (seBleed != null)
            {
                ++seBleed.m_stack;
            }
        }
        else
        {
            __instance.GetSEMan().AddStatusEffect("SE_Bleed".GetStableHashCode());
        }
    }
}