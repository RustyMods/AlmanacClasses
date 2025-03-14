using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.UI;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class TextsDialoguePatches
{
    [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.UpdateTextsList))]
    static class CompendiumAddActiveEffectsPatch
    {
        private static void Postfix(TextsDialog __instance)
        {
            if (!Player.m_localPlayer) return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var talent in SpellBook.m_slots.Values)
            {
                if (talent.m_talent is null) continue;
                stringBuilder.Append($"<color=orange>{talent.m_talent.GetName()}</color>\n");
                stringBuilder.Append(talent.m_talent.GetTooltip());
                stringBuilder.Append("\n");
            }
            TextsDialog.TextInfo text = new TextsDialog.TextInfo("$title_spell_book", Localization.instance.Localize(stringBuilder.ToString()));
            TextsDialog.TextInfo passive = new TextsDialog.TextInfo("$title_passive_abilities", GetPassives());
            __instance.m_texts.Insert(0, passive);
            __instance.m_texts.Insert(0, text);
        }

        private static string GetPassives()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(CharacteristicManager.GetTooltip() + "\n");
            foreach (var kvp in PlayerManager.m_playerTalents)
            {
                if (kvp.Value.m_type is not TalentType.Passive) continue;
                var data = kvp.Value;
                // if (!data.m_passiveActive) continue;
                stringBuilder.Append($"<color=orange>{data.GetName()}</color> [{(data.m_passiveActive ? "$text_on" : "$text_off")}]\n");
                stringBuilder.Append(data.GetTooltip());
                stringBuilder.Append("\n");
            }

            return Localization.instance.Localize(stringBuilder.ToString());
        }
    }
}