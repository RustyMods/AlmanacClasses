using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_DualWield : StatusEffect
{
    private readonly string m_key = "DualWield";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.ApplyModifier(m_talent.GetDamageReduction(m_talent.GetLevel()));
    }
}

public static class DualWield
{
    private static ItemDrop.ItemData? rightItem;
    private static ItemDrop.ItemData? leftItem;
    private static bool isDualWielding;
    
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
    
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    private static class Humanoid_EquipItem_Patch
    {
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects, ref bool __result)
        {
            if (!__instance.IsPlayer()) return true;
            if (__instance.GetRightItem() == null) return true;
            if (item.m_shared.m_name == "$item_spear_chitin") return true;
            if (item.m_shared.m_itemType != ItemDrop.ItemData.ItemType.OneHandedWeapon) return true;
            if (!PlayerManager.m_playerTalents.TryGetValue("DualWield", out Talent talent)) return true;
            if (__instance.GetLeftItem() != null)
            {
                __instance.UnequipItem(__instance.GetLeftItem(), false);
            }

            __instance.m_leftItem = item;
            __instance.m_leftItem.m_equipped = true;
            __instance.m_hiddenLeftItem = null;
            __instance.m_hiddenRightItem = null;
                
            rightItem = __instance.GetRightItem();
            leftItem = item;

            rightItem.m_shared.m_attack.m_attackStamina += leftItem.m_shared.m_attack.m_attackStamina;
            rightItem.m_shared.m_attack.m_attackEitr += leftItem.m_shared.m_attack.m_attackEitr;
            rightItem.m_shared.m_attack.m_attackHealth += leftItem.m_shared.m_attack.m_attackHealth;
            rightItem.m_shared.m_attack.m_attackHealthPercentage += leftItem.m_shared.m_attack.m_attackHealthPercentage;

            rightItem.m_shared.m_secondaryAttack.m_attackStamina += leftItem.m_shared.m_secondaryAttack.m_attackStamina;
            rightItem.m_shared.m_secondaryAttack.m_attackEitr += leftItem.m_shared.m_secondaryAttack.m_attackEitr;
            rightItem.m_shared.m_secondaryAttack.m_attackHealth += leftItem.m_shared.m_secondaryAttack.m_attackHealth;
            rightItem.m_shared.m_secondaryAttack.m_attackHealthPercentage += leftItem.m_shared.m_secondaryAttack.m_attackHealthPercentage;
                
            rightItem.m_shared.m_damages.m_blunt += leftItem.m_shared.m_damages.m_blunt / 2;
            rightItem.m_shared.m_damages.m_slash += leftItem.m_shared.m_damages.m_slash / 2;
            rightItem.m_shared.m_damages.m_pierce += leftItem.m_shared.m_damages.m_pierce / 2;
            rightItem.m_shared.m_damages.m_chop += leftItem.m_shared.m_damages.m_chop;
            rightItem.m_shared.m_damages.m_pickaxe += leftItem.m_shared.m_damages.m_pickaxe;
            rightItem.m_shared.m_damages.m_fire += leftItem.m_shared.m_damages.m_fire / 2;
            rightItem.m_shared.m_damages.m_frost += leftItem.m_shared.m_damages.m_frost / 2;
            rightItem.m_shared.m_damages.m_lightning += leftItem.m_shared.m_damages.m_lightning / 2;
            rightItem.m_shared.m_damages.m_poison += leftItem.m_shared.m_damages.m_poison / 2;
            rightItem.m_shared.m_damages.m_spirit += leftItem.m_shared.m_damages.m_spirit / 2;

            if (!leftItem.m_shared.m_equipStatusEffect)
                leftItem.m_shared.m_equipStatusEffect = ObjectDB.instance.GetStatusEffect(talent.m_statusEffectHash);

            isDualWielding = true;
            __result = true;

            if (__instance.IsItemEquiped(item)) item.m_equipped = true;
            __instance.SetupEquipment();
            if (triggerEquipEffects) __instance.TriggerEquipEffect(item);
            return false;
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UnequipItem))]
    private static class Humanoid_UnequipItem_Patch
    {
        private static void Postfix(ItemDrop.ItemData item)
        {
            if (isDualWielding && item == leftItem)
            {
                if (rightItem == null) return;
                rightItem.m_shared.m_attack.m_attackStamina -= leftItem.m_shared.m_attack.m_attackStamina;
                rightItem.m_shared.m_attack.m_attackEitr -= leftItem.m_shared.m_attack.m_attackEitr;
                rightItem.m_shared.m_attack.m_attackHealth -= leftItem.m_shared.m_attack.m_attackHealth;
                rightItem.m_shared.m_attack.m_attackHealthPercentage -= leftItem.m_shared.m_attack.m_attackHealthPercentage;
                
                rightItem.m_shared.m_secondaryAttack.m_attackStamina -= leftItem.m_shared.m_secondaryAttack.m_attackStamina;
                rightItem.m_shared.m_secondaryAttack.m_attackEitr -= leftItem.m_shared.m_secondaryAttack.m_attackEitr;
                rightItem.m_shared.m_secondaryAttack.m_attackHealth -= leftItem.m_shared.m_secondaryAttack.m_attackHealth;
                rightItem.m_shared.m_secondaryAttack.m_attackHealthPercentage -= leftItem.m_shared.m_secondaryAttack.m_attackHealthPercentage;
                
                rightItem.m_shared.m_damages.m_blunt -= leftItem.m_shared.m_damages.m_blunt / 2;
                rightItem.m_shared.m_damages.m_pierce -= leftItem.m_shared.m_damages.m_pierce / 2;
                rightItem.m_shared.m_damages.m_slash -= leftItem.m_shared.m_damages.m_slash / 2;
                rightItem.m_shared.m_damages.m_chop -= leftItem.m_shared.m_damages.m_chop;
                rightItem.m_shared.m_damages.m_pickaxe -= leftItem.m_shared.m_damages.m_pickaxe;
                rightItem.m_shared.m_damages.m_fire -= leftItem.m_shared.m_damages.m_fire / 2;
                rightItem.m_shared.m_damages.m_frost -= leftItem.m_shared.m_damages.m_frost / 2;
                rightItem.m_shared.m_damages.m_lightning -= leftItem.m_shared.m_damages.m_lightning / 2;
                rightItem.m_shared.m_damages.m_poison -= leftItem.m_shared.m_damages.m_poison / 2;
                rightItem.m_shared.m_damages.m_spirit -= leftItem.m_shared.m_damages.m_spirit / 2;

                if (leftItem.m_shared.m_equipStatusEffect && leftItem.m_shared.m_equipStatusEffect.name == "SE_DualWield")
                {
                    leftItem.m_shared.m_equipStatusEffect = null;
                }

                isDualWielding = false;
            }
        }
    }
}