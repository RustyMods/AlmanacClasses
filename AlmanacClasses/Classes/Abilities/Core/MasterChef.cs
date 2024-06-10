using HarmonyLib;

namespace AlmanacClasses.Classes.Abilities.Core;

public static class MasterChef
{
    [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
    private static class Player_GetTotalFoodValue_Postfix
    {
        private static void Postfix(Player __instance, ref float hp, ref float stamina, ref float eitr)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("MasterChef", out Talent ability)) return;
            float modifier = ability.GetFoodModifier(ability.GetLevel());
            hp = __instance.m_baseHP;
            stamina = __instance.m_baseStamina;
            eitr = 0.0f;
            foreach (Player.Food? food in __instance.m_foods)
            {
                hp += food.m_health *= modifier;
                stamina += food.m_stamina *= modifier;
                eitr += food.m_eitr *= modifier;
            }
        }
    }
}