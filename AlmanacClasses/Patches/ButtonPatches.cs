using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.Data;
using AlmanacClasses.UI;
using HarmonyLib;
using UnityEngine.UI;

namespace AlmanacClasses.Patches;

public static class ButtonPatches
{
    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnPointerEnter))]
    private static class Button_OnPointerEnter_Patch
    {
        private static void Postfix(Selectable __instance)
        {
            if (!__instance) return;
            if (!LoadUI.TalentButtons.Contains(__instance)) return;
            Talent? talent = TalentManager.GetTalentByButton(__instance.name);
            if (talent == null)
            {
                LoadUI.TalentName.text = __instance.name;
                LoadUI.TalentDescription.text = __instance.name + "_desc";
                LoadUI.TalentCost.text = Localization.instance.Localize("$almanac_cost: [<color=orange>value</color>]");
                LoadUI.ActivePassive.text = "";
            }
            else
            {
                LoadUI.TalentName.text = Localization.instance.Localize(talent.m_name);
                LoadUI.TalentDescription.text = LoadUI.GetTooltip(talent);
                LoadUI.TalentCost.text = Localization.instance.Localize($"$almanac_cost: <color=orange>{talent.m_cost}</color>");
                switch (talent.m_type)
                {
                    case TalentType.Ability or TalentType.StatusEffect or TalentType.Finder:
                        LoadUI.ActivePassive.text = Localization.instance.Localize("$almanac_ability");
                        break;
                    case TalentType.Characteristic:
                        LoadUI.ActivePassive.text = Localization.instance.Localize("$almanac_characteristic");
                        break;
                    case TalentType.Passive or TalentType.Comfort:
                        LoadUI.ActivePassive.text = Localization.instance.Localize("$almanac_passive");
                        break;
                    default:
                        LoadUI.ActivePassive.text = "";
                        break;
                } 
            }
        }
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnPointerExit))]
    private static class Button_OnPointerExist_Patch
    {
        private static void Postfix(Selectable __instance)
        {
            if (!LoadUI.TalentButtons.Contains(__instance)) return;
            LoadUI.TalentName.text = Localization.instance.Localize("$info_hover");
            var desc = 
                "[<color=orange>L.Alt</color>] + [<color=orange>Mouse0</color>] - $info_move_spellbar" 
                + "\n"
                + "[<color=orange>Mouse0</color>] - $info_swap_ability"
                + "\n"
                + "[<color=orange>Mouse0</color>] - $info_move_xp_bar";
            LoadUI.TalentDescription.text = Localization.instance.Localize(desc);
            LoadUI.TalentCost.text = "";
            LoadUI.ActivePassive.text = "";
        }
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnSelect))]
    private static class Button_OnSelect_Patch
    {
        private static bool Prefix(Selectable __instance) => !LoadUI.TalentButtons.Contains(__instance);
    }
}