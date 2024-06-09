using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public static class RangerSpawn
{
    public static void TriggerHunterSpawn(GameObject creature, Talent talent)
    {
        if (!creature) return;
        if (!creature.GetComponent<Humanoid>()) return;
        SpawnSystem.StartSpawnAnimation();
        AlmanacClassesPlugin._Plugin.StartCoroutine(SpawnSystem.DelayedSpawn(creature, "Friendly " + creature.name.Replace("_", " "), talent.GetLevel(), talent.GetLength()));
    }

    public static void TriggerHunterSpawn(GameObject creature, int level)
    {
        if (!creature) return;
        if (!creature.GetComponent<Humanoid>()) return;
        SpawnSystem.StartSpawnAnimation();
        AlmanacClassesPlugin._Plugin.StartCoroutine(SpawnSystem.DelayedSpawn(creature,
            "Friendly " + creature.name.Replace("_", " "), level, 50f));
    }
}