using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Data;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

public static class AbilityManager
{
    private static readonly List<string> m_castedSpells = new();
    public static readonly Dictionary<string, float> m_cooldownMap = new();

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
            AbilityData? ability = SpellBook.m_abilities[0];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell2.Value))
        {
            if (SpellBook.m_abilities.ElementAtOrDefault(1) == null) return;
            var ability = SpellBook.m_abilities[1];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell3.Value))
        {
            var ability = SpellBook.m_abilities[2];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell4.Value))
        {
            var ability = SpellBook.m_abilities[3];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell5.Value))
        {
            var ability = SpellBook.m_abilities[4];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell6.Value))
        {
            var ability = SpellBook.m_abilities[5];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell7.Value))
        {
            var ability = SpellBook.m_abilities[6];
            CastTalent(ability.m_data);
        }

        if (Input.GetKeyDown(AlmanacClassesPlugin._Spell8.Value))
        {
            var ability = SpellBook.m_abilities[7];
            CastTalent(ability.m_data);
        }
    }

    private static void CastTalent(Talent ability)
    {
        if (m_castedSpells.Contains(ability.m_name))
        {
            if (!m_cooldownMap.TryGetValue(ability.m_name, out float cooldown)) return;
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{ability.m_name} $msg_casted, $msg_wait {(int)(cooldown * (ability.m_ttl?.Value ?? 10f))} $msg_seconds");
            return;
        }

        bool flag1 = CheckAbilityName(ability);
        bool flag2 = CheckEffect(ability);
        if (flag1 || flag2)
        {
            PlayerManager.m_tempPlayerData.m_experience += (ability.m_level * 10) * AlmanacClassesPlugin._ExperienceMultiplier.Value;
            AlmanacClassesPlugin._Plugin.StartCoroutine(CoolDown(ability));
        }
    }

    private static IEnumerator CoolDown(Talent ability)
    {
        m_castedSpells.Add(ability.m_name);
        m_cooldownMap[ability.m_name] = 1f;
        float count = 0;
        while (count < ability.m_ttl?.Value)
        {
            m_cooldownMap[ability.m_name] -= 1f / ability.m_ttl.Value;
            yield return new WaitForSeconds(1f);
            ++count;
        }

        m_castedSpells.Remove(ability.m_name);
        m_cooldownMap.Remove(ability.m_name);
    }

    private static bool CheckAbilityName(Talent talent)
    {
        if (talent.m_ability.IsNullOrWhiteSpace()) return false;
        if (!CheckCost(talent)) return false;
        HitData.DamageTypes damages = TalentManager.GetDamages(talent);
        switch (talent.m_ability)
        {
            // case "TriggerBlink":
            //     return Spells.TriggerBlink();
            //     break;
            case "TriggerStoneThrow":
                Spells.TriggerStoneThrow(damages);
                break;
            case "TriggerRootBeam":
                Spells.TriggerRootBeam(damages);
                break;
            case "TriggerMeteor":
                Spells.TriggerMeteor(damages);
                break;
            case "TriggerLightningAOE":
                return Spells.TriggerLightningAOE(damages);
            case "TriggerGoblinBeam":
                Spells.TriggerGoblinBeam(damages);
                break;
            case "TriggerHunterSpawn":
                GameObject rangerCreature = GetCreatureByBiome();
                if (!rangerCreature) break;
                SpawnSystem.TriggerHunterSpawn(rangerCreature, talent.m_level);
                break;
            case "TriggerShamanSpawn":
                GameObject creature = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._ShamanSpawn.Value.ToString());
                if (!creature) break;
                SpawnSystem.TriggerShamanSpawn(creature, talent.m_level);
                break;
            case "TriggerSpawnTrap":
                Spells.TriggerSpawnTrap(damages);
                break;
            case "TriggerHeal":
                float amount = (talent.m_heal?.Value ?? 200f) * talent.m_level;
                Spells.TriggerHeal(amount, talent.m_radius);
                break;
            case "TriggerIceBreath":
                Spells.TriggerIceBreath(damages);
                break;
        }
        Player.m_localPlayer.RaiseSkill(talent.m_skill);
        return true;
    }

    private static GameObject GetCreatureByBiome()
    {
        Heightmap.Biome biome = WorldGenerator.instance.GetBiome(Player.m_localPlayer.transform.position);
        GameObject creature;
        switch (biome)
        {
            case Heightmap.Biome.Meadows:
                GameObject Neck = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerMeadowSpawn.Value.ToString());
                creature = Neck;
                break;
            case Heightmap.Biome.BlackForest:
                GameObject Greydwarf = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerBlackForestSpawn.Value.ToString());
                creature = Greydwarf;
                break;
            case Heightmap.Biome.Swamp:
                GameObject Draugr = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerSwampSpawn.Value.ToString());
                creature = Draugr;
                break;
            case Heightmap.Biome.Mountain:
                GameObject Ulv = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerMountainSpawn.Value.ToString());
                creature = Ulv;
                break;
            case Heightmap.Biome.Plains:
                GameObject Deathsquito = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerPlainsSpawn.Value.ToString());
                creature = Deathsquito;
                break;
            case Heightmap.Biome.Mistlands:
                GameObject Seeker = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerMistLandsSpawn.Value.ToString());
                creature = Seeker;
                break;
            case Heightmap.Biome.Ocean:
                GameObject Serpent = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerOceanSpawn.Value.ToString());
                creature = Serpent;
                break;
            case Heightmap.Biome.AshLands:
                GameObject Surtling = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerAshlandSpawn.Value.ToString());
                creature = Surtling;
                break;
            case Heightmap.Biome.DeepNorth:
                GameObject cultist = ZNetScene.instance.GetPrefab(AlmanacClassesPlugin._RangerDeepNorthSpawn.Value.ToString());
                creature = cultist;
                break;
            default:
                GameObject SkeletonFriendly = LoadedAssets.SkeletonFriendly;
                creature = SkeletonFriendly;
                break;
        }

        return creature;
    }

    private static bool CheckEffect(Talent talent)
    {
        if (talent.m_effect == null) return false;
        if (!CheckCost(talent)) return false;
        if (talent.m_radius > 0)
        {
            List<Player> players = new();
            Player.GetPlayersInRange(Player.m_localPlayer.transform.position, talent.m_radius, players);
            foreach (Player? player in players)
            {
                player.GetSEMan().AddStatusEffect(talent.m_effect.name.GetStableHashCode());
            }
        }
        else
        {
            Player.m_localPlayer.GetSEMan().AddStatusEffect(talent.m_effect);
        }

        return true;
    }

    private static bool CheckCost(Talent talent)
    {
        if (!CheckEitrCost(talent)) return false;
        if (!CheckStaminaCost(talent)) return false;
        if (!CheckHealthCost(talent)) return false;
        
        if (!talent.m_animation.IsNullOrWhiteSpace())
        {
            if (AnimationManager.IsEmote(talent.m_animation))
            {
                if (Player.m_localPlayer.InEmote()) Player.m_localPlayer.StopEmote();
                Player.m_localPlayer.StartEmote(talent.m_animation);
            }
            else
            {
                Player.m_localPlayer.m_zanim.SetTrigger(talent.m_animation);
            }
        }
        return true;
    }

    private static bool CheckHealthCost(Talent talent)
    {
        if (talent.m_healthCost == null) return true;
        if (!Player.m_localPlayer.HaveHealth(talent.m_healthCost?.Value ?? 0f))
        {
            Hud.instance.FlashHealthBar();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_hp_required");
            return false;
        }
        Player.m_localPlayer.UseHealth(talent.m_healthCost?.Value ?? 0f);
        return true;
    }

    private static bool CheckStaminaCost(Talent talent)
    {
        if (talent.m_staminaCost == null) return true;
        if (!Player.m_localPlayer.HaveStamina(talent.m_staminaCost?.Value ?? 0f))
        {
            Hud.instance.StaminaBarEmptyFlash();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_stamina_required");
            return false;
        }
        Player.m_localPlayer.UseStamina(talent.m_staminaCost?.Value ?? 0f);
        return true;
    }

    private static bool CheckEitrCost(Talent talent)
    {
        if (talent.m_eitrCost == null) return true;
        if (!Player.m_localPlayer.HaveEitr(talent.m_eitrCost?.Value ?? 4f))
        {
            Hud.instance.EitrBarEmptyFlash();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$hud_eitrrequired");
            return false;
        }
        Player.m_localPlayer.UseEitr(talent.m_eitrCost?.Value ?? 4f);
        return true;
    }
}