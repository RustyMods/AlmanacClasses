using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class ShamanSpawn
{
    public static void TriggerShamanSpawn(GameObject creature, Talent talent)
    {
        if (!creature) return;
        if (!creature.GetComponent<Humanoid>()) return;
        SpawnSystem.StartSpawnAnimation();
        AlmanacClassesPlugin._Plugin.StartCoroutine(SpawnSystem.DelayedMultipleSpawn(creature, "Friendly " + creature.name.Replace("_", string.Empty), talent.GetLevel(), talent.GetLength(talent.GetLevel())));
    }
}