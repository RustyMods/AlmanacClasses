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
        // CharacteristicManager.UpdateCharacteristics();
        CharacteristicManager.Update();
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_prestiged " + SelectedTalent.GetName() + " $almanac_to $text_lvl " + SelectedTalent.GetLevel());
        SkillTree.m_instance.Show();
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
            SkillTree.m_instance.SetSelectedName($"{SelectedTalent.GetName()} $text_lvl {SelectedTalent.GetLevel()}");
            SkillTree.m_instance.SetSelectedDescription(SelectedTalent.GetTooltip());
            SkillTree.m_instance.SetSelectedCost($"$almanac_cost: <color=orange>{SelectedTalent.GetCost()}</color>");
            SkillTree.m_instance.SetSelectedType(SelectedTalent.GetTalentType());
        }
        else
        {
            SkillTree.m_instance.SetSelectedName(
                $"{SelectedTalent.GetName()} $text_lvl {SelectedTalent.GetLevel()} --> <color=$FF5733>{SelectedTalent.GetLevel() + 1}</color>");
            SkillTree.m_instance.SetSelectedDescription(SelectedTalent.GetPrestigeTooltip());
            SkillTree.m_instance.SetSelectedCost($"$almanac_cost: <color=orange>{SelectedTalent.GetCost()}</color>");
            SkillTree.m_instance.SetSelectedType(SelectedTalent.GetTalentType());
        }
    }
    
    public static void DeselectTalent()
    {
        SelectedTalent = null;
        TalentButton.SetAllButtonColors(Color.white);
    }
}