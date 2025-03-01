using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.Managers;
using UnityEngine;

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
    private static readonly Dictionary<ItemDrop, TwoHandedData> m_itemDrop_to_data = new();
    private static readonly Dictionary<string, TwoHandedData> m_sharedName_to_data= new();
    public static bool IsMonkeyWrenchItem(string sharedName) => m_sharedName_to_data.Keys.Contains(sharedName);

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
    public static void Init()
    {
        List<ItemDrop> items = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.TwoHandedWeapon, "");
        foreach (ItemDrop item in items)
        {
            if (item.m_itemData.m_shared.m_icons.Length <= 0) continue;
            if (item.m_itemData.m_shared.m_itemType is ItemDrop.ItemData.ItemType.Tool) continue;
            if (IsDualItem(item.name.Replace("(Clone)",string.Empty))) continue;
            if (!IsCorrectSkillType(item.m_itemData.m_shared.m_skillType) && !IsAllowedStaff(item.m_itemData.m_shared.m_name)) continue;
            TwoHandedData _ = new TwoHandedData(item);
        }
    }
    public static void ModifyTwoHandedWeapons()
    {
        if (m_modified) return;
        foreach(var data in m_itemDrop_to_data.Values) data.Modify();
        if (!Player.m_localPlayer) return;
        Inventory? inventory = Player.m_localPlayer.GetInventory();
        foreach (ItemDrop.ItemData? item in inventory.m_inventory)
        {
            if (!IsMonkeyWrenchItem(item.m_shared.m_name)) continue;
            TwoHandedData.Modify(item);
        }

        m_modified = true;
    }
    public static void ResetTwoHandedWeapons()
    {
        foreach(var data in m_itemDrop_to_data.Values) data.Reset();
        if (!Player.m_localPlayer) return;
        Inventory? inventory = Player.m_localPlayer.GetInventory();
        foreach (ItemDrop.ItemData? item in inventory.m_inventory)
        {
            if (!m_sharedName_to_data.TryGetValue(item.m_shared.m_name, out TwoHandedData data)) continue;
            data.Reset(item);
        }

        m_modified = false;
    }
    private class TwoHandedData
    {
        private readonly ItemDrop m_item;
        private readonly ItemDrop.ItemData.ItemType m_type;
        private readonly ItemDrop.ItemData.ItemType m_attachOverride;
        private readonly ItemDrop.ItemData.AnimationState m_animationState;

        private readonly Attack.AttackType m_attackType;
        private readonly string m_attackAnimation;
        private readonly Attack.AttackType m_secondaryAttackType;
        private readonly string m_secondaryAttackAnimation;

        public void Reset() => Reset(m_item.m_itemData);
        
        public void Reset(ItemDrop.ItemData item)
        {
            item.m_shared.m_itemType = m_type;
            item.m_shared.m_attachOverride = m_attachOverride;
            item.m_shared.m_animationState = m_animationState;

            item.m_shared.m_attack.m_attackType = m_attackType;
            item.m_shared.m_attack.m_attackAnimation = m_attackAnimation;
            item.m_shared.m_secondaryAttack.m_attackType = m_secondaryAttackType;
            item.m_shared.m_secondaryAttack.m_attackAnimation = m_secondaryAttackAnimation;
            if (item.m_shared.m_equipStatusEffect && item.m_shared.m_equipStatusEffect.name == "SE_MonkeyWrench")
            {
                item.m_shared.m_equipStatusEffect = null;
            }
        }

        public void Modify() => Modify(m_item.m_itemData);
        
        public static void Modify(ItemDrop.ItemData item)
        {
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

        public TwoHandedData(ItemDrop item)
        {
            m_item = item;
            m_type = item.m_itemData.m_shared.m_itemType;
            m_attachOverride = item.m_itemData.m_shared.m_attachOverride;
            m_animationState = item.m_itemData.m_shared.m_animationState;
            m_attackType = item.m_itemData.m_shared.m_attack.m_attackType;
            m_attackAnimation = item.m_itemData.m_shared.m_attack.m_attackAnimation;
            m_secondaryAttackType = item.m_itemData.m_shared.m_secondaryAttack.m_attackType;
            m_secondaryAttackAnimation = item.m_itemData.m_shared.m_secondaryAttack.m_attackAnimation;

            m_itemDrop_to_data[item] = this;
            m_sharedName_to_data[item.m_itemData.m_shared.m_name] = this;
        }
    }
}