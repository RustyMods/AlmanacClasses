using System.Collections.Generic;
using System.Linq;

namespace AlmanacClasses.LoadAssets;

public static class LoadTwoHanded
{
    private static readonly Dictionary<ItemDrop, TwoHandedData> TwoHandedWeapons = new();

    private static readonly List<string> WarfareItems = new()
    {
        "DualAxeKrom_TW",
        "DualHammerRageHatred_TW",
        "DualSpearSvigaFrekk_TW",
        "DualSwordSkadi_TW",
        "DualHammerStormstrike_TW",
        "DualSwordScimitar_TW",
        "DualaxeDemonic_TW",
    };

    public static void InitTwoHandedWeapons()
    {
        List<ItemDrop> items = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.TwoHandedWeapon, "");
        foreach (ItemDrop item in items)
        {
            if (item.m_itemData.m_shared.m_icons.Length <= 0) continue;
            if (item.m_itemData.m_shared.m_itemType is ItemDrop.ItemData.ItemType.Tool) continue;
            if (WarfareItems.Contains(item.name)) continue;
            if (item.m_itemData.m_shared.m_skillType is Skills.SkillType.Swords or Skills.SkillType.Axes or Skills.SkillType.Polearms || item.m_itemData.m_shared.m_name == "$item_stafffireball")
            {
                TwoHandedData data = new()
                {
                    m_type = item.m_itemData.m_shared.m_itemType,
                    m_attachOverride = item.m_itemData.m_shared.m_attachOverride,
                    m_animationState = item.m_itemData.m_shared.m_animationState,
                    m_attackType = item.m_itemData.m_shared.m_attack.m_attackType,
                    m_attackAnimation = item.m_itemData.m_shared.m_attack.m_attackAnimation,
                    m_secondaryAttackType = item.m_itemData.m_shared.m_secondaryAttack.m_attackType,
                    m_secondaryAttackAnimation = item.m_itemData.m_shared.m_secondaryAttack.m_attackAnimation
                };
                TwoHandedWeapons[item] = data;
            }
        }
        
    }

    public static void ModifyTwoHandedWeapons()
    {
        foreach (KeyValuePair<ItemDrop, TwoHandedData> kvp in TwoHandedWeapons)
        {
            kvp.Key.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.OneHandedWeapon;
            kvp.Key.m_itemData.m_shared.m_attachOverride = ItemDrop.ItemData.ItemType.OneHandedWeapon;
            kvp.Key.m_itemData.m_shared.m_animationState = ItemDrop.ItemData.AnimationState.OneHanded;

            switch (kvp.Key.m_itemData.m_shared.m_skillType)
            {
                case Skills.SkillType.Axes:
                    ChangeToOneHandedAxe(kvp.Key.m_itemData);
                    break;
                case Skills.SkillType.Swords or Skills.SkillType.Polearms:
                    ChangeToOneHandedSword(kvp.Key.m_itemData);
                    break;
            }
            
            AddEquipStatusEffect(kvp.Key.m_itemData);
        }

        if (!Player.m_localPlayer) return;
        Inventory? inventory = Player.m_localPlayer.GetInventory();
        foreach (ItemDrop.ItemData? item in inventory.m_inventory)
        {
            if (TwoHandedWeapons.Keys.ToList().Find(x => x.m_itemData.m_shared.m_name == item.m_shared.m_name) == null) continue;
            item.m_shared.m_itemType = ItemDrop.ItemData.ItemType.OneHandedWeapon;
            item.m_shared.m_attachOverride = ItemDrop.ItemData.ItemType.OneHandedWeapon;
            item.m_shared.m_animationState = ItemDrop.ItemData.AnimationState.OneHanded;

            switch (item.m_shared.m_skillType)
            {
                case Skills.SkillType.Axes:
                    ChangeToOneHandedAxe(item);
                    break;
                case Skills.SkillType.Swords or Skills.SkillType.Polearms:
                    ChangeToOneHandedSword(item);
                    break;
            }
            
            AddEquipStatusEffect(item);
        }
    }

    private static void ChangeToOneHandedAxe(ItemDrop.ItemData item)
    {
        item.m_shared.m_attack.m_attackType = Attack.AttackType.Horizontal;
        item.m_shared.m_attack.m_attackAnimation = "swing_axe";
        item.m_shared.m_secondaryAttack.m_attackType = Attack.AttackType.Vertical;
        item.m_shared.m_secondaryAttack.m_attackAnimation = "axe_secondary";
    }

    private static void ChangeToOneHandedSword(ItemDrop.ItemData item)
    {
        item.m_shared.m_attack.m_attackType = Attack.AttackType.Horizontal;
        item.m_shared.m_attack.m_attackAnimation = "swing_longsword";
        item.m_shared.m_secondaryAttack.m_attackType = Attack.AttackType.Horizontal;
        item.m_shared.m_secondaryAttack.m_attackAnimation = "sword_secondary";
    }

    private static void AddEquipStatusEffect(ItemDrop.ItemData item)
    {
        if (item.m_shared.m_equipStatusEffect) return;
        item.m_shared.m_equipStatusEffect = ObjectDB.instance.GetStatusEffect("SE_MonkeyWrench".GetStableHashCode());
    }

    public static void ResetTwoHandedWeapons()
    {
        foreach (KeyValuePair<ItemDrop, TwoHandedData> kvp in TwoHandedWeapons)
        {
            kvp.Key.m_itemData.m_shared.m_itemType = kvp.Value.m_type;
            kvp.Key.m_itemData.m_shared.m_attachOverride = kvp.Value.m_attachOverride;
            kvp.Key.m_itemData.m_shared.m_animationState = kvp.Value.m_animationState;
            kvp.Key.m_itemData.m_shared.m_attack.m_attackType = kvp.Value.m_attackType;
            kvp.Key.m_itemData.m_shared.m_attack.m_attackAnimation = kvp.Value.m_attackAnimation;
            kvp.Key.m_itemData.m_shared.m_secondaryAttack.m_attackType = kvp.Value.m_secondaryAttackType;
            kvp.Key.m_itemData.m_shared.m_secondaryAttack.m_attackAnimation = kvp.Value.m_secondaryAttackAnimation;
            if (kvp.Key.m_itemData.m_shared.m_equipStatusEffect && kvp.Key.m_itemData.m_shared.m_equipStatusEffect.name == "SE_MonkeyWrench")
            {
                kvp.Key.m_itemData.m_shared.m_equipStatusEffect = null;
            }
        }

        if (!Player.m_localPlayer) return;
        Inventory? inventory = Player.m_localPlayer.GetInventory();
        foreach (ItemDrop.ItemData? item in inventory.m_inventory)
        {
            ItemDrop itemDrop = TwoHandedWeapons.Keys.ToList().Find(x => x.m_itemData.m_shared.m_name == item.m_shared.m_name);
            if (itemDrop == null) continue;
            if (!TwoHandedWeapons.TryGetValue(itemDrop, out TwoHandedData data)) continue;
            
            item.m_shared.m_itemType = data.m_type;
            item.m_shared.m_attachOverride = data.m_attachOverride;
            item.m_shared.m_animationState = data.m_animationState;

            item.m_shared.m_attack.m_attackType = data.m_attackType;
            item.m_shared.m_attack.m_attackAnimation = data.m_attackAnimation;
            item.m_shared.m_secondaryAttack.m_attackType = data.m_secondaryAttackType;
            item.m_shared.m_secondaryAttack.m_attackAnimation = data.m_secondaryAttackAnimation;
            if (item.m_shared.m_equipStatusEffect && item.m_shared.m_equipStatusEffect.name == "SE_MonkeyWrench")
            {
                item.m_shared.m_equipStatusEffect = null;
            }
        }
    }

    private class TwoHandedData
    {
        public ItemDrop.ItemData.ItemType m_type = ItemDrop.ItemData.ItemType.TwoHandedWeapon;
        public ItemDrop.ItemData.ItemType m_attachOverride;
        public ItemDrop.ItemData.AnimationState m_animationState;

        public Attack.AttackType m_attackType;
        public string m_attackAnimation = "";
        public Attack.AttackType m_secondaryAttackType;
        public string m_secondaryAttackAnimation = "";
    }
}