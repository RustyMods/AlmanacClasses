using System;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class ShamanSpawn
{
    public static void TriggerShamanSpawn(GameObject fallBack, Talent talent)
    {
        if (!fallBack) return;
        if (!fallBack.GetComponent<Humanoid>()) return;
        var prefab = talent.GetCreaturesByLevel(talent.GetLevel()) ?? fallBack;
        if (!prefab.GetComponent<Humanoid>()) return;
        AlmanacClassesPlugin._Plugin.StartCoroutine(SpawnSystem.DelayedMultipleSpawn(
            prefab, "Friendly " + prefab.name.Replace("_", string.Empty), 
            talent.GetCreatureByLevelLevel(talent.GetLevel()), 
            talent.GetCreaturesByLevelLength(talent.GetLevel()), 
            talent.GetCreatureByLevelLevel(talent.GetLevel()))
        );
    }
}