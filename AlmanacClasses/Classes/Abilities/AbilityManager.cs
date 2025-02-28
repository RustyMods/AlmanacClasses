using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes.Abilities.Warrior;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

public static class AbilityManager
{
    private static readonly List<string> m_castedSpells = new();
    public static readonly Dictionary<string, float> m_cooldownMap = new();
    public static bool OnCooldown() => m_cooldownMap.Count > 0;
    public static void CheckInput()
    {
        try
        {
            if (AlmanacClassesPlugin._SpellAlt.Value is not KeyCode.None)
            {
                if (!Input.GetKey(AlmanacClassesPlugin._SpellAlt.Value)) return;
                CheckSpellKeys();
            }
            else
            {
                CheckSpellKeys();
            }
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to get ability");
        }
        
    }
    private static void CheckSpellKeys()
    {
        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell1.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(0, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell2.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(1, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell3.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(2, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell4.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(3, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell5.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(4, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell6.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(5, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell7.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(6, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell8.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(7, out SpellBook.AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._MonkeyWrenchToggle.Value))
        {
            if (PlayerManager.m_playerTalents.TryGetValue("MonkeyWrench", out var talent) is false) return;
            ToggleMonkeyWrench(talent);
        }
    }
    private static void ToggleMonkeyWrench(Talent talent)
    {
        var passiveIsActive = talent.m_passiveActive;
        if (passiveIsActive)
            MonkeyWrench.ResetTwoHandedWeapons();
        else
            MonkeyWrench.ModifyTwoHandedWeapons();
                    
        PlayerManager.RefreshCurrentWeapon();
        talent.m_passiveActive = !passiveIsActive;
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"MonkeyWrench was {((talent.m_passiveActive) ? "activated" : "deactivated")}");
    }
    private static void CastTalent(Talent ability)
    {
        if (!CheckCooldown(ability)) return;
        if (!CheckCost(ability)) return;
        if (!CheckStatusEffect(ability) && !CheckAbilityName(ability)) return;
        UseCost(ability.GetHealthCost(), ability.GetStaminaCost(), ability.GetEitrCost(false));
        AnimationManager.DoAnimation(ability.GetAnimation());
        AlmanacClassesPlugin._Plugin.StartCoroutine(CoolDown(ability));
    }

    private static bool CheckCooldown(Talent ability)
    {
        if (!m_castedSpells.Contains(ability.m_key)) return true;
        if (!m_cooldownMap.TryGetValue(ability.m_key, out float cooldown)) return true;
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{ability.GetName()} $msg_casted, $msg_wait {(int)(cooldown * ability.GetCooldown(ability.GetLevel()))} $msg_seconds");
        return false;

    }
    private static IEnumerator CoolDown(Talent ability)
    {
        m_castedSpells.Add(ability.m_key);
        m_cooldownMap[ability.m_key] = 1f;
        float count = 0;
        float cooldown = ability.GetCooldown(ability.GetLevel());
        while (count < cooldown)
        {
            m_cooldownMap[ability.m_key] -= 1f / cooldown;
            SpellBook.UpdateCooldownDisplayForAbility(ability);
            yield return new WaitForSeconds(1f);
            ++count;
        }

        m_castedSpells.Remove(ability.m_key);
        m_cooldownMap.Remove(ability.m_key);
    }
    private static bool CheckStatusEffect(Talent talent)
    {
        if (talent.m_type is not TalentType.StatusEffect) return false;
        if (talent.m_status is not { } status) return false;
        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(status.NameHash())) return false;
        Player.m_localPlayer.GetSEMan().AddStatusEffect(status.NameHash());
        return true;
    }
    private static bool CheckAbilityName(Talent talent)
    {
        if (talent.m_type is not TalentType.Ability) return false;
        return talent.m_ability is { } action && action.Invoke();
    }
    private static bool CheckCost(Talent talent) => CheckEitrCost(talent) && CheckStaminaCost(talent) && CheckHealthCost(talent);

    private static void UseCost(float healthCost, float staminaCost, float eitrCost)
    {
        if (healthCost > 0f) Player.m_localPlayer.UseHealth(healthCost);
        if (staminaCost > 0f) Player.m_localPlayer.UseStamina(staminaCost);
        if (eitrCost > 0f) Player.m_localPlayer.UseEitr(eitrCost);
    }
    private static bool CheckHealthCost(Talent talent)
    {
        float cost = talent.GetHealthCost();
        if (cost == 0f) return true;
        if (Player.m_localPlayer.HaveHealth(cost)) return true;
        Hud.instance.FlashHealthBar();
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_hp_required");
        return false;
    }
    private static bool CheckStaminaCost(Talent talent)
    {
        float cost = talent.GetStaminaCost();
        if (cost == 0f) return true;
        if (Player.m_localPlayer.HaveStamina(cost)) return true;
        Hud.instance.StaminaBarEmptyFlash();
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_stamina_required");
        return false;
    }
    private static bool CheckEitrCost(Talent talent)
    {
        float cost = talent.GetEitrCost(false);
        if (cost == 0f) return true;
        // The HaveEitr method checks if the player's eitr is > the amount of eitr passed in instead of >=
        // Pass in the cost - 1 to allow using abilities when the player has the exact amount required
        if (Player.m_localPlayer.HaveEitr(cost - 1)) return true;
        Hud.instance.EitrBarEmptyFlash();
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$hud_eitrrequired");
        return false;
    }
    
    [HarmonyPatch(typeof(Player), nameof(Player.UseHotbarItem))]
    private static class Player_HotBar_Override
    {
        private static bool Prefix() => !AreKeysAlpha() || !Input.GetKey(AlmanacClassesPlugin._SpellAlt.Value);
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