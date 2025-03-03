using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_DualWield : StatusEffect
{
    private readonly string m_key = "DualWield";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
            m_name = talent.GetName();
            m_tooltip = talent.GetTooltip();
        }
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
    private static string m_lastLeftItem = "";
    private static bool m_changeAttach;
    
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
        instance.m_attackAnimation = normalAttack.EndsWith("_secondary") ? hasAxes ? "dualaxes_secondary" : "dual_knives_secondary" : hasKnife ? "dual_knives" : "dualaxes";
        instance.m_attackChainLevels = normalAttack.EndsWith("_secondary") ? 1 : 4;
    }

    [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.AttachItem))]
    private static class AttachItem_Override
    {
        private static void Prefix(VisEquipment __instance, int itemHash, ref Transform joint)
        {
            if (!Player.m_localPlayer || !m_changeAttach) return;
            if (!PlayerManager.m_playerTalents.ContainsKey("DualWield")) return;
            if (joint != __instance.m_backMelee) return;
            if (ObjectDB.instance.GetItemPrefab(itemHash) is not { } item ||
                !item.TryGetComponent(out ItemDrop component)) return;
            if (component.m_itemData.m_shared.m_name != m_lastLeftItem) return;
            joint = __instance.m_backTool;
            m_lastLeftItem = "";
            m_changeAttach = false;
        }
    }
    
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    private static class Humanoid_EquipItem_Patch
    {
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects, ref bool __result)
        {
            if (!__instance.IsPlayer()) return true;
            if (__instance != Player.m_localPlayer) return true;
            if (!PlayerManager.m_playerTalents.TryGetValue("DualWield", out Talent talent)) return true;
            if (__instance.GetRightItem() == null) return true;
            if (__instance.GetRightItem().m_shared.m_itemType is not ItemDrop.ItemData.ItemType.OneHandedWeapon) return true;
            if (item.m_shared.m_name == "$item_spear_chitin") return true;
            if (MonkeyWrench.IsDualItem(item.m_shared.m_name)) return true;
            if (item.m_shared.m_itemType != ItemDrop.ItemData.ItemType.OneHandedWeapon) return true;

            if (item.m_shared.m_dlc.Length > 0 && !DLCMan.instance.IsDLCInstalled(item.m_shared.m_dlc))
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_dlcrequired");
                return false;
            }

            if (Game.m_worldLevel > 0 && item.m_worldLevel < Game.m_worldLevel &&
                item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Utility)
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_ng_item_too_low");
                return false;
            }
            
            var leftItem = __instance.GetLeftItem();
            if (leftItem != null)
            {
                var isShield = leftItem.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield;
                if (isShield) return true;
                __instance.UnequipItem(leftItem, false);
            }

            __instance.m_leftItem = item;
            __instance.m_leftItem.m_equipped = true;
            __instance.m_hiddenLeftItem = null;
            __instance.m_hiddenRightItem = null;
                
            rightItem = __instance.GetRightItem();
            leftItem = item;
            
            if (rightItem.m_shared.m_attachOverride is not ItemDrop.ItemData.ItemType.Tool && leftItem.m_shared.m_attachOverride is not ItemDrop.ItemData.ItemType.Tool)
            {
                m_changeAttach = true;
            }

            ImproveRightItem();
            talent.m_passiveActive = true;
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
        private static void Postfix(Humanoid __instance, ItemDrop.ItemData item)
        {
            if (__instance != Player.m_localPlayer) return;
            if (!isDualWielding) return;
            ResetRightItem();
            m_lastLeftItem = leftItem?.m_shared.m_name ?? "";
            if (item == rightItem && leftItem != null)
            {
                __instance.m_rightItem = leftItem;
                __instance.m_leftItem = null;
                __instance.SetupVisEquipment(__instance.m_visEquipment, false);
            }
            rightItem = null;
            leftItem = null;
            isDualWielding = false;
            if (!PlayerManager.m_playerTalents.TryGetValue("DualWield", out Talent talent)) return;
            if (talent.m_status is { } status && __instance.GetSEMan().RemoveStatusEffect(status.NameHash()))
                talent.m_passiveActive = false;
            // if (__instance.GetSEMan().RemoveStatusEffect(talent.m_statusEffectHash)) talent.m_passiveActive = false;
        }
    }

    private static void ResetRightItem()
    {
        if (rightItem == null || leftItem == null) return;
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
    }

    private static void ImproveRightItem()
    {
        if (rightItem == null || leftItem == null) return;
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
    }
}