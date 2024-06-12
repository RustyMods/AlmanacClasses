using System.Collections.Generic;
using HarmonyLib;

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
            __instance.Drop(__instance.m_itemPrefab, 1, (int)(__instance.m_amount * talent.GetForageModifier(talent.GetLevel())));
        }
    }
    private static bool IsForageItem(string prefabName, List<string> list) => list.Contains(prefabName);
}