using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;
using UnityEngine.UI;

namespace AlmanacClasses.Patches;

public static class ButtonPatches
{
    /// <summary>
    /// Patches that allow to change the spell info texts when user cursor enters/exits buttons
    /// </summary>
    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnPointerEnter))]
    private static class Button_OnPointerEnter_Patch
    {
        private static void Postfix(Selectable __instance)
        {
            if (!__instance) return;
            if (!TalentButton.IsTalentButton(__instance)) return;
            if (__instance.name == "$button_center")
            {
                if (Prestige.SelectedTalent != null)
                {
                    Prestige.UpdateSelectedPrestigeTooltip();
                }
            }
            else
            {
                if (TalentManager.m_talentsByButton.TryGetValue(__instance.name, out Talent talent))
                {
                    if (TalentManager.m_altTalentsByButton.TryGetValue(__instance.name, out Talent alt))
                    {
                        if (alt.m_alt?.Value is AlmanacClassesPlugin.Toggle.On)
                        {
                            talent = alt;
                        }
                    }
                    UpdateInfo(talent);
                }
                else
                {
                    SkillTree.m_instance.SetDefaultTooltip();
                }
            }
        }
    }

    private static void UpdateInfo(Talent talent)
    {
        SkillTree.m_instance.SetSelectedName(talent.GetName() + " $text_lvl " + talent.m_level);
        SkillTree.m_instance.SetSelectedDescription(talent.GetTooltip());
        SkillTree.m_instance.SetSelectedCost($"$almanac_cost: <color=orange>{talent.GetCost()}</color>");
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnPointerExit))]
    private static class Button_OnPointerExist_Patch
    {
        private static void Postfix(Selectable __instance)
        {
            if (!TalentButton.IsTalentButton(__instance)) return;
            SkillTree.m_instance.SetDefaultTooltip();
        }
    }

    [HarmonyPatch(typeof(Selectable), nameof(Selectable.OnSelect))]
    private static class Button_OnSelect_Patch
    {
        private static bool Prefix(Selectable __instance) => !TalentButton.IsTalentButton(__instance);
        
    }
}