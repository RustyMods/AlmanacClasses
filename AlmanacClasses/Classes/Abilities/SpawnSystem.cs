using System.Collections;
using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

public static class SpawnSystem
{
    private static Humanoid? SpawnedCreature;
    private static readonly List<Humanoid> SpawnedCreatures = new();
    public static readonly int FriendlyKey = "FriendlyKey".GetStableHashCode();

    public static void TriggerShamanSpawn(GameObject creature, int level)
    {
        if (!creature) return;
        if (!creature.GetComponent<Humanoid>()) return;
        StartSpawnAnimation();
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedMultipleSpawn(creature, "Friendly " + creature.name.Replace("_", string.Empty), level));
    }
    
    public static void TriggerHunterSpawn(GameObject creature, int level)
    {
        if (!creature) return;
        if (!creature.GetComponent<Humanoid>()) return;
        StartSpawnAnimation();
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedSpawn(creature, "Friendly " + creature.name.Replace("_", " "), level));
    }

    private static IEnumerator DelayedMultipleSpawn(GameObject creature, string name, int level)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Humanoid? humanoid in SpawnedCreatures)
        {
            humanoid.SetHealth(0);
        }
        SpawnedCreatures.Clear();
        Vector3 location = Player.m_localPlayer.GetLookDir() * 5f + Player.m_localPlayer.transform.position;
        int max = 3;
        int count = 0;
        while (count < max)
        {
            var random = Random.insideUnitSphere * 5f;
            Vector3 pos = location + new Vector3(random.x, 10f, random.z);
            ZoneSystem.instance.GetSolidHeight(pos, out float height, 1000);
            if (height >= 0.0 && Mathf.Abs(height - pos.y) <= 2f && Vector3.Distance(location, Player.m_localPlayer.transform.position) >= 2f)
            {
                pos.y = height;
            }
            LoadedAssets.StaffPreSpawnEffects.Create(pos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            GameObject critter = Object.Instantiate(creature, pos, Quaternion.identity);
            SetSpawnHumanoid(critter, name, level, true);
            SetSpawnTameable(critter, name);
            SetSpawnZSyncAnimation(critter);
            SetSpawnBaseAI(critter);
            RemoveCharacterDrops(critter);
            ++count;
        }

        yield return new WaitForSeconds(15f * level);
        foreach (Humanoid? humanoid in SpawnedCreatures)
        {
            humanoid.SetHealth(0);
        }
        SpawnedCreatures.Clear();
    }
    private static IEnumerator DelayedSpawn(GameObject creature, string name, int level)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 location = SetSpawnLocation();
        LoadedAssets.StaffPreSpawnEffects.Create(location, Quaternion.identity);
        yield return new WaitForSeconds(2.5f);
        DestroySpawnedCreature();
        GameObject critter = Object.Instantiate(creature, location, Quaternion.identity);
        SetSpawnHumanoid(critter, name, level);
        SetSpawnTameable(critter, name);
        SetSpawnZSyncAnimation(critter);
        SetSpawnBaseAI(critter);
        RemoveCharacterDrops(critter);

        yield return new WaitForSeconds(50f * level);
        DestroySpawnedCreature();
    }

    private static void DestroySpawnedCreature()
    {
        if (SpawnedCreature != null) SpawnedCreature.SetHealth(0);
    }
    private static void SetSpawnHumanoid(GameObject creature, string name, int level, bool multiple = false)
    {
        if (creature.TryGetComponent(out Humanoid component))
        {
            if (!multiple) SpawnedCreature = component;
            else SpawnedCreatures.Add(component);
            component.name = name.Replace(" ", string.Empty);
            component.m_name = name;
            component.SetLevel(Mathf.Clamp(level, 0, 3));
            component.m_faction = Character.Faction.Players;
            component.m_boss = false;
        }

        if (!creature.TryGetComponent(out ZNetView znv)) return;
        if (!znv.IsValid()) return;
        znv.GetZDO().Set(FriendlyKey, true);
        znv.GetZDO().Set(ZDOVars.s_tamedName, name);
    }
    
    private static void StartSpawnAnimation()
    {
        if (Player.m_localPlayer.InEmote()) Player.m_localPlayer.StopEmote();
        Player.m_localPlayer.m_zanim.SetTrigger("staff_summon");
        LoadedAssets.FX_SummonSkeleton.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
    }
    private static Vector3 SetSpawnLocation()
    {
        Vector3 location = Player.m_localPlayer.GetLookDir() * 5f + Player.m_localPlayer.transform.position;
        ZoneSystem.instance.GetSolidHeight(location, out float height, 1000);
        if (height >= 0.0 && Mathf.Abs(height - location.y) <= 2f && Vector3.Distance(location, Player.m_localPlayer.transform.position) >= 2f)
        {
            location.y = height;
        }

        return location;
    }
    private static void SetSpawnZSyncAnimation(GameObject creature)
    {
        if (creature.TryGetComponent(out ZSyncAnimation syncAnimation))
        {
            syncAnimation.SetBool("wakeup", true);
        }
    }
    private static void SetSpawnBaseAI(GameObject creature)
    {
        if (creature.TryGetComponent(out BaseAI baseAI))
        {
            baseAI.Alert();
            baseAI.SetAggravated(baseAI.IsAggravated(), BaseAI.AggravatedReason.Damage);
            baseAI.m_passiveAggresive = true;
        }
    }
    public static void SetSpawnTameable(GameObject creature, string name, bool patch = false)
    {
        if (creature.TryGetComponent(out Tameable component))
        {
            if (!patch) component.Command(Player.m_localPlayer, false);
            component.SetText(name);
        }
        else
        {
            Tameable tameable = creature.AddComponent<Tameable>();
            tameable.m_fedDuration = 30f;
            tameable.m_tamingTime = 1800f;
            tameable.m_startsTamed = true;
            tameable.m_unsummonDistance = 150f;
            tameable.m_unsummonOnOwnerLogoutSeconds = 120f;
            tameable.m_sootheEffect = LoadedAssets.SoothEffects;
            tameable.m_unSummonEffect = LoadedAssets.UnSummonEffects;
            tameable.m_levelUpOwnerSkill = Skills.SkillType.BloodMagic;
            tameable.m_levelUpFactor = 0.5f;
            tameable.m_commandable = true;
            if(!patch) tameable.Command(Player.m_localPlayer, false);
            tameable.SetText(name);
        }
        
    }
    public static void RemoveCharacterDrops(GameObject creature)
    {
        if (creature.TryGetComponent(out CharacterDrop characterDrop))
        {
            characterDrop.m_drops = new();
        }
    }

    public enum SpawnOptions
    {
        Neck, Boar, Greyling, Eikthyr,
        Greydwarf, Greydwarf_Elite, Greywarf_Shaman,
        Troll, Skeleton, Ghost, gd_king,
        Skeleton_Poison, Wraith, Leech, Draugr, Draugr_ranged,
        Draugr_Elite, Bonemass, Wolf, Ulv, Fenring, Fenring_Cultist,
        StoneGolem, Hatchling, Dragon, Bat, Goblin, GoblinShaman, GoblinBrute,
        GoblinBrute_Hildir, GoblinBruteBros, GoblinShaman_Hildir, 
        GoblinKing, Deathsquito, Seeker, SeekerBrute, SeekerBrood, 
        Gjall, Tick, Serpent, Surtling, Fenring_Cultist_Hildir, Skeleton_Hildir
    }
}