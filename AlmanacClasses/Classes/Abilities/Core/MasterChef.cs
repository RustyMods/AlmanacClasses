using System.Text;
using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class MasterChef
{
    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    private static class Player_GetTotalFoodValue_Prefix
    {
        private static void Prefix(Player __instance)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("MasterChef", out Talent ability)) return;
            float modifier = ability.GetFoodModifier(ability.GetLevel());

            // Runs during UpdateFood which resets all the food to item values before getting total
            // Then master chef iterates through the new list before GetTotalFoodValue
            // And multiplies by the modifier
            
            foreach (Player.Food food in __instance.m_foods)
            {
                food.m_health *= modifier;
                food.m_stamina *= modifier;
                food.m_eitr *= modifier;
            }
        }
    }

    [HarmonyPatch(typeof(ItemDrop.ItemData), nameof(ItemDrop.ItemData.GetTooltip), typeof(ItemDrop.ItemData),
        typeof(int), typeof(bool), typeof(float))]
    private static class AddExtraTooltip
    {
        private static void Postfix(ItemDrop.ItemData item, int qualityLevel, bool crafting, ref string __result)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("MasterChef", out Talent ability)) return;
            if (item.m_shared.m_itemType is not ItemDrop.ItemData.ItemType.Consumable) return;
            float modifier = ability.GetFoodModifier(ability.GetLevel());
            Player localPlayer = Player.m_localPlayer;

            StringBuilder stringBuilder = new StringBuilder();
            AddPrefix(stringBuilder, item, crafting, qualityLevel);

            if (item.m_shared.m_food > 0.0 || item.m_shared.m_foodStamina > 0.0 || item.m_shared.m_foodEitr > 0.0)
            {
                if (item.m_shared.m_food > 0.0)
                {
                    stringBuilder.AppendFormat(
                        "\n$item_food_health: <color=#ff8080ff>{0}</color> <color=#adff2f>+{2:0}</color> ($item_current:<color=yellow>{1:0}</color>)",
                        item.m_shared.m_food, localPlayer.GetMaxHealth(), item.m_shared.m_food * modifier - item.m_shared.m_food);
                }

                if (item.m_shared.m_foodStamina > 0.0)
                {
                    stringBuilder.AppendFormat(
                        "\n$item_food_stamina: <color=#ffff80ff>{0}</color> <color=#adff2f>+{2:0}</color> ($item_current:<color=yellow>{1:0}</color>)",
                        item.m_shared.m_foodStamina, localPlayer.GetMaxStamina(), item.m_shared.m_foodStamina * modifier - item.m_shared.m_foodStamina);
                }

                if (item.m_shared.m_foodEitr > 0.0)
                {
                    stringBuilder.AppendFormat(
                        "\n$item_food_eitr: <color=#9090ffff>{0}</color> <color=#adff2f>+{2:0}</color> ($item_current:<color=yellow>{1:0}</color>)",
                        item.m_shared.m_foodEitr, localPlayer.GetMaxEitr(), item.m_shared.m_foodEitr * modifier - item.m_shared.m_foodEitr);

                }

                if (item.m_shared.m_foodRegen > 0.0)
                {
                    stringBuilder.AppendFormat("\n$item_food_regen: <color=orange>{0} hp/tick</color>",
                        item.m_shared.m_foodRegen);
                }
            }
            
            AddPostfix(stringBuilder, localPlayer, item, qualityLevel);

            stringBuilder.Append($"\n\n<color=#adff2f>{ability.GetName()}</color>");
            stringBuilder.Append($"\n{ability.GetTooltip()}");
            __result = Localization.instance.Localize(stringBuilder.ToString());
        }
    }

    private static void AddPrefix(StringBuilder stringBuilder, ItemDrop.ItemData item, bool crafting, int qualityLevel)
    {
        stringBuilder.Append(item.m_shared.m_description + "\n");
        if (item.m_shared.m_dlc.Length > 0)
            stringBuilder.Append("\n<color=#00FFFF>$item_dlc</color>");
        if (item.m_worldLevel > 0)
            stringBuilder.Append("\n<color=orange>$item_newgameplusitem " +
                                 (item.m_worldLevel != 1 ? item.m_worldLevel.ToString() : "") + "</color>");
        if (item.m_crafterID != 0L)
            stringBuilder.AppendFormat("\n$item_crafter: <color=orange>{0}</color>",
                CensorShittyWords.FilterUGC(item.m_crafterName, UGCType.CharacterName,
                    playerId: item.m_crafterID));
        if (!item.m_shared.m_teleportable && !ZoneSystem.instance.GetGlobalKey(GlobalKeys.TeleportAll))
            stringBuilder.Append("\n<color=orange>$item_noteleport</color>");
        if (item.m_shared.m_value > 0)
            stringBuilder.Append($"\n$item_value: <color=orange>{item.GetValue()} ({item.m_shared.m_value})</color");
        stringBuilder.Append(item.m_shared.m_maxStackSize > 1
            ? $"\n$item_weight: <color=orange>{item.GetNonStackedWeight():0.0} ({item.GetWeight():0.0} $item_total)</color>"
            : $"\n$item_weight: <color=orange>{item.GetWeight():0.0}</color>");
        if (item.m_shared.m_useDurability)
        {
            stringBuilder.Append(crafting
                ? $"\n$item_durability: <color=orange>{item.GetMaxDurability()}</color>"
                : $"\n$item_durability: <color=orange>{(int)(item.GetDurabilityPercentage() * 100f)}%</color> <color=yellow>({item.m_durability}/{item.GetMaxDurability(qualityLevel)})</color>");
        }

        if (item.m_shared.m_canBeReparied && !crafting && item.m_shared.m_useDurability)
        {
            Recipe recipe = ObjectDB.instance.GetRecipe(item);
            if (recipe != null)
            {
                stringBuilder.Append($"\n$item_repairlevel: <color=orange>{recipe.m_minStationLevel}</color>");
            }
        }
    }

    private static void AddPostfix(StringBuilder stringBuilder, Player localPlayer, ItemDrop.ItemData item, int qualityLevel)
    {
        float skillLevel = localPlayer.GetSkillLevel(item.m_shared.m_skillType);
        string statusEffectTooltip1 = item.GetStatusEffectTooltip(qualityLevel, skillLevel);
        if (statusEffectTooltip1.Length > 0)
            stringBuilder.Append("\n\n" + statusEffectTooltip1);
            
        string chainTooltip = item.GetChainTooltip(qualityLevel, skillLevel);
        if (chainTooltip.Length > 0)
            stringBuilder.Append("\n\n" + chainTooltip);
            
        if (item.m_shared.m_eitrRegenModifier > 0.0 && localPlayer != null)
        {
            stringBuilder.AppendFormat(
                "\n$item_eitrregen_modifier: <color=orange>{0:+0;-0}%</color> ($item_total:<color=yellow>{1:+0;-0}%</color>)",
                item.m_shared.m_eitrRegenModifier * 100f, localPlayer.GetEquipmentEitrRegenModifier() * 100f);
        }
        if (localPlayer != null)
            localPlayer.AppendEquipmentModifierTooltips(item, stringBuilder);
        string statusEffectTooltip2 = item.GetSetStatusEffectTooltip(qualityLevel, skillLevel);
        if (statusEffectTooltip2.Length > 0)
            stringBuilder.AppendFormat(
                "\n\n$item_seteffect (<color=orange>{0}</color> $item_parts):<color=orange>{1}</color>\n{2}",
                item.m_shared.m_setSize, item.m_shared.m_setStatusEffect.m_name, statusEffectTooltip2);
    }
}