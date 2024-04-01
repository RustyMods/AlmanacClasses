using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class InventoryGUIPatches
{
    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Awake))]
    private static class InventoryGUI_Awake_Patch
    {
        private static void Postfix(InventoryGui __instance)
        {
            if (!__instance) return;
            LoadUI.InitSkillTree(__instance);
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Hide))]
    private static class InventoryGUI_Hide_Patch
    {
        private static bool Prefix() => !TalentBook.IsTalentBookVisible();
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateCharacterStats))]
    private static class InventoryGUI_UpdateCharacterStats_Patch
    {
        private static void Postfix(InventoryGui __instance)
        {
            if (!__instance) return;
            int level = PlayerManager.GetPlayerLevel(PlayerManager.m_tempPlayerData.m_experience);
            __instance.m_playerName.text = Game.instance.GetPlayerProfile().GetName() + " LVL " + level;
        }
    }

}