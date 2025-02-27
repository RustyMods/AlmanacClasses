﻿using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public static class RangerSpawn
{
    public static void TriggerHunterSpawn(GameObject creature, Talent talent)
    {
        if (!creature) return;
        AlmanacClassesPlugin._Plugin.StartCoroutine(SpawnSystem.DelayedSpawn(creature, "Friendly " + creature.name.Replace("_", " "), talent.GetLevel(), talent.GetLength(talent.GetLevel())));
    }

    public static void TriggerHunterSpawn(GameObject creature, int level)
    {
        if (!creature || !creature.GetComponent<Humanoid>()) return;
        SpawnSystem.StartSpawnAnimation();
        AlmanacClassesPlugin._Plugin.StartCoroutine(SpawnSystem.DelayedSpawn(creature,
            "Friendly " + creature.name.Replace("_", " "), level, 50f));
    }
}