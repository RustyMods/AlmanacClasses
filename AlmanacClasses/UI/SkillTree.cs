using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Managers;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class SkillTree : MonoBehaviour
{
    public static SkillTree m_instance = null!;
    [Header("Image Elements")]
    public Image ExperienceBarFill = null!;
    [Header("Backgrounds")]
    public Image PanelBackground = null!;
    public Image HeaderBackground = null!;
    public Image StatsBackground = null!;
    public Image TooltipBackground = null!;
    public Image CharacteristicBackground = null!;
    public Image StatsTooltipBackground = null!;
    public Image ExtraInfoBackground = null!;
    public Image BonusBackground = null!;
    public Image InventoryBackground = null!;
    [Header("Text Elements")]
    public Text PointsUsed = null!;
    public Text RequiredPoints = null!;
    public Text LevelText = null!;
    public Text LevelAmount = null!;
    public Text ExperienceText = null!;
    public Text TalentPointsText = null!;
    public Text TalentsPointsAmount = null!;
    public Text CharacteristicText = null!;
    public Text CharacteristicAmount = null!;
    public Text StatsTooltip = null!;
    public Text BonusTooltip = null!;
    
    public Text TalentName = null!;
    public Text TalentDescription = null!;
    public Text TalentCost = null!;
    public Text ActivePassive = null!;

    public Text SpellBarHotKeyTooltip = null!;
    public Text ResetButtonText = null!;
    public Text ClassBardText = null!;
    public Text ClassShamanText = null!;
    public Text ClassSageText = null!;
    public Text ClassWarriorText = null!;
    public Text ClassRogueText = null!;
    public Text ClassRangerText = null!;

    public Text[] m_texts = null!;

    public static void UpdateUI()
    {
        if (!m_instance) return;
        if (AlmanacClassesPlugin._ShowUIEnabled.Value is AlmanacClassesPlugin.Toggle.On)
        {
            if (Input.GetKeyDown(AlmanacClassesPlugin._ShowUIKey.Value)) m_instance.Show();
        }
        if (!Input.GetKeyDown(KeyCode.Escape) || !m_instance.IsVisible()) return;
        m_instance.Hide();
    }

    public void Init()
    {
        m_instance = this;
        m_texts = GetComponentsInChildren<Text>();
        PanelBackground = GetComponent<Image>();
        HeaderBackground = transform.Find("Panel/$part_header/$part_title/Background").GetComponent<Image>();
        StatsBackground = transform.Find("Panel/$part_header/$part_stats").GetComponent<Image>();
        CharacteristicBackground = transform.Find("Panel/$part_header/$part_characteristics").GetComponent<Image>();
        StatsTooltipBackground = transform.Find("Panel/$part_header/$part_stats_tooltip").GetComponent<Image>();
        TooltipBackground = transform.Find("Panel/$part_header/$part_tooltip").GetComponent<Image>();
        ExtraInfoBackground = transform.Find("Panel/$part_header/$part_prestige").GetComponent<Image>();
        BonusBackground = transform.Find("Panel/$part_header/$part_stats_bonus").GetComponent<Image>();
        InventoryBackground = transform.Find("Panel/$part_header/$part_inventory").GetComponent<Image>();
        ExperienceBarFill = transform.Find("Panel/$part_header/$part_stats/$part_experience_bar/$image_experience_fill").GetComponent<Image>();
        ExperienceText = transform.Find("Panel/$part_header/$part_stats/$part_experience_bar/$text_experience").GetComponent<Text>();
        LevelText = transform.Find("Panel/$part_header/$part_stats/$part_level/$text_title").GetComponent<Text>();
        LevelAmount = transform.Find("Panel/$part_header/$part_stats/$part_level/$text_count").GetComponent<Text>();
        TalentPointsText = transform.Find("Panel/$part_header/$part_stats/$part_talent_points/$text_title").GetComponent<Text>();
        TalentsPointsAmount = transform.Find("Panel/$part_header/$part_stats/$part_talent_points/$text_count").GetComponent<Text>();
        CharacteristicText = transform.Find("Panel/$part_header/$part_stats/$part_characteristic_points/$text_title").GetComponent<Text>();
        CharacteristicText.resizeTextForBestFit = true;
        CharacteristicAmount = transform.Find("Panel/$part_header/$part_stats/$part_characteristic_points/$text_count").GetComponent<Text>();
        CharacteristicAmount.horizontalOverflow = HorizontalWrapMode.Overflow;
        StatsTooltip = transform.Find("Panel/$part_header/$part_stats_tooltip/$text").GetComponent<Text>();
        BonusTooltip = transform.Find("Panel/$part_header/$part_stats_bonus/$text").GetComponent<Text>();
        TalentName = Utils.FindChild(transform, "$text_name").GetComponent<Text>();
        TalentDescription = Utils.FindChild(transform, "$text_description").GetComponent<Text>();
        TalentCost = Utils.FindChild(transform, "$text_cost").GetComponent<Text>();
        ActivePassive = Utils.FindChild(transform, "$text_active_passive").GetComponent<Text>();
        SpellBarHotKeyTooltip = Utils.FindChild(transform, "$part_hotkey_tooltip").GetComponent<Text>();
        SpellBarHotKeyTooltip.text = Localization.instance.Localize($"$info_spellbook_key: <color=orange>{AlmanacClassesPlugin._SpellAlt.Value}</color>");
        PointsUsed = Utils.FindChild(transform, "$text_used_points").GetComponent<Text>();
        RequiredPoints = Utils.FindChild(transform, "$text_prestige_needed").GetComponent<Text>();
        ClassBardText = Utils.FindChild(transform, "$text_bard").GetComponent<Text>();
        ClassShamanText = Utils.FindChild(transform, "$text_shaman").GetComponent<Text>();
        ClassSageText = Utils.FindChild(transform, "$text_sage").GetComponent<Text>();
        ClassWarriorText = Utils.FindChild(transform, "$text_warrior").GetComponent<Text>();
        ClassRogueText = Utils.FindChild(transform, "$text_rogue").GetComponent<Text>();
        ClassRangerText = Utils.FindChild(transform, "$text_ranger").GetComponent<Text>();
        transform.Find("Panel/$part_header/$part_title").gameObject.SetActive(false);
        LoadCloseButton();
        LoadResetButton();
        UpdateBackground();
        Utils.FindChild(transform, "$button_center").GetComponent<Button>().onClick.AddListener(Prestige.OnClickPrestige);
        transform.Find("Panel/$part_header/$part_characteristics").gameObject.AddComponent<CharacteristicPanel>().Init();
        transform.Find("Panel/$part_header/$part_inventory").gameObject.AddComponent<SpellInventory>().Init();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!m_instance) return;
        gameObject.SetActive(true);
        FillLines.SetInitialFillLines();
        PassiveBar.m_instance.Show();
        UpdateTexts();
    }

    public void Hide()
    {
        if (!m_instance) return;
        gameObject.SetActive(false);
        Prestige.DeselectTalent();
        PassiveBar.m_instance.Hide();
        if (Player.m_localPlayer) Player.m_localPlayer.m_zanim.SetInt("crafting", 0);
    }
    public void UpdateTexts()
    {
        int experience = PlayerManager.m_tempPlayerData.m_experience;
        int level = PlayerManager.GetPlayerLevel(experience);
        int nextXP = PlayerManager.GetRequiredExperience(level + 1);
        int availableTalents = TalentManager.GetAvailableTalentPoints();

        if (availableTalents < 0)
        {
            TalentManager.ResetTalents();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_cfg_changed");
        }
        SetLevel(level);
        SetExperience(experience, nextXP);
        SetTitles();
        SetTalentPoint(availableTalents);
        int usedPoints = TalentManager.GetTotalBoughtTalentPoints();
        SetUsedTalentPoints(usedPoints);
        SetRequiredTalentPointsThreshold(usedPoints);
        CharacteristicPanel.m_instance.UpdateTexts();
        CharacteristicManager.UpdateCharacteristicPoints();
        CharacteristicButtons.UpdateAllButtons();
        UpdateStatsBonus();
    }

    public void SetRequiredTalentPointsThreshold(int usedPoints)
    {
        if (usedPoints < AlmanacClassesPlugin._PrestigeThreshold.Value)
        {
            int requiredPoints = AlmanacClassesPlugin._PrestigeThreshold.Value - usedPoints;
            RequiredPoints.text = Localization.instance.Localize($"$text_required_points_to_prestige: <color=orange>{requiredPoints}</color>");
        }
        else
        {
            RequiredPoints.text = Localization.instance.Localize("$text_required_points_to_prestige: <color=orange>0</color>");
        }
    }
    
    public void SetTalentPoint(int amount) => TalentsPointsAmount.text = $"<color=orange>{amount}</color>";
    public void SetUsedTalentPoints(int amount) => PointsUsed.text = Localization.instance.Localize($"$text_talent_points_used: <color=orange>{amount}</color>");

    public void SetTitles()
    {
        CharacteristicText.text = Localization.instance.Localize("$almanac_characteristic $text_points");
        TalentPointsText.text = Localization.instance.Localize("$text_talent_points");
        ClassBardText.text = Localization.instance.Localize("$class_bard");
        ClassShamanText.text = Localization.instance.Localize("$class_shaman");
        ClassSageText.text = Localization.instance.Localize("$class_sage");
        ClassWarriorText.text = Localization.instance.Localize("$class_warrior");
        ClassRogueText.text = Localization.instance.Localize("$class_rogue");
        ClassRangerText.text = Localization.instance.Localize("$class_ranger");
        ResetButtonText.text = Localization.instance.Localize("$info_reset_talents");
    }

    public void SetLevel(int level)
    {
        LevelText.text = $"{Localization.instance.Localize("$text_level")}";
        LevelAmount.text = $"<color=orange>{level}</color>";
    }

    public void SetExperience(int current, int next)
    {
        ExperienceText.text = Localization.instance.Localize($"<color=orange>{current}</color>" + " / " + $"<color=orange>{next}</color> $text_xp");
    }
    
    public void UpdateStatsBonus() => BonusTooltip.text = Localization.instance.Localize(CharacteristicManager.GetTooltip());
    public void SetSelectedName(string text) => TalentName.text = Localization.instance.Localize(text);
    public void SetSelectedDescription(string text) => TalentDescription.text = Localization.instance.Localize(text);
    public void SetSelectedCost(string text) => TalentCost.text = Localization.instance.Localize(text);
    public void SetSelectedType(string text) => ActivePassive.text = Localization.instance.Localize(text);
    public void SetHotkeyTooltip(string text) => SpellBarHotKeyTooltip.text = Localization.instance.Localize(text);
    public bool IsVisible() => gameObject.activeInHierarchy;
    public static bool IsPanelVisible() => m_instance && m_instance.IsVisible();
    public void SetDefaultTooltip()
    {
        SetSelectedName("$info_hover");
        string desc = 
            "[<color=orange>L.Alt</color>] + [<color=orange>Mouse0</color>] - $info_move_spellbar" 
            + "\n"
            + "[<color=orange>Mouse0</color>] - $info_swap_ability"
            + "\n"
            + "[<color=orange>Mouse0</color>] - $info_move_xp_bar";
        SetSelectedDescription(desc);
        SetSelectedCost("");
        SetSelectedType("");
    }
    public void UpdateBackground()
    {
        if (!m_instance) return;
        if (AlmanacClassesPlugin._CustomBackground.Value.IsNullOrWhiteSpace()) return;
        if (!SpriteManager.m_backgrounds.TryGetValue(AlmanacClassesPlugin._CustomBackground.Value, out Sprite background)) return;
        PanelBackground.sprite = background;
        HeaderBackground.sprite = background;
        StatsBackground.sprite = background;
        TooltipBackground.sprite = background;
        CharacteristicBackground.sprite = background;
        StatsTooltipBackground.sprite = background;
        ExtraInfoBackground.sprite = background;
        BonusBackground.sprite = background;
        InventoryBackground.sprite = background;
    }
    public void SetBackgroundColor(Color color) => PanelBackground.color = color;
    private void LoadCloseButton()
    {
        Transform closeButton = Utils.FindChild(transform, "$button_close");
        if (closeButton.TryGetComponent(out Button component))
        {
            component.transition = Selectable.Transition.SpriteSwap;
            component.spriteState = LoadUI.m_vanillaButton.spriteState;
            component.onClick.AddListener(m_instance.Hide);
        }

        closeButton.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
    }
    
    private void LoadResetButton()
    {
        Transform resetButton = Utils.FindChild(transform, "$button_reset");

        ResetButtonText = resetButton.GetChild(0).GetComponent<Text>();
        
        if (resetButton.TryGetComponent(out Button component))
        {
            component.transition = Selectable.Transition.SpriteSwap;
            component.spriteState = LoadUI.m_vanillaButton.spriteState;
            component.onClick.AddListener(OnReset);
        }

        resetButton.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
    }
    
    private static void OnReset()
    {
        if (!Player.m_localPlayer) return;
        if (!Player.m_localPlayer.NoCostCheat() && Player.m_localPlayer.GetInventory().CountItems("$item_coins") < AlmanacClassesPlugin._ResetCost.Value)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"$text_cost {AlmanacClassesPlugin._ResetCost.Value} $item_coins $text_to_reset");
            return;
        }

        if (AbilityManager.OnCooldown())
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_on_cooldown");
            return;
        }

        if (!Player.m_localPlayer.NoCostCheat())
        {
            Player.m_localPlayer.GetInventory().RemoveItem("$item_coins", AlmanacClassesPlugin._ResetCost.Value);
        }
        TalentManager.ResetTalents();
        ExperienceBar.UpdateExperienceBar();
    }
}