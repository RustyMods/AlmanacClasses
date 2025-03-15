using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class Forager
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.RPC_Pick))]
    private static class Pickable_RPC_Pick_Prefix
    {
        private static void Prefix(Pickable __instance)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("Forager", out Talent talent)) return;
            if (!__instance.m_nview.IsOwner() || __instance.m_picked) return;
            if (!__instance.m_itemPrefab) return;
            if (!__instance.m_itemPrefab.TryGetComponent(out ItemDrop component)) return;
            if (component.m_itemData.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.Consumable)
            {
                if (!IsForageItem(__instance.m_itemPrefab.name, talent.GetCustomForageItems())) return;
            }
            
            var bonusChance = talent.GetForageModifier(talent.GetLevel()) - 1.0f;
            var guaranteedDrops = Mathf.FloorToInt(bonusChance);
            var extraDropChance = bonusChance % 1;
            
            // In the case of for example 250% bonus drop, we have 2 guaranteed drops
            // And 50% chance of a 3rd.
            for (var i = 0; i < guaranteedDrops; i++)
                __instance.Drop(__instance.m_itemPrefab, 1, __instance.m_amount);
            
            if (ClassUtilities.RandomBoolWithWeight(extraDropChance))
                __instance.Drop(__instance.m_itemPrefab, 1, __instance.m_amount);
        }
    }

    [HarmonyPatch(typeof(Pickable), nameof(Pickable.GetHoverText))]
    private static class Pickable_GetHoverText_Postfix
    {
        private static void Postfix(Pickable __instance, ref string __result)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("Forager", out Talent talent)) return;
            if (!__instance.m_nview.IsOwner() || __instance.m_picked) return;
            if (!__instance.m_itemPrefab) return;
            if (!__instance.m_itemPrefab.TryGetComponent(out ItemDrop component)) return;
            if (component.m_itemData.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.Consumable)
            {
                if (!IsForageItem(__instance.m_itemPrefab.name, talent.GetCustomForageItems())) return;
            }
            var bonusChance = (talent.GetForageModifier(talent.GetLevel()) - 1.0f) * 100.0f;
            __result += Localization.instance.Localize($"\n[{talent.GetName()} <color=orange>{talent.GetLevel()}</color>]: <color=orange>{Mathf.Round(bonusChance)}%</color> Double Drop");
        }
    }

    private static bool IsForageItem(string prefabName, List<string> list) => list.Contains(prefabName);
}