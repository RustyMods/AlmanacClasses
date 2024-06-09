using AlmanacClasses.Classes;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class ItemDataPatches
{
    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetWeaponLoadingTime))]
    private static class ItemData_GetWeaponLoadingTime_Patch
    {
        private static void Postfix(ItemDrop.ItemData __instance, ref float __result)
        {
            if (!__instance.m_shared.m_attack.m_requiresReload) return;
            if (__instance.m_shared.m_skillType is not Skills.SkillType.Crossbows) return;
            if (!Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_QuickShot".GetStableHashCode())) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("QuickShot", out Talent talent)) return;
            __result /= talent.GetSpeedModifier(talent.GetLevel());
            
            // if (talent.m_chance != null)
            // {
            //     __result = Mathf.Clamp(__result * (talent.m_chance.Value * talent.m_level / 100f),0f, 10f); 
            // }
        }
    }
}