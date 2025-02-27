﻿using AlmanacClasses.Classes;
using UnityEngine;

namespace AlmanacClasses.UI;

/// <summary>
/// Component attached to buildable altar prefab
/// Allows user to interact with the prefab and access the UI
/// </summary>
public class TalentBook : MonoBehaviour, Interactable, Hoverable
{
    public string m_name = "$title_altar";
    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (hold || alt) return false;
        ShowUI();
        return true;
    }
    
    public static void ShowUI()
    {
        Player.m_localPlayer.m_zanim.SetInt("crafting", 1);
        LoadUI.SetInitialFillLines();
        LoadUI.SkillTree_UI.SetActive(true);
        SetTextElements();
    }

    private static void SetTextElements()
    {
        int experience = PlayerManager.m_tempPlayerData.m_experience;
        int level = PlayerManager.GetPlayerLevel(experience);
        int nextXP = PlayerManager.GetRequiredExperience(level + 1);
        int availableTalents = TalentManager.GetAvailableTalentPoints();

        if (availableTalents < 0)
        {
            LoadUI.ResetTalents();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_cfg_changed");
        }
        
        int constitution = CharacteristicManager.GetCharacteristic(Characteristic.Constitution);
        int dexterity = CharacteristicManager.GetCharacteristic(Characteristic.Dexterity);
        int intelligence = CharacteristicManager.GetCharacteristic(Characteristic.Intelligence);
        int strength = CharacteristicManager.GetCharacteristic(Characteristic.Strength);
        int wisdom = CharacteristicManager.GetCharacteristic(Characteristic.Wisdom);

        LoadUI.PrestigeText.text = "";
        LoadUI.ExperienceTitleText.text = Localization.instance.Localize("$text_experience");
        LoadUI.LevelText.text = $"{Localization.instance.Localize("$text_level")}: <color=orange>{level}</color>";
        LoadUI.ExperienceText.text = $"<color=orange>{experience}</color>" + " / " + $"<color=orange>{nextXP}</color>";
        LoadUI.ExperienceBarFill.fillAmount = (float)experience / (float)nextXP;
        LoadUI.TalentPointsText.text = Localization.instance.Localize("$text_talent_points") + ": " + $"<color=orange>{availableTalents}</color>";
        LoadUI.CharacteristicsTitleText.text = Localization.instance.Localize("$almanac_characteristic");
        
        LoadUI.ConstitutionText.text = $"<color=orange>{constitution}</color>";
        LoadUI.DexterityText.text = $"<color=orange>{dexterity}</color>";
        LoadUI.IntelligenceText.text = $"<color=orange>{intelligence}</color>";
        LoadUI.StrengthText.text = $"<color=orange>{strength}</color>";
        LoadUI.WisdomText.text = $"<color=orange>{wisdom}</color>";
        
        LoadUI.ClassBardText.text = Localization.instance.Localize("$class_bard");
        LoadUI.ClassShamanText.text = Localization.instance.Localize("$class_shaman");
        LoadUI.ClassSageText.text = Localization.instance.Localize("$class_sage");
        LoadUI.ClassWarriorText.text = Localization.instance.Localize("$class_warrior");
        LoadUI.ClassRogueText.text = Localization.instance.Localize("$class_rogue");
        LoadUI.ClassRangerText.text = Localization.instance.Localize("$class_ranger");

        LoadUI.ResetButtonText.text = Localization.instance.Localize("$info_reset_talents");
        
        int usedPoints = TalentManager.GetTotalBoughtTalentPoints();
        
        LoadUI.PointsUsed.text = Localization.instance.Localize($"$text_talent_points_used: <color=orange>{usedPoints}</color>");
        
        if (usedPoints < AlmanacClassesPlugin._PrestigeThreshold.Value)
        {
            int requiredPoints = AlmanacClassesPlugin._PrestigeThreshold.Value - usedPoints;
            LoadUI.RequiredPoints.text = Localization.instance.Localize($"$text_required_points_to_prestige: <color=orange>{requiredPoints}</color>");
        }
        else
        {
            LoadUI.RequiredPoints.text = Localization.instance.Localize("$text_required_points_to_prestige: <color=orange>0</color>");
        }
    }
    public static void HideUI()
    {
        if (!LoadUI.SkillTree_UI) return;
        LoadUI.SkillTree_UI.SetActive(false);
        Player.m_localPlayer.m_zanim.SetInt("crafting", 0);
        Prestige.DeselectTalent();
    }
    public static void UpdateTalentBookUI()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || !IsTalentBookVisible()) return;
        HideUI();
        Menu.instance.OnClose();
    }

    public static bool IsTalentBookVisible() => LoadUI.SkillTree_UI && LoadUI.SkillTree_UI.activeInHierarchy;
    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;
    public string GetHoverText() => Localization.instance.Localize(m_name) + "\n" + Localization.instance.Localize("[<color=yellow><b>$KEY_Use</b></color>] $info_open_book");
    public string GetHoverName() => Localization.instance.Localize(m_name);
}