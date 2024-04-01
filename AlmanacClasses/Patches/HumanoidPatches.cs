using AlmanacClasses.Classes;
using AlmanacClasses.Data;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class HumanoidPatches
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GetAttackDrawPercentage))]
    private static class Humanoid_GetAttackDrawPercentage_Patch
    {
        private static void Postfix(Humanoid __instance, ref float __result)
        {
            if (!__instance.IsPlayer()) return;
            if (!__instance.GetSEMan().HaveStatusEffect("SE_QuickShot".GetStableHashCode())) return;
            ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();
            if (currentWeapon.m_shared.m_skillType is Skills.SkillType.Bows or Skills.SkillType.Crossbows)
            {
                __result = 1f;
            }
        }
    }
    
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.Awake))]
    private static class Humanoid_Awake_Patch
    {
        private static void Postfix(Humanoid __instance)
        {
            if (!__instance) return;
            if (__instance.IsPlayer()) return;
            SetFriendlySpawns(__instance);
        }
    }

    private static void SetFriendlySpawns(Humanoid instance)
    {
        if (!instance.m_nview.IsValid()) return;
        if (!instance.m_nview.GetZDO().GetBool(Classes.Abilities.SpawnSystem.FriendlyKey)) return;
        instance.m_nview.GetZDO().Persistent = false;
        instance.m_faction = Character.Faction.Players;
        instance.m_name = "Friendly " + instance.name.Replace("_", " ").Replace("(Clone)","");
        instance.m_boss = false;
        Classes.Abilities.SpawnSystem.SetSpawnTameable(instance.gameObject, "Friendly " + instance.name.Replace("_", " ").Replace("(Clone)",""), true);
        Classes.Abilities.SpawnSystem.RemoveCharacterDrops(instance.gameObject);
    }

    private static ItemDrop.ItemData? rightItem;
    private static ItemDrop.ItemData? leftItem;
    private static bool isDualWielding;

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
    private static class Humanoid_EquipItem_Patch
    {
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects, ref bool __result)
        {
            if (!__instance.IsPlayer()) return true;
            if (__instance.GetRightItem() == null) return true;
            if (item.m_shared.m_itemType != ItemDrop.ItemData.ItemType.OneHandedWeapon) return true;
            if (!PlayerManager.m_playerTalents.TryGetValue("DualWield", out Talent ability)) return true;
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
            if (isDualWielding && (item == leftItem || item == leftItem))
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

                isDualWielding = false;
            }
        }
    }
}