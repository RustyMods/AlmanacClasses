using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class StoreGUIPatches
{
    [HarmonyPatch(typeof(StoreGui), nameof(StoreGui.IsVisible))]
    private static class StoreGUI_IsVisible_Patch
    {
        private static void Postfix(ref bool __result) => __result |= SkillTree.IsPanelVisible();
    }
}