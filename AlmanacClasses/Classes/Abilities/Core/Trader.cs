using System.Collections.Generic;
using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class Trader
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.IsTeleportable))]
    private static class Player_IsTeleportable_Postfix 
    {
        private static void Postfix(Humanoid __instance, ref bool __result)
        {
            if (__result) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("Trader", out Talent talent)) return;
            Inventory? inventory = __instance.GetInventory();
            if (inventory.IsTeleportable()) return;

            foreach (ItemDrop.ItemData itemData in inventory.m_inventory)
            {
                if (itemData.m_shared.m_teleportable) continue;
                if (!IsTeleportable(itemData.m_shared.m_name, talent.m_level)) return;
            }

            __result = true;
        }
    }

    private static bool IsTeleportable(string sharedName, int talentLevel)
    {
        Dictionary<string, int> map = new()
        {
            ["$item_copperore"] = 1,
            ["$item_copperscrap"] = 1,
            ["$item_tinore"] = 1,
            ["$item_copper"] = 1,
            ["$item_tin"] = 1,
            ["$item_bronze"] = 1,
            ["$item_bronzescrap"] = 1,
            ["$item_ironscrap"] = 2,
            ["$item_ironore"] = 2,
            ["$item_iron"] = 2,
            ["$item_silverore"] = 3,
            ["$item_silver"] = 3,
            ["$item_dragonegg"] = 3,
            ["$item_blackmetalscrap"] = 4,
            ["$item_blackmetal"] = 4,
            ["$item_dvergrneedle"] = 5,
            ["$item_ironpit"] = 5
        };
        if (map.TryGetValue(sharedName, out int level))
        {
            return talentLevel >= level;
        }

        return true;
    }
}