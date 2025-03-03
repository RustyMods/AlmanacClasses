using AlmanacClasses.LoadAssets;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class AirBender
{
    private static int JumpCount;
    
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    private static class Player_Update_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!__instance) return;
            if (__instance != Player.m_localPlayer) return;
            CheckDoubleJump(__instance);
            CheckDoubleJumpAlt(__instance);
        }
    }

    private static void CheckDoubleJumpAlt(Player instance)
    {
        if (!instance) return;
        if (!PlayerManager.m_playerTalents.TryGetValue("AirBenderAlt", out Talent talent)) return;
        if (!talent.m_passiveActive) return;
        if (instance.IsOnGround()) return;

        if (!(ZInput.GetButtonDown("Jump") || ZInput.GetButtonDown("JoyJump"))) return;
        bool flag = false;

        float eitrCost = talent.GetEitrCost(true, talent.GetLevel());
        if (!instance.HaveEitr(eitrCost - 1))
        {
            Hud.instance.EitrBarEmptyFlash();
            return;
        }
        instance.UseEitr(eitrCost);
        
        if (!instance.HaveStamina(instance.m_jumpStaminaUsage))
        {
            Hud.instance.StaminaBarEmptyFlash();
            flag = true;
        }
        
        Jump(instance, flag);
    }
    private static void CheckDoubleJump(Player instance)
    {
        if (!instance) return;
        if (!PlayerManager.m_playerTalents.TryGetValue("AirBender", out Talent talent)) return;
        if (!talent.m_passiveActive) return;
        if (instance.IsOnGround())
        {
            JumpCount = 0;
            return;
        }

        if (!(ZInput.GetButtonDown("Jump") || ZInput.GetButtonDown("JoyJump"))) return;

        if (JumpCount >= talent.GetLevel()) return;
        
        bool flag = false;

        float eitrCost = talent.GetEitrCost(false);
        if (!instance.HaveEitr(eitrCost - 1))
        {
            Hud.instance.EitrBarEmptyFlash();
            return;
        }
        instance.UseEitr(eitrCost);
        
        if (!instance.HaveStamina(instance.m_jumpStaminaUsage))
        {
            Hud.instance.StaminaBarEmptyFlash();
            flag = true;
        }
        
        Jump(instance, flag);
        ++JumpCount;
    }

    private static void Jump(Player instance, bool flag)
    {
        float speed = instance.m_speed;
        instance.GetSEMan().ApplyStatusEffectSpeedMods(ref speed, instance.m_currentVel);
        if (speed <= 0.0) flag = true;
        float num1 = instance.GetSkills().GetSkillFactor(Skills.SkillType.Jump);
        if (!flag) instance.RaiseSkill(Skills.SkillType.Jump);
        Vector3 velocity = instance.m_body.velocity;
        Vector3 normalized = (instance.m_lastGroundNormal + Vector3.up).normalized;
        float num3 = (float)(1.0 + num1 * 0.4000005);
        float num4 = instance.m_jumpForce * num3;
        float num5 = Vector3.Dot(normalized, velocity);
        if (num5 < num4)
        {
            velocity += normalized * (num4 - num5);
        }
        Vector3 jump = velocity + instance.m_moveDir * instance.m_jumpForceForward * num3;
        instance.GetSEMan().ApplyStatusEffectJumpMods(ref jump);
        if (flag) jump *= instance.m_jumpForceTiredFactor;
        if (jump.x <= 0.0 && jump.y <= 0.0 && jump.z <= 0.0) return;
        instance.m_body.WakeUp();
        instance.m_body.velocity = jump;
        instance.ResetGroundContact();
        instance.m_lastGroundTouch = 1f;
        instance.m_jumpTimer = 0.0f;
        instance.m_zanim.SetTrigger("jump");
        instance.AddNoise(30f);
        Transform transform = instance.transform;
        Quaternion rotation = transform.rotation;
        Vector3 position = transform.position;
        instance.m_jumpEffects.Create(position, rotation, transform);
        VFX.ShieldHitEffects.Create(position, rotation * Quaternion.Euler(90f, 0f, 0f), transform);
        instance.ResetCloth();
        instance.OnJump();
        instance.SetCrouch(false);
        instance.UpdateBodyFriction();
    }
}