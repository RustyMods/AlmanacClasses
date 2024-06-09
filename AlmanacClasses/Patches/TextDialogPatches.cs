using System.Collections.Generic;
using System.Text;
using AlmanacClasses.Managers;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class TextDialogPatches
{
    [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.AddActiveEffects))]
    private static class TextsDialog_AddActiveEffects_Patch
    {
        private static bool Prefix(TextsDialog __instance)
        {
            if (!__instance) return false;
            if (!Player.m_localPlayer) return false;
            List<StatusEffect> effects = new();
            Player.m_localPlayer.GetSEMan().GetHUDStatusEffects(effects);
            StringBuilder stringBuilder = new();
            foreach(StatusEffect statusEffect in effects)
            {
                if (!StatusEffectManager.IsClassEffect(statusEffect.name)) continue;
                stringBuilder.Append("<color=orange>" + Localization.instance.Localize(statusEffect.m_name) + "</color>\n");
                stringBuilder.Append(Localization.instance.Localize(statusEffect.GetTooltipString()));
                stringBuilder.Append("\n\n");
            }

            Player.m_localPlayer.GetGuardianPowerHUD(out StatusEffect se, out float _);
            if (se)
            {
                stringBuilder.Append("<color=yellow>" + Localization.instance.Localize("$inventory_selectedgp") + "</color>\n");
                stringBuilder.Append("<color=orange>" + Localization.instance.Localize(se.m_name) + "</color>\n");
                stringBuilder.Append(Localization.instance.Localize(se.GetTooltipString()));
            }
            __instance.m_texts.Insert(0, new TextsDialog.TextInfo(Localization.instance.Localize("$inventory_activeeffects"), stringBuilder.ToString()));
            return false;
        }
    }
}