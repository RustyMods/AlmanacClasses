using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Classes.Abilities.Ranger;
using AlmanacClasses.Classes.Abilities.Sage;
using AlmanacClasses.Classes.Abilities.Shaman;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx;
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
        if (AlmanacClassesPlugin._SpellAlt.Value is not KeyCode.None)
        {
            try
            {
                if (!Input.GetKey(AlmanacClassesPlugin._SpellAlt.Value)) return;
                CheckSpellKeys();
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to get ability");
            }
        }
        else
        {
            try
            {
                CheckSpellKeys();
            }
            catch
            {
                AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to get ability");
            }
        }
    }
    private static void CheckSpellKeys()
    {
        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell1.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(0, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell2.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(1, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell3.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(2, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell4.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(3, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell5.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(4, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell6.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(5, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell7.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(6, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell8.Value))
        {
            if (!SpellBook.m_abilities.TryGetValue(7, out AbilityData ability)) return;
            CastTalent(ability.m_data);
        }
    }
    private static void CastTalent(Talent ability)
    {
        if (m_castedSpells.Contains(ability.m_key))
        {
            if (!m_cooldownMap.TryGetValue(ability.m_key, out float cooldown)) return;
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{ability.m_key} $msg_casted, $msg_wait {(int)(cooldown * (ability.m_cooldown?.Value ?? 10f))} $msg_seconds");
            return;
        }
        if (!CheckCost(ability)) return;

        bool se = CheckStatusEffect(ability);
        bool action = CheckAbilityName(ability);
        if (!se && !action) return;
        AnimationManager.DoAnimation(ability.m_animation);
        AlmanacClassesPlugin._Plugin.StartCoroutine(CoolDown(ability));
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
            yield return new WaitForSeconds(1f);
            ++count;
        }

        m_castedSpells.Remove(ability.m_key);
        m_cooldownMap.Remove(ability.m_key);
    }
    private static bool CheckStatusEffect(Talent talent)
    {
        if (talent.m_type is not TalentType.StatusEffect) return false;
        if (talent.m_statusEffectHash == 0) return false;
        if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(talent.m_statusEffectHash)) return false;
        Player.m_localPlayer.GetSEMan().AddStatusEffect(talent.m_statusEffectHash);
        return true;
    }
    private static bool CheckAbilityName(Talent talent)
    {
        if (talent.m_type is not TalentType.Ability) return false;
        HitData.DamageTypes damages = talent.GetDamages(talent.GetLevel());
        switch (talent.m_ability)
        {
            case "TriggerStoneThrow":
                StoneThrow.TriggerStoneThrow(damages);
                break;
            case "TriggerRootBeam":
                RootBeam.TriggerRootBeam(damages);
                break;
            case "TriggerMeteor":
                MeteorStrike.TriggerMeteor(damages);
                break;
            case "TriggerLightningAOE":
                if (!CallOfLightning.TriggerLightningAOE(damages)) return false;
                break;
            case "TriggerGoblinBeam":
                GoblinBeam.TriggerGoblinBeam(damages);
                break;
            case "TriggerHunterSpawn":
                GameObject? rangerCreature = talent.GetCreatures(Player.m_localPlayer.GetCurrentBiome());
                if (rangerCreature == null) break;
                RangerSpawn.TriggerHunterSpawn(rangerCreature, talent);
                break;
            case "TriggerShamanSpawn":
                GameObject? creature = talent.GetCreature();
                if (creature == null) break;
                ShamanSpawn.TriggerShamanSpawn(creature, talent);
                break;
            case "TriggerSpawnTrap":
                RangerTrap.TriggerSpawnTrap(damages, talent.GetLength(talent.GetLevel()));
                break;
            case "TriggerHeal":
                if (!ShamanHeal.TriggerHeal(talent.GetHealAmount(talent.GetLevel()))) return false;
                break;
            case "TriggerIceBreath":
                IceBreath.TriggerIceBreath(damages);
                break;
        }
        return true;
    }
    private static bool CheckCost(Talent talent) => CheckEitrCost(talent) && CheckStaminaCost(talent) && CheckHealthCost(talent);
    private static bool CheckHealthCost(Talent talent)
    {
        float cost = talent.GetHealthCost();
        if (!Player.m_localPlayer.HaveHealth(cost))
        {
            Hud.instance.FlashHealthBar();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_hp_required");
            return false;
        }
        Player.m_localPlayer.UseHealth(cost);
        return true;
    }
    private static bool CheckStaminaCost(Talent talent)
    {
        float cost = talent.GetStaminaCost();
        if (!Player.m_localPlayer.HaveStamina(cost))
        {
            Hud.instance.StaminaBarEmptyFlash();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_stamina_required");
            return false;
        }
        Player.m_localPlayer.UseStamina(cost);
        return true;
    }
    private static bool CheckEitrCost(Talent talent)
    {
        float cost = talent.GetEitrCost();
        if (!Player.m_localPlayer.HaveEitr(cost))
        {
            Hud.instance.EitrBarEmptyFlash();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$hud_eitrrequired");
            return false;
        }
        Player.m_localPlayer.UseEitr(cost);
        return true;
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