using AlmanacClasses.Classes;
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
            if (!LoadUI.IsTalentButton(__instance)) return;
            if (!TalentManager.m_talentsByButton.TryGetValue(__instance.name, out Talent talent))
            {
                if (__instance.name == "$button_center")
                {
                    if (LoadUI.SelectedTalent != null)
                    {
                        if (LoadUI.SelectedTalent.GetLevel() >= LoadUI.SelectedTalent.GetPrestigeCap())
                        {
                            LoadUI.TalentName.text = Localization.instance.Localize(string.Format("{0} $text_lvl {1}",
                                LoadUI.SelectedTalent.GetName(), LoadUI.SelectedTalent.GetLevel()));
                            LoadUI.TalentDescription.text = LoadUI.SelectedTalent.GetTooltip();
                            LoadUI.TalentCost.text = Localization.instance.Localize($"$almanac_cost: <color=orange>{LoadUI.SelectedTalent.GetCost()}</color>");
                            LoadUI.ActivePassive.text = Localization.instance.Localize(LoadUI.SelectedTalent.GetTalentType());
                        }
                        else
                        {
                            LoadUI.TalentName.text = Localization.instance.Localize(string.Format("{0} $text_lvl {1} --> <color=$FF5733>{2}</color>",
                                LoadUI.SelectedTalent.GetName(), LoadUI.SelectedTalent.GetLevel(),
                                LoadUI.SelectedTalent.GetLevel() + 1));
                            // LoadUI.TalentName.text = Localization.instance.Localize(LoadUI.SelectedTalent.GetName() + " $text_lvl " + LoadUI.SelectedTalent.GetLevel() + $" --> <color=#FF5733>{LoadUI.SelectedTalent.GetLevel() + 1}</color>");
                            LoadUI.TalentDescription.text = LoadUI.SelectedTalent.GetPrestigeTooltip();
                            LoadUI.TalentCost.text = Localization.instance.Localize($"$almanac_cost: <color=orange>{LoadUI.SelectedTalent.GetCost()}</color>");
                            LoadUI.ActivePassive.text = Localization.instance.Localize(LoadUI.SelectedTalent.GetTalentType());
                        }
                        return;
                    }
                }
                LoadUI.TalentName.text = Localization.instance.Localize(__instance.name);
                LoadUI.TalentDescription.text = Localization.instance.Localize(__instance.name + "_desc");
                LoadUI.TalentCost.text = "";
                LoadUI.ActivePassive.text = "";
            }
            else
            {
                if (TalentManager.m_altTalentsByButton.TryGetValue(__instance.name, out Talent alt))
                {
                    if (alt.m_alt?.Value is AlmanacClassesPlugin.Toggle.On)
                    {
                        talent = alt;
                    }
                }
                LoadUI.TalentName.text = Localization.instance.Localize(talent.GetName() + " $text_lvl " + talent.m_level);
                LoadUI.TalentDescription.text = Localization.instance.Localize(talent.GetTooltip());
                LoadUI.TalentCost.text = Localization.instance.Localize($"$almanac_cost: <color=orange>{talent.GetCost()}</color>");
                LoadUI.ActivePassive.text = Localization.instance.Localize(talent.GetTalentType());
            }
        }
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnPointerExit))]
    private static class Button_OnPointerExist_Patch
    {
        private static void Postfix(Selectable __instance)
        {
            if (!LoadUI.IsTalentButton(__instance)) return;
            LoadUI.TalentName.text = Localization.instance.Localize("$info_hover");
            string desc = 
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
        private static bool Prefix(Selectable __instance) => !LoadUI.IsTalentButton(__instance);
    }
}