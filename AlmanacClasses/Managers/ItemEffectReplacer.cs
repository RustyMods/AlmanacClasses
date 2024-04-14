using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Managers;

public static class ItemEffectReplacer
{
    private static readonly Dictionary<string, GameObject> PrefabToSet = new();

    public static void RegisterPrefabForEffectClone(GameObject prefab, string originalPrefab)
    {
        PrefabToSet[originalPrefab] = prefab;
    }

    private static void CloneEffects()
    {
        foreach (var kvp in PrefabToSet)
        {
            GameObject original = ZNetScene.instance.GetPrefab(kvp.Key);
            if (!original) continue;
            if (!kvp.Value.TryGetComponent(out ItemDrop clone)) continue;
            if (!original.TryGetComponent(out ItemDrop component)) continue;
            clone.m_itemData.m_shared.m_hitEffect = component.m_itemData.m_shared.m_hitEffect;
            clone.m_itemData.m_shared.m_blockEffect = component.m_itemData.m_shared.m_blockEffect;
            clone.m_itemData.m_shared.m_startEffect = component.m_itemData.m_shared.m_startEffect;
            clone.m_itemData.m_shared.m_triggerEffect = component.m_itemData.m_shared.m_triggerEffect;
            clone.m_itemData.m_shared.m_hitTerrainEffect = component.m_itemData.m_shared.m_hitTerrainEffect;
            clone.m_itemData.m_shared.m_trailStartEffect = component.m_itemData.m_shared.m_trailStartEffect;
        }
    }

    // [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    // private static class ZNetScene_Awake_Patch
    // {
    //     private static void Postfix(ZNetScene __instance)
    //     {
    //         if (!__instance) return;
    //         CloneEffects();
    //     }
    // }

}