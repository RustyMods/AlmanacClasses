using AlmanacClasses.Classes;
using UnityEngine;

namespace AlmanacClasses.UI;

public static class Prestige
{
    public static Talent? SelectedTalent;

    public static void OnClickPrestige()
    {
        if (TalentManager.GetTotalBoughtTalentPoints() < AlmanacClassesPlugin._PrestigeThreshold.Value)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_not_enough_tp_to_prestige");
            return;
        }

        if (SelectedTalent == null)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_select_talent");
            return;
        }
        if (!PlayerManager.m_tempPlayerData.m_boughtTalents.ContainsKey(SelectedTalent.m_key)) return;
        int cost = SelectedTalent.GetCost();
        if (cost > TalentManager.GetAvailableTalentPoints())
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_not_enough_tp");
            return;
        }

        if (SelectedTalent.GetLevel() >= SelectedTalent.GetPrestigeCap())
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_prestige_cap");
            return;
        }

        if (SelectedTalent.m_type is TalentType.Characteristic &&
            SelectedTalent.GetLevel() >= AlmanacClassesPlugin._characteristicCap.Value)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_prestige_cap");
            return;
        }
            
        AddLevel(SelectedTalent);
        CharacteristicManager.UpdateCharacteristics();
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_prestiged " + SelectedTalent.GetName() + " $almanac_to $text_lvl " + SelectedTalent.GetLevel());
        TalentBook.ShowUI();
        UpdateSelectedPrestigeTooltip();
    }
    
    private static void AddLevel(Talent ability)
    {
        if (PlayerManager.m_tempPlayerData.m_boughtTalents.ContainsKey(ability.m_key))
        {
            ++PlayerManager.m_tempPlayerData.m_boughtTalents[ability.m_key];
        }

        if (PlayerManager.m_playerTalents.TryGetValue(ability.m_key, out Talent talent))
        {
            talent.AddLevel();
        }
    }
    
    public static void UpdateSelectedPrestigeTooltip()
    {
        if (SelectedTalent is null) return;
        if (SelectedTalent.GetLevel() >= SelectedTalent.GetPrestigeCap())
        {
            LoadUI.TalentName.text = Localization.instance.Localize(string.Format("{0} $text_lvl {1}",
                SelectedTalent.GetName(), SelectedTalent.GetLevel()));
            LoadUI.TalentDescription.text = SelectedTalent.GetTooltip();
            LoadUI.TalentCost.text = Localization.instance.Localize($"$almanac_cost: <color=orange>{SelectedTalent.GetCost()}</color>");
            LoadUI.ActivePassive.text = Localization.instance.Localize(SelectedTalent.GetTalentType());
        }
        else
        {
            LoadUI.TalentName.text = Localization.instance.Localize(string.Format("{0} $text_lvl {1} --> <color=$FF5733>{2}</color>",
                SelectedTalent.GetName(), SelectedTalent.GetLevel(),
                SelectedTalent.GetLevel() + 1));
            LoadUI.TalentDescription.text = SelectedTalent.GetPrestigeTooltip();
            LoadUI.TalentCost.text = Localization.instance.Localize($"$almanac_cost: <color=orange>{SelectedTalent.GetCost()}</color>");
            LoadUI.ActivePassive.text = Localization.instance.Localize(SelectedTalent.GetTalentType());
        }
    }
    
    public static void DeselectTalent()
    {
        SelectedTalent = null;
        TalentButton.SetAllButtonColors(Color.white);
    }
}