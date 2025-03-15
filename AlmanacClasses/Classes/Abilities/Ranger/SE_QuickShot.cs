using AlmanacClasses.LoadAssets;
using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public class SE_QuickShot : StatusEffect
{
    private readonly string m_key = "QuickShot";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        
        base.Setup(character);
    }
}

public static class QuickShot
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GetAttackDrawPercentage))]
    private static class Humanoid_GetAttackDrawPercentage_Patch
    {
        private static void Postfix(Humanoid __instance, ref float __result)
        {
            if (!__instance.IsPlayer()) return;
            ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
            if (currentWeapon.m_shared.m_skillType is not Skills.SkillType.Bows) return;
            if (!__instance.GetSEMan().HaveStatusEffect("SE_QuickShot".GetStableHashCode())) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("QuickShot", out Talent talent)) return;
            __result *= talent.GetSpeedModifier(talent.GetLevel());
        }
    }
    
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
        }
    }
}