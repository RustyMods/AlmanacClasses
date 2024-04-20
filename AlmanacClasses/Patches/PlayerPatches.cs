using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes;
using AlmanacClasses.Data;
using AlmanacClasses.LoadAssets;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using YamlDotNet.Serialization;

namespace AlmanacClasses.Patches;

public static class PlayerPatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetMaxEitr))]
    private static class Player_SetMaxEitr_Patch
    {
        private static void Prefix(Player __instance, ref float eitr)
        {
            if (__instance != Player.m_localPlayer) return;
            eitr += PlayerManager.GetTotalAddedEitr();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.SetMaxStamina))]
    private static class Player_SetMaxStamina_Patch
    {
        private static void Prefix(Player __instance, ref float stamina)
        {
            if (__instance != Player.m_localPlayer) return;
            stamina += PlayerManager.GetTotalAddedStamina();
        }
    }

    private static string initialFoodItem = "";
    private static float initialFood;
    private static float initialStamina;
    private static float initialEitr;

    [HarmonyPatch(typeof(Player), nameof(Player.EatFood))]
    private static class Player_EatFood_Patch
    {
        private static void Prefix(Player __instance, ref ItemDrop.ItemData item)
        {
            if (!__instance.CanEat(item, false)) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("CoreChef", out Talent ability)) return;
            initialFoodItem = item.m_shared.m_name;
            initialFood = item.m_shared.m_food;
            initialStamina = item.m_shared.m_foodStamina;
            initialEitr = item.m_shared.m_foodEitr;
            item.m_shared.m_food *= AlmanacClassesPlugin._MasterChefIncrease.Value * ability.m_level;
            item.m_shared.m_foodStamina *= AlmanacClassesPlugin._MasterChefIncrease.Value * ability.m_level;
            item.m_shared.m_foodEitr *= AlmanacClassesPlugin._MasterChefIncrease.Value * ability.m_level;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.EatFood))]
    private static class Player_EatFood_Patch2
    {
        private static void Postfix(ref ItemDrop.ItemData item, ref bool __result)
        {
            if (!__result) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("CoreChef", out Talent ability)) return;
            if (initialFoodItem != item.m_shared.m_name) return;
            item.m_shared.m_food = initialFood;
            item.m_shared.m_foodStamina = initialStamina;
            item.m_shared.m_foodEitr = initialEitr;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Save))]
    private static class Player_Save_Patch
    {
        private static void Prefix()
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Saving classes data");
            PlayerManager.SavePlayerData();
        }
    }

    public static bool initiated;
    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    private static class Player_OnSpawned_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return;
            if (initiated) return;
            PlayerManager.InitPlayerData();
            TalentManager.InitTalents(PlayerManager.m_tempPlayerData.m_prestige);
            PlayerManager.InitPlayerTalents();
            PlayerManager.AddPassiveStatusEffects(__instance);
            CharacteristicManager.AddCharacteristicsEffect(__instance);
            if (PlayerManager.m_playerTalents.TryGetValue("MonkeyWrench", out Talent ability))
            {
                LoadTwoHanded.ModifyTwoHandedWeapons();
            }

            initiated = true;
        } 
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnRespawn))]
    private static class Player_OnRespawn_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return;
            CharacteristicManager.AddCharacteristicsEffect(__instance);
            PlayerManager.AddPassiveStatusEffects(__instance);
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.GetHoverName))]
    private static class Player_GetHoverName_Patch
    {
        private static void Postfix(Player __instance, ref string __result)
        {
            if (__instance.m_customData.TryGetValue(PlayerManager.m_playerDataKey, out string data))
            {
                var deserializer = new DeserializerBuilder().Build();
                var playerData = deserializer.Deserialize<PlayerData>(data);
                var name = __result;
                switch (playerData.m_prestige)
                {
                    case 1:
                        break;
                    case 2:
                        __result = $"<color=blue>{name}</color>";
                        break;
                    case 3:
                        __result = $"<color=red>{name}</color>";
                        break;
                    default:
                        __result = $"<color=orange>{name}</color>";
                        break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    private static class Player_Update_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!__instance) return;
            if (__instance != Player.m_localPlayer) return;
            CheckDoubleJump(__instance);
        }
    }

    private static int JumpCount = 0;
    
    private static void CheckDoubleJump(Player instance)
    {
        if (!instance) return;
        if (!PlayerManager.m_playerTalents.TryGetValue("CoreComfort1", out Talent talent)) return;
        if (instance.IsOnGround())
        {
            JumpCount = 0;
            return;
        }
        
        if (JumpCount >= ((talent.m_healthCost?.Value ?? 1) * talent.m_level)) return;
        if (!ZInput.GetButtonDown("Jump")) return;
        bool flag = false;

        if (!instance.HaveEitr(talent.m_eitrCost?.Value ?? 0f))
        {
            Hud.instance.EitrBarEmptyFlash();
            return;
        }
        instance.UseEitr(talent.m_eitrCost?.Value ?? 0f);
        
        if (!instance.HaveStamina(instance.m_jumpStaminaUsage))
        {
            Hud.instance.StaminaBarEmptyFlash();
            flag = true;
        }
        float speed = instance.m_speed;
        instance.GetSEMan().ApplyStatusEffectSpeedMods(ref speed);
        if (speed <= 0.0) flag = true;
        float num1 = 0.0f;
        num1 = instance.GetSkills().GetSkillFactor(Skills.SkillType.Jump);
        if (!flag) instance.RaiseSkill(Skills.SkillType.Jump);
        Vector3 velocity = instance.m_body.velocity;
        // double num2 = Mathf.Acos(Mathf.Clamp01(instance.m_lastGroundNormal.y));
        Vector3 normalized = (instance.m_lastGroundNormal + Vector3.up).normalized;
        float num3 = (float)(1.0 + num1 * 0.4000005);
        float num4 = instance.m_jumpForce * num3;
        float num5 = Vector3.Dot(normalized, velocity);
        if (num5 < num4)
        {
            velocity += normalized * (num4 - num5);
        }
        Vector3 jump = velocity + instance.m_moveDir * instance.m_jumpForceForward * num3;
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
        var rotation = transform.rotation;
        var position = transform.position;
        instance.m_jumpEffects.Create(position, rotation, transform);
        LoadedAssets.ShieldHitEffects.Create(position, rotation * Quaternion.Euler(90f, 0f, 0f), transform);
        instance.ResetCloth();
        instance.OnJump();
        instance.SetCrouch(false);
        instance.UpdateBodyFriction();
        ++JumpCount;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UseHotbarItem))]
    private static class Player_UseHotbarItem_Patch
    {
        private static bool Prefix()
        {
            if (!AreKeysAlpha()) return true;
            return !Input.GetKey(AlmanacClassesPlugin._SpellAlt.Value);
        }

        private static bool AreKeysAlpha()
        {
            if (AlmanacClassesPlugin._SpellAlt.Value is KeyCode.None) return false;
            List<ConfigEntry<KeyCode>> configs = new()
            {
                AlmanacClassesPlugin._Spell1,
                AlmanacClassesPlugin._Spell2,
                AlmanacClassesPlugin._Spell3,
                AlmanacClassesPlugin._Spell4,
                AlmanacClassesPlugin._Spell5,
                AlmanacClassesPlugin._Spell6,
                AlmanacClassesPlugin._Spell7,
                AlmanacClassesPlugin._Spell8
            };
            return configs.Any(isKeyAlpha);
        }

        private static bool isKeyAlpha(ConfigEntry<KeyCode> config)
        {
            return config.Value 
                is KeyCode.Alpha1 
                or KeyCode.Alpha2 
                or KeyCode.Alpha3
                or KeyCode.Alpha4 
                or KeyCode.Alpha4 
                or KeyCode.Alpha5 
                or KeyCode.Alpha6
                or KeyCode.Alpha7 
                or KeyCode.Alpha8;
        }
    }
}