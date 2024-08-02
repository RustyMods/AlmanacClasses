using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Managers;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_MonkeyWrench : StatusEffect
{
    private readonly string m_key = "MonkeyWrench";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        m_name = talent.GetName();
        m_tooltip = talent.GetTooltip();
        // m_icon = SpriteManager.WarriorIcon;
        talent.m_passiveActive = true;
        base.Setup(character);
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        if (m_character is not Player player) return;
        if (player.GetCurrentWeapon() == null) return;
        if (!MonkeyWrench.IsMonkeyWrenchItem(player.GetCurrentWeapon().m_shared.m_name)) return;
        hitData.ApplyModifier(m_talent.GetDamageReduction(m_talent.GetLevel()));
    }

    public override void Stop()
    {
        m_talent.m_passiveActive = false;
        base.Stop();
    }
}

public static class MonkeyWrench
{
    private static bool m_modified;
    private static readonly Dictionary<ItemDrop, TwoHandedData> TwoHandedWeapons = new();
    public static bool IsMonkeyWrenchItem(string sharedName) => TwoHandedWeapons.Keys.ToList().Find(x => x.m_itemData.m_shared.m_name == sharedName) != null;

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

    private static readonly List<string> StaffItems = new()
    {
        "$item_stafffireball",
        "$item_staffshield",
        "$item_staffclusterbomb",
        "$item_staffgreenroots",
        "$item_staffredtroll"
    };

    public static bool IsDualItem(string name) => name.StartsWith("Dual") || IsBerzekr(name) || IsSkollAndHati(name);
    private static bool IsBerzekr(string name) => name.StartsWith("AxeBerzerkr") || name.StartsWith("$item_axe_berzerkr") ||  name is "AxeBerzerkr";
    private static bool IsSkollAndHati(string name) => name is "KnifeSkollAndHati" or "$item_knife_skollandhati";
    private static bool IsAllowedStaff(string sharedName) => StaffItems.Contains(sharedName);
    private static bool IsCorrectSkillType(Skills.SkillType type) => type is Skills.SkillType.Swords or Skills.SkillType.Axes or Skills.SkillType.Polearms;
    public static void InitTwoHandedWeapons()
    {
        List<ItemDrop> items = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.TwoHandedWeapon, "");
        foreach (ItemDrop item in items)
        {
            if (item.m_itemData.m_shared.m_icons.Length <= 0) continue;
            if (item.m_itemData.m_shared.m_itemType is ItemDrop.ItemData.ItemType.Tool) continue;
            if (IsDualItem(item.name.Replace("(Clone)",string.Empty))) continue;
            if (!IsCorrectSkillType(item.m_itemData.m_shared.m_skillType) && !IsAllowedStaff(item.m_itemData.m_shared.m_name)) continue;
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
    public static void ModifyTwoHandedWeapons()
    {
        if (m_modified) return;
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
            if (!IsMonkeyWrenchItem(item.m_shared.m_name)) continue;
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

        m_modified = true;
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
        return;
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

        m_modified = false;
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