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
            if (!__instance.m_itemPrefab) return;
            if (!__instance.m_itemPrefab.TryGetComponent(out ItemDrop component)) return;
            if (component.m_itemData.m_shared.m_food <= 0f) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("Forager", out Talent talent)) return;
            __instance.Drop(__instance.m_itemPrefab, 1, (int)(__instance.m_amount * talent.GetForageModifier(talent.GetLevel())));
        }        
    }
}