using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities.Core;
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

    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    private static class Player_GetTotalFoodValue_Postfix
    {
        private static void Postfix(Player __instance, ref float hp, ref float stamina, ref float eitr)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("MasterChef", out Talent ability)) return;
            float modifier = ability.GetFoodModifier(ability.GetLevel());
            hp = __instance.m_baseHP;
            stamina = __instance.m_baseStamina;
            eitr = 0.0f;
            foreach (Player.Food? food in __instance.m_foods)
            {
                hp += food.m_health *= modifier;
                stamina += food.m_stamina *= modifier;
                eitr += food.m_eitr *= modifier;
            }
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
            TalentManager.InitializeTalents();
            PlayerManager.InitPlayerTalents();
            PlayerManager.AddPassiveStatusEffects(__instance);
            if (PlayerManager.m_playerTalents.ContainsKey("MonkeyWrench"))
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
            PlayerManager.AddPassiveStatusEffects(__instance);
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    private static class Player_Update_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!__instance) return;
            if (__instance != Player.m_localPlayer) return;
            AirBender.CheckDoubleJump(__instance);
        }
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