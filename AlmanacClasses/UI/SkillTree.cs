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
    public Image PanelBackground = null!;
    public Image HeaderBackground = null!;
    public Image StatsBackground = null!;
    public Image TooltipBackground = null!;
    [Header("Text Elements")]
    public Text PointsUsed = null!;
    public Text RequiredPoints = null!;
    public Text PrestigeText = null!;
    public Text LevelText = null!;
    public Text ExperienceTitleText = null!;
    public Text ExperienceText = null!;
    public Text TalentPointsText = null!;
    public Text TalentName = null!;
    public Text TalentDescription = null!;
    public Text TalentCost = null!;
    public Text ActivePassive = null!;
    public Text CharacteristicsTitleText = null!;
    public Text ConstitutionText = null!;
    public Text DexterityText = null!;
    public Text IntelligenceText = null!;
    public Text StrengthText = null!;
    public Text WisdomText = null!;
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
        PanelBackground = GetComponent<Image>();
        m_texts = GetComponentsInChildren<Text>();
        HeaderBackground = transform.Find("Panel/$part_header/$part_title/Background").GetComponent<Image>();
        StatsBackground = transform.Find("Panel/$part_header/$part_stats").GetComponent<Image>();
        TooltipBackground = transform.Find("Panel/$part_header/$part_tooltip").GetComponent<Image>();
        ExperienceBarFill = Utils.FindChild(transform, "$image_experience_fill").GetComponent<Image>();
        PrestigeText = Utils.FindChild(transform, "$text_prestige").GetComponent<Text>();
        LevelText = Utils.FindChild(transform, "$text_level").GetComponent<Text>();
        ExperienceTitleText = Utils.FindChild(transform, "$text_experience_title").GetComponent<Text>();
        ExperienceText = Utils.FindChild(transform, "$text_experience").GetComponent<Text>();
        TalentPointsText = Utils.FindChild(transform, "$text_talent_points").GetComponent<Text>();
        TalentName = Utils.FindChild(transform, "$text_name").GetComponent<Text>();
        TalentDescription = Utils.FindChild(transform, "$text_description").GetComponent<Text>();
        TalentCost = Utils.FindChild(transform, "$text_cost").GetComponent<Text>();
        ActivePassive = Utils.FindChild(transform, "$text_active_passive").GetComponent<Text>();

        CharacteristicsTitleText = Utils.FindChild(transform, "$text_stats_title").GetComponent<Text>();
        CharacteristicsTitleText.resizeTextForBestFit = true;
        ConstitutionText = Utils.FindChild(transform, "$text_constitution").GetComponent<Text>();
        DexterityText = Utils.FindChild(transform, "$text_dexterity").GetComponent<Text>();
        IntelligenceText = Utils.FindChild(transform, "$text_intelligence").GetComponent<Text>();
        StrengthText = Utils.FindChild(transform, "$text_strength").GetComponent<Text>();
        WisdomText = Utils.FindChild(transform, "$text_wisdom").GetComponent<Text>();
        SpellBarHotKeyTooltip = Utils.FindChild(transform, "$part_hotkey_tooltip").GetComponent<Text>();
        SpellBarHotKeyTooltip.text = Localization.instance.Localize($"$info_spellbook_key: <color=orange>{AlmanacClassesPlugin._SpellAlt.Value}</color>");

        PointsUsed = Utils.FindChild(transform, "$text_used_points").GetComponent<Text>();
        PointsUsed.resizeTextForBestFit = true;
        RequiredPoints = Utils.FindChild(transform, "$text_prestige_needed").GetComponent<Text>();
        RequiredPoints.resizeTextForBestFit = true;

        ClassBardText = Utils.FindChild(transform, "$text_bard").GetComponent<Text>();
        ClassShamanText = Utils.FindChild(transform, "$text_shaman").GetComponent<Text>();
        ClassSageText = Utils.FindChild(transform, "$text_sage").GetComponent<Text>();
        ClassWarriorText = Utils.FindChild(transform, "$text_warrior").GetComponent<Text>();
        ClassRogueText = Utils.FindChild(transform, "$text_rogue").GetComponent<Text>();
        ClassRangerText = Utils.FindChild(transform, "$text_ranger").GetComponent<Text>();
        
        Utils.FindChild(transform, "$text_title").GetComponent<Text>().text = "$title_talents";
        Utils.FindChild(transform, "$text_constitution_title").GetComponent<Text>().text = Localization.instance.Localize("$almanac_constitution");
        Utils.FindChild(transform, "$text_dexterity_title").GetComponent<Text>().text = Localization.instance.Localize("$almanac_dexterity");
        Utils.FindChild(transform, "$text_intelligence_title").GetComponent<Text>().text = Localization.instance.Localize("$almanac_intelligence");
        Utils.FindChild(transform, "$text_strength_title").GetComponent<Text>().text = Localization.instance.Localize("$almanac_strength");
        Utils.FindChild(transform, "$text_wisdom_title").GetComponent<Text>().text = Localization.instance.Localize("$almanac_wisdom");
        LoadCloseButton();
        LoadResetButton();
        Utils.FindChild(transform, "$button_center").GetComponent<Button>().onClick.AddListener(Prestige.OnClickPrestige);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        FillLines.SetInitialFillLines();
        UpdateTexts();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Prestige.DeselectTalent();
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
        
        int constitution = CharacteristicManager.GetCharacteristic(Characteristic.Constitution);
        int dexterity = CharacteristicManager.GetCharacteristic(Characteristic.Dexterity);
        int intelligence = CharacteristicManager.GetCharacteristic(Characteristic.Intelligence);
        int strength = CharacteristicManager.GetCharacteristic(Characteristic.Strength);
        int wisdom = CharacteristicManager.GetCharacteristic(Characteristic.Wisdom);

        PrestigeText.text = "";
        ExperienceTitleText.text = Localization.instance.Localize("$text_experience");
        LevelText.text = $"{Localization.instance.Localize("$text_level")}: <color=orange>{level}</color>";
        ExperienceText.text = $"<color=orange>{experience}</color>" + " / " + $"<color=orange>{nextXP}</color>";
        
        ExperienceBarFill.fillAmount = (float)experience / (float)nextXP;
        TalentPointsText.text = Localization.instance.Localize("$text_talent_points") + ": " + $"<color=orange>{availableTalents}</color>";
        CharacteristicsTitleText.text = Localization.instance.Localize("$almanac_characteristic");
        
        ConstitutionText.text = $"<color=orange>{constitution}</color>";
        DexterityText.text = $"<color=orange>{dexterity}</color>";
        IntelligenceText.text = $"<color=orange>{intelligence}</color>";
        StrengthText.text = $"<color=orange>{strength}</color>";
        WisdomText.text = $"<color=orange>{wisdom}</color>";
        
        ClassBardText.text = Localization.instance.Localize("$class_bard");
        ClassShamanText.text = Localization.instance.Localize("$class_shaman");
        ClassSageText.text = Localization.instance.Localize("$class_sage");
        ClassWarriorText.text = Localization.instance.Localize("$class_warrior");
        ClassRogueText.text = Localization.instance.Localize("$class_rogue");
        ClassRangerText.text = Localization.instance.Localize("$class_ranger");

        ResetButtonText.text = Localization.instance.Localize("$info_reset_talents");
        
        int usedPoints = TalentManager.GetTotalBoughtTalentPoints();
        
        PointsUsed.text = Localization.instance.Localize($"$text_talent_points_used: <color=orange>{usedPoints}</color>");
        
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
    public void SetSelectedName(string text) => TalentName.text = Localization.instance.Localize(text);
    public void SetSelectedDescription(string text) => TalentDescription.text = Localization.instance.Localize(text);
    public void SetSelectedCost(string text) => TalentCost.text = Localization.instance.Localize(text);
    public void SetSelectedType(string text) => ActivePassive.text = Localization.instance.Localize(text);
    public void SetHotkeyTooltip(string text) => SpellBarHotKeyTooltip.text = Localization.instance.Localize(text);
    public bool IsVisible() => gameObject.activeInHierarchy;

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
        if (Player.m_localPlayer.GetInventory().CountItems("$item_coins") < AlmanacClassesPlugin._ResetCost.Value)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"$text_cost {AlmanacClassesPlugin._ResetCost.Value} $item_coins $text_to_reset");
            return;
        }

        if (AbilityManager.OnCooldown())
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_on_cooldown");
            return;
        }
        Player.m_localPlayer.GetInventory().RemoveItem("$item_coins", AlmanacClassesPlugin._ResetCost.Value);
        TalentManager.ResetTalents();
        ExperienceBar.UpdateExperienceBar();
    }
}