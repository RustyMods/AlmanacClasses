using System.Collections.Generic;
using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class Forager
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
    private static class Pickable_Interact_Postfix
    {
        private static void Postfix(Pickable __instance, ref bool __result)
        {
            if (!__result) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("Forager", out Talent talent)) return;
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