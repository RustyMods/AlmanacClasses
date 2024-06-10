using AlmanacClasses.Classes;
using AlmanacClasses.Data;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Patches;

public static class HumanoidPatches
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.Awake))]
    private static class Humanoid_Awake_Patch
    {
        private static void Postfix(Humanoid __instance)
        {
            if (!__instance) return;
            if (__instance.IsPlayer()) return;
            SetFriendlySpawns(__instance);
        }
    }

    private static void SetFriendlySpawns(Humanoid instance)
    {
        if (!instance.m_nview.IsValid()) return;
        if (!instance.m_nview.GetZDO().GetBool(Classes.Abilities.SpawnSystem.FriendlyKey)) return;
        instance.m_nview.GetZDO().Persistent = false;
        instance.m_faction = Character.Faction.Players;
        instance.m_name = "Friendly " + instance.name.Replace("_", " ").Replace("(Clone)","");
        instance.m_boss = false;
        Classes.Abilities.SpawnSystem.SetSpawnTameable(instance.gameObject, "Friendly " + instance.name.Replace("_", " ").Replace("(Clone)",""), true);
        Classes.Abilities.SpawnSystem.RemoveCharacterDrops(instance.gameObject);
    }
}