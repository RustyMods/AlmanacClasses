using AlmanacClasses.Classes;
using AlmanacClasses.Data;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Patches;

public static class AttackPatches
{
    [HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
    private static class Attack_Start_Postfix_Patch
    {
        private static void Postfix(Attack __instance, Humanoid character, ref bool __result)
        {
            if (!__result) return;
            if (!character.IsPlayer()) return;
            CheckLuckyShot(__instance, character);
        }
    }

    private static void CheckLuckyShot(Attack instance, Humanoid character)
    {
        if (instance.m_attackAnimation is not "bow_fire" or "crossbow_fire") return;
        if (!PlayerManager.m_playerTalents.TryGetValue("LuckyShot", out Talent talent)) return;
        ItemDrop.ItemData projectile = character.GetAmmoItem();
        if (projectile == null) return;
        if (talent.m_values == null) return;

        int percentage = (int)talent.GetChance(talent.GetLevel());
        int random = Random.Range(0, 101);
        if (random <= percentage)
        {
            ItemDrop.ItemData clone = projectile.Clone();
            clone.m_stack = 1;
            Player.m_localPlayer.GetInventory().AddItem(clone);
        }
    }
    
    [HarmonyPatch(typeof(Attack), nameof(Attack.Start))]
    private static class Attach_Start_Prefix_Patch
    {
        private static void Prefix(Attack __instance, Humanoid character)
        {
            if (!character.IsPlayer()) return;
            ModifyAttackAnimation(__instance, character);
        }
    }

    private static void ModifyAttackAnimation(Attack instance, Humanoid character)
    {
        if (!PlayerManager.m_playerTalents.ContainsKey("DualWield")) return;
        ItemDrop.ItemData right = character.GetRightItem();
        ItemDrop.ItemData left = character.GetLeftItem();
        if (right == null || left == null) return;
        if (right.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.OneHandedWeapon) return;
        if (left.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.OneHandedWeapon) return;
        string normalAttack = instance.m_attackAnimation;

        bool hasKnife = right.m_shared.m_skillType is Skills.SkillType.Knives || left.m_shared.m_skillType is Skills.SkillType.Knives;
        bool hasAxes = right.m_shared.m_skillType is Skills.SkillType.Axes || left.m_shared.m_skillType is Skills.SkillType.Axes;
        // instance.m_attackAnimation = normalAttack.EndsWith("_secondary") ? "dual_knives_secondary" : "dual_knives";
        instance.m_attackAnimation = normalAttack.EndsWith("_secondary") ? hasAxes ? "dualaxes_secondary" : "dual_knives_secondary" : hasKnife ? "dual_knives" : "dualaxes";
        instance.m_attackChainLevels = normalAttack.EndsWith("_secondary") ? 1 : 4;
    }
}