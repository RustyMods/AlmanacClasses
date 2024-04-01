using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Data;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AlmanacClasses.UI;

public static class LoadUI
{
    public static readonly List<Selectable> TalentButtons = new();
    private static ButtonSfx sfx = null!;
    private static Button buttonComponent = null!;
    
    public static GameObject SkillTree_UI = null!;
    private static GameObject ExperienceBarHUD = null!;
    private static RectTransform ExpBarRect = null!;
    private static Image ExpHudFillBar = null!;

    private static Text ExpHudText = null!;
    // public static Color OriginalExpBarColor;

    public static GameObject InfoHoverElement = null!;
    public static GameObject SpellBarHoverName = null!;

    public static Image PanelBackground = null!;

    public static Text PointsUsed = null!;
    public static Text RequiredPoints = null!;
    
    public static Text PrestigeText = null!;
    public static Text LevelText = null!;
    public static Text ExperienceText = null!;
    public static Text TalentPointsText = null!;
    public static Text TalentName = null!;
    public static Text TalentDescription = null!;
    public static Text TalentCost = null!;
    public static Text ActivePassive = null!;

    public static Text ConstitutionText = null!;
    public static Text DexterityText = null!;
    public static Text IntelligenceText = null!;
    public static Text StrengthText = null!;
    public static Text WisdomText = null!;
    public static Text SpellBarHotKeyTooltip = null!;
    public static Image ExperienceBarFill = null!;

    public static GameObject MenuInfoPanel = null!;

    private static Button CenterButton = null!;
    public static bool m_initLineFillSet;
    public static readonly Dictionary<string, Button> ButtonMap = new();
    private static readonly Dictionary<Button, Dictionary<string, Image>> ButtonFillLineMap = new();
    private static readonly Dictionary<Button, float> ButtonCoreLineAmountMap = new();
    private static readonly List<Button> CheckedTalents = new();
    #region All Fill Lines Images
    #region Line Up
    private static Image LineCoreUp = null!;
    private static Image LineUp1Right = null!;
    private static Image LineUp1Left = null!;
    private static Image LineUp2Right = null!;
    private static Image LineUp2Left = null!;
    private static Image LineUp3Right = null!;
    private static Image LineUp3RightUp = null!;
    private static Image LineUp4Left = null!;
    private static Image LineUp4LeftUp = null!;
    #endregion
    #region Line Bard
    private static Image LineCoreBard = null!;
    private static Image LineBard1Right = null!;
    private static Image LineBard1Left = null!;
    private static Image LineBard2Right = null!;
    private static Image LineBard2Left = null!;
    private static Image LineBard3Right = null!;
    private static Image LineBard3RightUp = null!;
    private static Image LineBard4Left = null!;
    private static Image LineBard4LeftUp = null!;
    #endregion
    #region Line Shaman
    private static Image LineCoreShaman = null!;
    private static Image LineShaman1Right = null!;
    private static Image LineShaman1Left = null!;
    private static Image LineShaman2Right = null!;
    private static Image LineShaman2Left = null!;
    private static Image LineShaman3Right = null!;
    private static Image LineShaman3RightUp = null!;
    private static Image LineShaman4Left = null!;
    private static Image LineShaman4LeftUp = null!;
    #endregion
    #region Line Sage
    private static Image LineCoreSage = null!;
    private static Image LineSage1Right = null!;
    private static Image LineSage1Left = null!;
    private static Image LineSage2Right = null!;
    private static Image LineSage2Left = null!;
    private static Image LineSage3Right = null!;
    private static Image LineSage3RightUp = null!;
    private static Image LineSage4Left = null!;
    private static Image LineSage4LeftUp = null!;
    #endregion
    #region Line Down
    private static Image LineCoreDown = null!;
    private static Image LineDown1Right = null!;
    private static Image LineDown1Left = null!;
    private static Image LineDown2Right = null!;
    private static Image LineDown2Left = null!;
    private static Image LineDown3Right = null!;
    private static Image LineDown3RightDown = null!;
    private static Image LineDown4Left = null!;
    private static Image LineDown4LeftDown = null!;
    #endregion
    #region Line Ranger
    private static Image LineCoreRanger = null!;
    private static Image LineRanger1Right = null!;
    private static Image LineRanger1Left = null!;
    private static Image LineRanger2Right = null!;
    private static Image LineRanger2Left = null!;
    private static Image LineRanger3Right = null!;
    private static Image LineRanger3RightUp = null!;
    private static Image LineRanger4Left = null!;
    private static Image LineRanger4LeftUp = null!;
    #endregion
    #region Line Rogue
    private static Image LineCoreRogue = null!;
    private static Image LineRogue1Right = null!;
    private static Image LineRogue1Left = null!;
    private static Image LineRogue2Right = null!;
    private static Image LineRogue2Left = null!;
    private static Image LineRogue3Right = null!;
    private static Image LineRogue3RightUp = null!;
    private static Image LineRogue4Left = null!;
    private static Image LineRogue4LeftUp = null!;
    #endregion
    #region Line Warrior
    private static Image LineCoreWarrior = null!;
    private static Image LineWarrior1Right = null!;
    private static Image LineWarrior1Left = null!;
    private static Image LineWarrior2Right = null!;
    private static Image LineWarrior2Left = null!;
    private static Image LineWarrior3Right = null!;
    private static Image LineWarrior3RightUp = null!;
    private static Image LineWarrior4Left = null!;
    private static Image LineWarrior4LeftUp = null!;
    #endregion
    #region Line Radial
    private static Image LineRadial1 = null!;
    private static Image LineRadial2 = null!;
    private static Image LineRadial3 = null!;
    private static Image LineRadial4 = null!;
    private static Image LineRadial5 = null!;
    private static Image LineRadial6 = null!;
    private static Image LineRadial7 = null!;
    private static Image LineRadial8 = null!;
    #endregion
    #endregion
    private static readonly List<Image> AllLines = new();

    private static readonly Dictionary<string, List<string>> EndTalents = new()
    {
        { "$button_chef", new() { "$button_core_1", "$button_core_2", "$button_lumberjack" } },
        { "$button_bard_talent_5", new(){"$button_bard_1", "$button_bard_2", "$button_bard_talent_2"} },
        { "$button_shaman_talent_5", new(){"$button_shaman_1", "$button_shaman_2", "$button_shaman_talent_2"}},
        { "$button_sage_talent_5", new(){"$button_sage_1", "$button_sage_2", "$button_sage_talent_4"}},
        { "$button_sail", new(){"$button_core_7", "$button_core_8", "$button_treasure"}},
        { "$button_ranger_talent_5", new(){"$button_ranger_1", "$button_ranger_2", "$button_ranger_talent_2"}},
        { "$button_rogue_talent_5", new(){"$button_rogue_1", "$button_rogue_2", "$button_rogue_talent_2"}},
        { "$button_warrior_talent_5", new(){"$button_warrior_1", "$button_warrior_2", "$button_warrior_talent_2"}},
    };

    private static bool IsEXPBarVisible() => ExperienceBarHUD && ExperienceBarHUD.activeInHierarchy;

    public static void InitHudExperienceBar(Hud instance)
    {
        ExperienceBarHUD = Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("Experience_Bar"), instance.transform, false);
        ExperienceBarHUD.AddComponent<ExperienceBarMove>();
        ExperienceBarHUD.SetActive(AlmanacClassesPlugin._HudVisible.Value is AlmanacClassesPlugin.Toggle.On);
        ExpBarRect = ExperienceBarHUD.GetComponent<RectTransform>();
        ExpBarRect.SetAsLastSibling();
        ExpBarRect.anchoredPosition = AlmanacClassesPlugin._ExperienceBarPos.Value;
        ExpHudFillBar = ExperienceBarHUD.transform.Find("FillBar").GetComponent<Image>();
        ExpHudText = ExperienceBarHUD.transform.Find("$text_experience").GetComponent<Text>();
        ExpHudFillBar.fillAmount = 0f;
        ExpHudText.text = "";
        // OriginalExpBarColor = ExpHudFillBar.color;

        InfoHoverElement = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("ElementHover_UI");
        
        Text[] hoverTexts = InfoHoverElement.GetComponentsInChildren<Text>();

        Text[] hudTexts = ExperienceBarHUD.GetComponentsInChildren<Text>();
        Font? NorseBold = GetFont("Norsebold");
        AddFonts(hudTexts, NorseBold);
        AddFonts(hoverTexts, NorseBold);
        SpellBook.LoadElements();
        
        MenuInfoPanel = Object.Instantiate(LoadUI.InfoHoverElement, Hud.instance.transform, false);
        MenuInfoPanel.transform.SetAsFirstSibling();
        MenuInfoPanel.transform.position = AlmanacClassesPlugin._SpellBookPos.Value + new Vector2(0f, 150f);
        MenuInfoPanel.SetActive(false);

        GameObject HoverName = new GameObject("title");
        HoverName.AddComponent<RectTransform>();
        Text text = HoverName.AddComponent<Text>();
        text.font = NorseBold;
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.supportRichText = true;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        SpellBarHoverName = HoverName;

    }

    public static void InitSkillTree(InventoryGui instance)
    {
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Initializing talent UI");
        Transform vanillaButton = Utils.FindChild(Utils.FindChild(instance.transform, "TrophiesFrame"), "Closebutton");
        sfx = vanillaButton.GetComponent<ButtonSfx>();
        buttonComponent = vanillaButton.GetComponent<Button>();

        SkillTree_UI = Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("Almanac_SkillTree"), instance.transform, false);
        SkillTree_UI.SetActive(false);

        PanelBackground = SkillTree_UI.GetComponent<Image>();

        SetTextFont();

        PrestigeText = Utils.FindChild(SkillTree_UI.transform, "$text_prestige").GetComponent<Text>();
        LevelText = Utils.FindChild(SkillTree_UI.transform, "$text_level").GetComponent<Text>();
        ExperienceText = Utils.FindChild(SkillTree_UI.transform, "$text_experience").GetComponent<Text>();
        TalentPointsText = Utils.FindChild(SkillTree_UI.transform, "$text_talent_points").GetComponent<Text>();
        TalentName = Utils.FindChild(SkillTree_UI.transform, "$text_name").GetComponent<Text>();
        TalentDescription = Utils.FindChild(SkillTree_UI.transform, "$text_description").GetComponent<Text>();
        TalentCost = Utils.FindChild(SkillTree_UI.transform, "$text_cost").GetComponent<Text>();
        ActivePassive = Utils.FindChild(SkillTree_UI.transform, "$text_active_passive").GetComponent<Text>();

        ConstitutionText = Utils.FindChild(SkillTree_UI.transform, "$text_constitution").GetComponent<Text>();
        DexterityText = Utils.FindChild(SkillTree_UI.transform, "$text_dexterity").GetComponent<Text>();
        IntelligenceText = Utils.FindChild(SkillTree_UI.transform, "$text_intelligence").GetComponent<Text>();
        StrengthText = Utils.FindChild(SkillTree_UI.transform, "$text_strength").GetComponent<Text>();
        WisdomText = Utils.FindChild(SkillTree_UI.transform, "$text_wisdom").GetComponent<Text>();
        SpellBarHotKeyTooltip = Utils.FindChild(SkillTree_UI.transform, "$part_hotkey_tooltip").GetComponent<Text>();
        SpellBarHotKeyTooltip.text = $"Alt Spell Book Key: <color=orange>{AlmanacClassesPlugin._SpellAlt.Value}</color>";
        ExperienceBarFill = Utils.FindChild(SkillTree_UI.transform, "$image_experience_fill").GetComponent<Image>();

        PointsUsed = Utils.FindChild(SkillTree_UI.transform, "$text_used_points").GetComponent<Text>();
        RequiredPoints = Utils.FindChild(SkillTree_UI.transform, "$text_prestige_needed").GetComponent<Text>();


        // Static Elements
        Utils.FindChild(SkillTree_UI.transform, "$text_title").GetComponent<Text>().text = "$title_talents";
        Utils.FindChild(SkillTree_UI.transform, "$text_constitution_title").GetComponent<Text>().text =
            Localization.instance.Localize("$almanac_constitution");
        Utils.FindChild(SkillTree_UI.transform, "$text_dexterity_title").GetComponent<Text>().text =
            Localization.instance.Localize("$almanac_dexterity");
        Utils.FindChild(SkillTree_UI.transform, "$text_intelligence_title").GetComponent<Text>().text =
            Localization.instance.Localize("$almanac_intelligence");
        Utils.FindChild(SkillTree_UI.transform, "$text_strength_title").GetComponent<Text>().text =
            Localization.instance.Localize("$almanac_strength");
        Utils.FindChild(SkillTree_UI.transform, "$text_wisdom_title").GetComponent<Text>().text =
            Localization.instance.Localize("$almanac_wisdom");

        LoadCloseButton();
        LoadResetButton();
        AddSFXToTalentButtons();
        
        SetPrestigeButton();
        SetLineFill();
        SetAllButtonEvents();
    }

    public static void OnChangeExperienceBarPosition(object sender, EventArgs e)
    {
        if (sender is not ConfigEntry<Vector2> config) return;

        ExpBarRect.anchoredPosition = config.Value;
    }

    public static void OnChangeExperienceBarVisibility(object sender, EventArgs e)
    {
        if (sender is not ConfigEntry<AlmanacClassesPlugin.Toggle> config) return;
        ExperienceBarHUD.SetActive(config.Value is AlmanacClassesPlugin.Toggle.On);
    }

    public static void UpdateExperienceBarHud()
    {
        if (!IsEXPBarVisible()) return;
        int experience = PlayerManager.m_tempPlayerData.m_experience;
        int level = PlayerManager.GetPlayerLevel(experience);
        int nxtLvlExp = PlayerManager.GetRequiredExperience(level + 1);
        float percentage = (float)experience / nxtLvlExp;
        ExpHudText.text = $"{experience} / {nxtLvlExp}";
        ExpHudFillBar.fillAmount = percentage;
    }

    private static void SetPrestigeButton()
    {
        Utils.FindChild(SkillTree_UI.transform, "$button_center").GetComponent<Button>().onClick.AddListener(() =>
        {
            int usedPoints = TalentManager.GetTotalBoughtTalentPoints();
            if (usedPoints < AlmanacClassesPlugin._PrestigeThreshold.Value)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_talent_required");
                return;
            }

            if (!TalentManager.AllTalents.TryGetValue("PrestigeButton", out Talent ability)) return;
            int cost = ability.m_cost;
            if (cost > TalentManager.GetAvailableTalentPoints())
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_not_enough_tp_to_prestige");
            }
            else
            {
                usedPoints += cost;
                PlayerManager.m_playerTalents[ability.m_key] = ability;
                PlayerManager.m_tempPlayerData.m_prestigePoints += usedPoints;
                ++PlayerManager.m_tempPlayerData.m_prestige;
                ResetTalents();
                TalentManager.PrestigeTalents(PlayerManager.m_tempPlayerData.m_prestige);
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_prestiged " + PlayerManager.m_tempPlayerData.m_prestige);
            }
        });
    }

    public static void SetInitialFillLines()
    {
        if (m_initLineFillSet) return;
        CheckedTalents.Clear();
        CheckedTalents.Add(CenterButton);
        foreach (KeyValuePair<string, Talent> kvp in PlayerManager.m_playerTalents)
        {
            string buttonName = kvp.Value.m_buttonName;
            if (!ButtonMap.TryGetValue(buttonName, out Button button)) continue;
            CheckedTalents.Add(button);
            
            if (!ButtonFillLineMap.TryGetValue(button, out Dictionary<string, Image> lines)) continue;
            if (!ButtonCoreLineAmountMap.TryGetValue(button, out float amount)) continue;
            Dictionary<string, Image> validatedLines = new();
            foreach (Button talent in CheckedTalents)
            {
                if (lines.TryGetValue(talent.name, out Image line))
                {
                    validatedLines[talent.name] = line;
                }
            }

            if (validatedLines.Count == 0) return;
            Transform? checkmark = Utils.FindChild(button.transform, "Checkmark");
            checkmark.gameObject.SetActive(true);
            foreach (KeyValuePair<string, Image> line in validatedLines)
            {
                line.Value.fillAmount = line.Value.transform.parent.name == "$line_core" ? amount : 1f;
            }
        }

        m_initLineFillSet = true;
    }
    #region Set Line Methods
    private static void SetLineFill()
    {
        AllLines.Clear();
        Transform lines = Utils.FindChild(SkillTree_UI.transform, "$part_lines");
        SetLineUp(lines);
        SetLineBard(lines);
        SetLineShaman(lines);
        SetLineSage(lines);
        SetLineDown(lines);
        SetLineRanger(lines);
        SetLineRogue(lines);
        SetLineWarrior(lines);
        SetLineRadial(lines);
    }

    private static void SetLineUp(Transform lines)
    {
        Transform lineUp = Utils.FindChild(lines, "$part_lines_up");
        LineCoreUp = lineUp.Find("$line_core/LineFill").GetComponent<Image>();
        LineUp1Right = lineUp.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineUp1Left = lineUp.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineUp2Right = lineUp.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineUp2Left = lineUp.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineUp3Right = lineUp.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineUp3RightUp = lineUp.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineUp4Left = lineUp.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineUp4LeftUp = lineUp.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreUp);
        AllLines.Add(LineUp1Right);
        AllLines.Add(LineUp1Left);
        AllLines.Add(LineUp2Right);
        AllLines.Add(LineUp2Left);
        AllLines.Add(LineUp3Right);
        AllLines.Add(LineUp3RightUp);
        AllLines.Add(LineUp4Left);
        AllLines.Add(LineUp4LeftUp);
    }

    private static void SetLineBard(Transform lines)
    {
        Transform lineBard = Utils.FindChild(lines, "$part_lines_up_right");
        LineCoreBard = lineBard.Find("$line_core/LineFill").GetComponent<Image>();
        LineBard1Right = lineBard.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineBard1Left = lineBard.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineBard2Right = lineBard.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineBard2Left = lineBard.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineBard3Right = lineBard.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineBard3RightUp = lineBard.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineBard4Left = lineBard.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineBard4LeftUp = lineBard.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreBard);
        AllLines.Add(LineBard1Right);
        AllLines.Add(LineBard1Left);
        AllLines.Add(LineBard2Right);
        AllLines.Add(LineBard2Left);
        AllLines.Add(LineBard3Right);
        AllLines.Add(LineBard3RightUp);
        AllLines.Add(LineBard4Left);
        AllLines.Add(LineBard4LeftUp);
    }

    private static void SetLineShaman(Transform lines)
    {
        Transform lineShaman = Utils.FindChild(lines, "$part_lines_right");
        LineCoreShaman = lineShaman.Find("$line_core/LineFill").GetComponent<Image>();
        LineShaman1Right = lineShaman.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineShaman1Left = lineShaman.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineShaman2Right = lineShaman.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineShaman2Left = lineShaman.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineShaman3Right = lineShaman.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineShaman3RightUp = lineShaman.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineShaman4Left = lineShaman.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineShaman4LeftUp = lineShaman.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreShaman);
        AllLines.Add(LineShaman1Right);
        AllLines.Add(LineShaman1Left);
        AllLines.Add(LineShaman2Right);
        AllLines.Add(LineShaman2Left);
        AllLines.Add(LineShaman3Right);
        AllLines.Add(LineShaman3RightUp);
        AllLines.Add(LineShaman4Left);
        AllLines.Add(LineShaman4LeftUp);
    }

    private static void SetLineSage(Transform lines)
    {
        Transform lineSage = Utils.FindChild(lines, "$part_lines_down_right");
        LineCoreSage = lineSage.Find("$line_core/LineFill").GetComponent<Image>();
        LineSage1Right = lineSage.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineSage1Left = lineSage.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineSage2Right = lineSage.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineSage2Left = lineSage.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineSage3Right = lineSage.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineSage3RightUp = lineSage.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineSage4Left = lineSage.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineSage4LeftUp = lineSage.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreSage);
        AllLines.Add(LineSage1Right);
        AllLines.Add(LineSage1Left);
        AllLines.Add(LineSage2Right);
        AllLines.Add(LineSage2Left);
        AllLines.Add(LineSage3Right);
        AllLines.Add(LineSage3RightUp);
        AllLines.Add(LineSage4Left);
        AllLines.Add(LineSage4LeftUp);
    }

    private static void SetLineDown(Transform lines)
    {
        Transform lineDown = lines.Find("$part_lines_down");
        LineCoreDown = lineDown.Find("$line_core/LineFill").GetComponent<Image>();
        LineDown1Right = lineDown.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineDown1Left = lineDown.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineDown2Right = lineDown.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineDown2Left = lineDown.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineDown3Right = lineDown.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineDown3RightDown = lineDown.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineDown4Left = lineDown.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineDown4LeftDown = lineDown.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreDown);
        AllLines.Add(LineDown1Right);
        AllLines.Add(LineDown1Left);
        AllLines.Add(LineDown2Right);
        AllLines.Add(LineDown2Left);
        AllLines.Add(LineDown3Right);
        AllLines.Add(LineDown3RightDown);
        AllLines.Add(LineDown4Left);
        AllLines.Add(LineDown4LeftDown);
    }

    private static void SetLineRanger(Transform lines)
    {
        Transform lineRanger = Utils.FindChild(lines, "$part_lines_down_left");
        LineCoreRanger = lineRanger.Find("$line_core/LineFill").GetComponent<Image>();
        LineRanger1Right = lineRanger.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineRanger1Left = lineRanger.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineRanger2Right = lineRanger.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineRanger2Left = lineRanger.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineRanger3Right = lineRanger.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineRanger3RightUp = lineRanger.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineRanger4Left = lineRanger.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineRanger4LeftUp = lineRanger.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreRanger);
        AllLines.Add(LineRanger1Right);
        AllLines.Add(LineRanger1Left);
        AllLines.Add(LineRanger2Right);
        AllLines.Add(LineRanger2Left);
        AllLines.Add(LineRanger3Right);
        AllLines.Add(LineRanger3RightUp);
        AllLines.Add(LineRanger4Left);
        AllLines.Add(LineRanger4LeftUp);
    }

    private static void SetLineRogue(Transform lines)
    {
        Transform lineRogue = Utils.FindChild(lines, "$part_lines_left");
        LineCoreRogue = lineRogue.Find("$line_core/LineFill").GetComponent<Image>();
        LineRogue1Right = lineRogue.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineRogue1Left = lineRogue.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineRogue2Right = lineRogue.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineRogue2Left = lineRogue.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineRogue3Right = lineRogue.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineRogue3RightUp = lineRogue.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineRogue4Left = lineRogue.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineRogue4LeftUp = lineRogue.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreRogue);
        AllLines.Add(LineRogue1Right);
        AllLines.Add(LineRogue1Left);
        AllLines.Add(LineRogue2Right);
        AllLines.Add(LineRogue2Left);
        AllLines.Add(LineRogue3Right);
        AllLines.Add(LineRogue3RightUp);
        AllLines.Add(LineRogue4Left);
        AllLines.Add(LineRogue4LeftUp);
    }

    private static void SetLineWarrior(Transform lines)
    {
        Transform lineWarrior = Utils.FindChild(lines, "$part_lines_up_left");
        LineCoreWarrior = lineWarrior.Find("$line_core/LineFill").GetComponent<Image>();
        LineWarrior1Right = lineWarrior.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineWarrior1Left = lineWarrior.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineWarrior2Right = lineWarrior.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineWarrior2Left = lineWarrior.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineWarrior3Right = lineWarrior.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineWarrior3RightUp = lineWarrior.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineWarrior4Left = lineWarrior.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineWarrior4LeftUp = lineWarrior.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineCoreWarrior);
        AllLines.Add(LineWarrior1Right);
        AllLines.Add(LineWarrior1Left);
        AllLines.Add(LineWarrior2Right);
        AllLines.Add(LineWarrior2Left);
        AllLines.Add(LineWarrior3Right);
        AllLines.Add(LineWarrior3RightUp);
        AllLines.Add(LineWarrior4Left);
        AllLines.Add(LineWarrior4LeftUp);
    }

    private static void SetLineRadial(Transform lines)
    {
        Transform lineRadial = Utils.FindChild(lines, "$part_lines_radial");
        LineRadial1 = lineRadial.Find("$line_1/$part_line/LineFill").GetComponent<Image>();
        LineRadial2 = lineRadial.Find("$line_2/$part_line/LineFill").GetComponent<Image>();
        LineRadial3 = lineRadial.Find("$line_3/$part_line/LineFill").GetComponent<Image>();
        LineRadial4 = lineRadial.Find("$line_4/$part_line/LineFill").GetComponent<Image>();
        LineRadial5 = lineRadial.Find("$line_5/$part_line/LineFill").GetComponent<Image>();
        LineRadial6 = lineRadial.Find("$line_6/$part_line/LineFill").GetComponent<Image>();
        LineRadial7 = lineRadial.Find("$line_7/$part_line/LineFill").GetComponent<Image>();
        LineRadial8 = lineRadial.Find("$line_8/$part_line/LineFill").GetComponent<Image>();
        
        AllLines.Add(LineRadial1);
        AllLines.Add(LineRadial2);
        AllLines.Add(LineRadial3);
        AllLines.Add(LineRadial4);
        AllLines.Add(LineRadial5);
        AllLines.Add(LineRadial6);
        AllLines.Add(LineRadial7);
        AllLines.Add(LineRadial8);
    }
    #endregion

    private static void LoadCloseButton()
    {
        Transform closeButton = Utils.FindChild(SkillTree_UI.transform, "$button_close");
        if (closeButton.TryGetComponent(out Button component))
        {
            component.transition = Selectable.Transition.SpriteSwap;
            component.spriteState = buttonComponent.spriteState;
            component.onClick.AddListener(TalentBook.HideUI);
        }

        closeButton.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = sfx.m_sfxPrefab;
    }

    private static void LoadResetButton()
    {
        Transform resetButton = Utils.FindChild(SkillTree_UI.transform, "$button_reset");
        if (resetButton.TryGetComponent(out Button component))
        {
            component.transition = Selectable.Transition.SpriteSwap;
            component.spriteState = buttonComponent.spriteState;
            component.onClick.AddListener(() =>
            {
                if (Player.m_localPlayer.GetInventory().CountItems("$item_coins") < AlmanacClassesPlugin._ResetCost.Value)
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"Cost {AlmanacClassesPlugin._ResetCost.Value} $item_coins to reset talents");
                    return;
                }

                Player.m_localPlayer.GetInventory().RemoveItem("$item_coins", AlmanacClassesPlugin._ResetCost.Value);
                ResetTalents();
            });
        }

        resetButton.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = sfx.m_sfxPrefab;
    }

    public static void ResetTalents(bool command = false)
    {
        LoadTwoHanded.ResetTwoHandedWeapons();
        SpellBook.DestroyElements();
        SpellBook.m_abilities.Clear();
        List<StatusEffect> effects = Player.m_localPlayer.GetSEMan().GetStatusEffects().FindAll(x => x is StatusEffectManager.Data.TalentEffect);
        foreach (StatusEffect effect in effects)
        {
            Player.m_localPlayer.GetSEMan().RemoveStatusEffect(effect);
        }
        PlayerManager.m_tempPlayerData.m_boughtTalents.Clear();
        foreach (Button talent in CheckedTalents)
        {
            Transform checkmark = Utils.FindChild(talent.transform, "Checkmark");
            if (!checkmark) continue;
            checkmark.gameObject.SetActive(false);
        }
        CheckedTalents.Clear();
        CheckedTalents.Add(CenterButton);
        foreach (Image line in AllLines) line.fillAmount = 0f;
                
        PlayerManager.m_playerTalents.Clear();
        PlayerManager.m_tempPlayerData.m_boughtTalents.Clear();
        CharacteristicManager.ReloadCharacteristics();
        if (!command) TalentBook.ShowUI();
    }

    private static void AddSFXToTalentButtons()
    {
        TalentButtons.Clear();
        Transform talents = Utils.FindChild(SkillTree_UI.transform, "$part_talents");
        Button[] buttons = talents.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            TalentButtons.Add(button);
            ButtonMap[button.name] = button;
            button.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = sfx.m_sfxPrefab;
        }
    }

    private static void SetAllButtonEvents()
    {
        ButtonFillLineMap.Clear();
        ButtonCoreLineAmountMap.Clear();
        CheckedTalents.Clear();
        CenterButton = Utils.FindChild(SkillTree_UI.transform, "$button_center").GetComponent<Button>();
        CheckedTalents.Add(CenterButton);
        SetCoreButtonsEvents();
        SetBardButtonEvents();
        SetShamanButtonEvents();
        SetSageButtonEvents();
        SetRangerButtonEvents();
        SetRogueButtonEvents();
        SetWarriorButtonEvents();
    }

    private static void SetCoreButtonsEvents()
    {
        Transform? CoreCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_core_characteristics");
        Transform? CoreTalents = Utils.FindChild(SkillTree_UI.transform, "$part_core_talents");
        SetButton(CoreCharacteristics, "$button_core_1", new()
        {
            {"$button_center", LineCoreUp}, {"$button_bard_talent_1", LineUp1Left}, {"$button_warrior_talent_1", LineUp1Right}
        }, 0.4f, "Core1");
        SetButton(CoreCharacteristics, "$button_core_2", new(){{"$button_core_1", LineCoreUp}}, 0.53f, "Core2");
        SetButton(CoreCharacteristics, "$button_core_3", new(){{"$button_core_2", LineUp2Right}}, 1f, "Core3");
        SetButton(CoreCharacteristics, "$button_core_4", new(){{"$button_core_2", LineUp2Left}}, 1f, "Core4");
        SetButton(CoreCharacteristics, "$button_core_5", new()
        {
            {"$button_lumberjack", LineUp3Right}, {"$button_warrior_6", LineRadial8}
        }, 1f, "Core5");
        SetButton(CoreCharacteristics, "$button_core_6", new(){{"$button_lumberjack", LineUp4Left}, {"$button_bard_5", LineRadial1}}, 1f, "Core6");
        SetButton(CoreTalents, "$button_lumberjack", new(){{"$button_core_2", LineCoreUp},{"$button_core_6", LineUp4Left}, {"$button_core_5", LineUp3Right}}, 0.7f, "CoreComfort1");
        SetButton(CoreTalents, "$button_chef", new(){{"$button_lumberjack", LineCoreUp}}, 1f, "CoreChef");
        SetButton(CoreTalents, "$button_comfort_1", new(){{"$button_core_5", LineUp3RightUp}}, 1f, "CoreLumberjack");
        SetButton(CoreTalents, "$button_comfort_2", new(){{"$button_core_6", LineUp4LeftUp}}, 1f, "CoreComfort2");
        
        SetButton(CoreCharacteristics, "$button_core_7", new(){{"$button_center", LineCoreDown}, {"$button_sneak", LineDown1Right}, {"$button_merchant", LineDown1Left}}, 0.4f, "Core7");
        SetButton(CoreCharacteristics, "$button_core_8", new(){{"$button_core_7", LineCoreDown}}, 0.53f, "Core8");
        SetButton(CoreCharacteristics, "$button_core_9", new(){{"$button_core_8", LineDown2Right}}, 1f, "Core9");
        SetButton(CoreCharacteristics, "$button_core_10", new(){{"$button_core_8", LineDown2Left}}, 1f, "Core10");
        SetButton(CoreCharacteristics, "$button_core_11", new(){{"$button_treasure", LineDown3Right}, {"$button_sage_6", LineRadial4}}, 1f, "Core11");
        SetButton(CoreCharacteristics, "$button_core_12", new(){{"$button_treasure", LineDown4Left}, {"$button_ranger_5", LineRadial5}}, 1f, "Core12");
        SetButton(CoreTalents, "$button_treasure", new(){{"$button_core_8", LineCoreDown}, {"$button_core_11", LineDown3Right}, {"$button_core_12", LineDown4Left}}, 0.7f, "CoreTreasure");
        SetButton(CoreTalents, "$button_sneak", new(){{"$button_core_7", LineDown1Right}, {"$button_sage_1", LineSage1Left}}, 1f, "CoreSneak");
        SetButton(CoreTalents, "$button_merchant", new(){{"$button_core_7", LineDown1Left}, {"$button_ranger_1", LineRanger1Right}}, 1f, "CoreMerchant");
        SetButton(CoreTalents, "$button_shield", new(){{"$button_core_11", LineDown3RightDown}}, 1f, "CoreCarry");
        SetButton(CoreTalents, "$button_rain", new(){{"$button_core_12", LineDown4LeftDown}}, 1f, "RainProof");
        SetButton(CoreTalents, "$button_sail", new(){{"$button_treasure", LineCoreDown}}, 1f, "CoreSail");
    }

    private static void SetBardButtonEvents()
    {
        Transform? BardTalents = Utils.FindChild(SkillTree_UI.transform, "$part_bard_talents");
        Transform? BardCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_bard_characteristics");
        SetButton(BardCharacteristics, "$button_bard_1", new(){{"$button_center", LineCoreBard}, {"$button_bard_talent_1", LineBard1Right}, {"$button_shaman_1", LineBard1Left}}, 0.4f, "Bard1");
        SetButton(BardCharacteristics, "$button_bard_2", new(){{"$button_bard_1", LineCoreBard}}, 0.53f, "Bard2");
        SetButton(BardCharacteristics, "$button_bard_3", new(){{"$button_bard_2", LineBard2Right}}, 1f, "Bard3");
        SetButton(BardCharacteristics, "$button_bard_4", new(){{"$button_bard_2", LineBard2Left}}, 1f, "Bard4");
        SetButton(BardCharacteristics, "$button_bard_5", new(){{"$button_bard_talent_2", LineBard3Right}, {"$button_core_6", LineRadial1}}, 1f, "Bard5");
        SetButton(BardCharacteristics, "$button_bard_6", new(){{"$button_bard_talent_2", LineBard4Left},{"$button_shaman_5", LineRadial2}}, 1f, "Bard6");
        SetButton(BardTalents, "$button_bard_talent_1", new(){{"$button_core_1", LineUp1Left}, {"$button_bard_1", LineBard1Right}}, 1f, "SongOfSpeed");
        SetButton(BardTalents, "$button_bard_talent_2", new(){{"$button_bard_2", LineCoreBard}, {"$button_bard_5", LineBard3Right}, {"$button_bard_6", LineBard4Left}}, 0.7f, "SongOfVitality");
        SetButton(BardTalents, "$button_bard_talent_3", new(){{"$button_bard_5", LineBard3RightUp}}, 1f, "SongOfDamage");
        SetButton(BardTalents, "$button_bard_talent_4", new(){{"$button_bard_6", LineBard4LeftUp}}, 1f, "SongOfHealing");
        SetButton(BardTalents, "$button_bard_talent_5", new(){{"$button_bard_talent_2", LineCoreBard}}, 1f, "SongOfSpirit");
    }

    private static void SetShamanButtonEvents()
    {
        Transform ShamanTalents = Utils.FindChild(SkillTree_UI.transform, "$part_shaman_talents");
        Transform ShamanCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_shaman_characteristics");
        SetButton(ShamanCharacteristics, "$button_shaman_1", new()
        {
            {"$button_center", LineCoreShaman}, {"$button_shaman_talent_1", LineShaman1Right}, {"$button_sage_talent_1", LineShaman1Left}
        }, 0.4f, "Shaman1");
        SetButton(ShamanCharacteristics, "$button_shaman_2", new(){{"$button_shaman_1", LineCoreShaman}}, 0.53f, "Shaman2");
        SetButton(ShamanCharacteristics, "$button_shaman_3", new(){{"$button_shaman_2", LineShaman2Right}}, 1f, "Shaman3");
        SetButton(ShamanCharacteristics, "$button_shaman_4", new(){{"$button_shaman_2", LineShaman2Left}}, 1f, "Shaman4");
        SetButton(ShamanCharacteristics, "$button_shaman_5", new(){{"$button_shaman_talent_2", LineShaman3Right}, {"$button_bard_6", LineRadial2}}, 1f, "Shaman5");
        SetButton(ShamanCharacteristics, "$button_shaman_6", new(){{"$button_shaman_talent_2", LineShaman4Left}, {"$button_sage_5", LineRadial3}}, 1f, "Shaman6");
        SetButton(ShamanTalents, "$button_shaman_talent_1", new(){{"$button_bard_1", LineBard1Left}, {"$button_shaman_1", LineShaman1Right}}, 1f, "ShamanHeal");
        SetButton(ShamanTalents, "$button_shaman_talent_2", new(){{"$button_shaman_2", LineCoreShaman}, {"$button_shaman_5", LineShaman3Right}, {"$button_shaman_6",LineShaman4Left}}, 0.7f, "RootBeam");
        SetButton(ShamanTalents, "$button_shaman_talent_3", new(){{"$button_shaman_5", LineShaman3RightUp}}, 1f, "ShamanGhosts");
        SetButton(ShamanTalents, "$button_shaman_talent_4", new(){{"$button_shaman_6", LineShaman4LeftUp}}, 1f, "ShamanRegeneration");
        SetButton(ShamanTalents, "$button_shaman_talent_5", new(){{"$button_shaman_talent_2", LineCoreShaman}}, 1f, "ShamanShield");
    }

    private static void SetSageButtonEvents()
    {
        Transform SageTalents = Utils.FindChild(SkillTree_UI.transform, "$part_sage_talents");
        Transform SageCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_sage_characteristics");
        SetButton(SageCharacteristics, "$button_sage_1", new(){{"$button_center", LineCoreSage}, {"$button_sage_talent_1", LineSage1Right}, {"$button_sneak", LineSage1Left}}, 0.4f, "Sage1");
        SetButton(SageCharacteristics, "$button_sage_2", new(){{"$button_sage_1", LineCoreSage}}, 0.53f, "Sage2");
        SetButton(SageCharacteristics, "$button_sage_3", new(){{"$button_sage_2", LineSage2Right}}, 1f, "Sage3");
        SetButton(SageCharacteristics, "$button_sage_4", new(){{"$button_sage_2", LineSage2Left}}, 1f, "Sage4");
        SetButton(SageCharacteristics, "$button_sage_5", new(){{"$button_sage_talent_4", LineSage3Right}, {"$button_shaman_6", LineRadial3}}, 1f, "Sage5");
        SetButton(SageCharacteristics, "$button_sage_6", new(){{"$button_sage_talent_4", LineSage4Left}, {"$button_core_11", LineRadial4}}, 1f, "Sage6");
        SetButton(SageTalents, "$button_sage_talent_1", new(){{"$button_shaman_1", LineShaman1Left}, {"$button_sage_1", LineSage1Right}}, 1f, "StoneThrow");
        SetButton(SageTalents, "$button_sage_talent_4", new(){{"$button_sage_2", LineCoreSage}, {"$button_sage_6", LineSage4Left}, {"$button_sage_5", LineSage3Right}}, 0.7f, "CallOfLightning");
        SetButton(SageTalents, "$button_sage_talent_3", new(){{"$button_sage_6", LineSage4LeftUp}}, 1f, "MeteorStrike");
        SetButton(SageTalents, "$button_sage_talent_2", new(){{"$button_sage_5", LineSage3RightUp}}, 1f, "GoblinBeam");
        SetButton(SageTalents, "$button_sage_talent_5", new(){{"$button_sage_talent_4", LineCoreSage}}, 1f, "IceBreath");
    }

    private static void SetRangerButtonEvents()
    {
        Transform RangerTalents = Utils.FindChild(SkillTree_UI.transform, "$part_ranger_talents");
        Transform RangerCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_ranger_characteristics");
        SetButton(RangerCharacteristics, "$button_ranger_1", new()
        {
            {"$button_center", LineCoreRanger},
            {"$button_ranger_talent_1", LineRanger1Left},
            {"$button_merchant", LineRanger1Right}
        }, 0.4f, "Ranger1");
        SetButton(RangerCharacteristics, "$button_ranger_2", new(){{"$button_ranger_1", LineCoreRanger}}, 0.53f, "Ranger2");
        SetButton(RangerCharacteristics, "$button_ranger_3", new(){{"$button_ranger_2", LineRanger2Right}}, 1f, "Ranger3");
        SetButton(RangerCharacteristics, "$button_ranger_4", new(){{"$button_ranger_2", LineRanger2Left}}, 1f, "Ranger4");
        SetButton(RangerCharacteristics, "$button_ranger_5", new()
        {
            {"$button_ranger_talent_2", LineRanger3Right},
            {"$button_core_12", LineRadial5}
        }, 1f, "Ranger5");
        SetButton(RangerCharacteristics, "$button_ranger_6", new()
        {
            {"$button_ranger_talent_2", LineRanger4Left},
            {"$button_rogue_5", LineRadial6}
        }, 1f, "Ranger6");
        SetButton(RangerTalents, "$button_ranger_talent_1", new()
        {
            {"$button_rogue_1", LineRogue1Right},
            {"$button_ranger_1", LineRanger1Left}
        }, 1f, "DeerHunter");
        SetButton(RangerTalents, "$button_ranger_talent_2", new()
        {
            {"$button_ranger_2", LineCoreRanger},
            {"$button_ranger_5", LineRanger3Right},
            {"$button_ranger_6",LineRanger4Left}
        }, 0.7f, "LuckyShot");
        SetButton(RangerTalents, "$button_ranger_talent_3", new(){{"$button_ranger_5", LineRanger3RightUp}}, 1f, "RangerTamer");
        SetButton(RangerTalents, "$button_ranger_talent_4", new(){{"$button_ranger_6", LineRanger4LeftUp}}, 1f, "RangerTrap");
        SetButton(RangerTalents, "$button_ranger_talent_5", new(){{"$button_ranger_talent_2", LineCoreRanger}}, 1f, "QuickShot");
    }

    private static void SetRogueButtonEvents()
    {
        Transform RogueTalents = Utils.FindChild(SkillTree_UI.transform, "$part_rogue_talents");
        Transform RogueCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_rogue_characteristics");
        SetButton(RogueCharacteristics, "$button_rogue_1", new()
        {
            {"$button_center", LineCoreRogue},
            {"$button_ranger_talent_1", LineRogue1Right},
            {"$button_rogue_talent_1", LineRogue1Left}
        }, 0.4f, "Rogue1");
        SetButton(RogueCharacteristics, "$button_rogue_2", new(){{"$button_rogue_1", LineCoreRogue}}, 0.53f, "Rogue2");
        SetButton(RogueCharacteristics, "$button_rogue_3", new(){{"$button_rogue_2", LineRogue2Right}}, 1f, "Rogue3");
        SetButton(RogueCharacteristics, "$button_rogue_4", new(){{"$button_rogue_2", LineRogue2Left}}, 1f, "Rogue4");
        SetButton(RogueCharacteristics, "$button_rogue_5", new()
        {
            {"$button_rogue_talent_2", LineRogue3Right},
            {"$button_ranger_6", LineRadial6}
        }, 1f, "Rogue5");
        SetButton(RogueCharacteristics, "$button_rogue_6", new()
        {
            {"$button_rogue_talent_2", LineRogue4Left},
            {"$button_warrior_5", LineRadial7}
        }, 1f, "Rogue6");
        SetButton(RogueTalents, "$button_rogue_talent_1", new()
        {
            {"$button_rogue_1", LineRogue1Left},
            {"$button_warrior_1", LineWarrior1Right}
        }, 1f, "RogueSpeed");
        SetButton(RogueTalents, "$button_rogue_talent_2", new()
        {
            {"$button_rogue_2", LineCoreRogue},
            {"$button_rogue_5", LineRogue3Right},
            {"$button_rogue_6",LineRogue4Left}
        }, 0.7f, "RogueReflect");
        SetButton(RogueTalents, "$button_rogue_talent_3", new(){{"$button_rogue_5", LineRogue3RightUp}}, 1f, "RogueDamage");
        SetButton(RogueTalents, "$button_rogue_talent_4", new(){{"$button_rogue_6", LineRogue4LeftUp}}, 1f, "RogueStamina");
        SetButton(RogueTalents, "$button_rogue_talent_5", new(){{"$button_rogue_talent_2", LineCoreRogue}}, 1f, "RogueBleed");
    }

    private static void SetWarriorButtonEvents()
    {
        Transform WarriorTalents = Utils.FindChild(SkillTree_UI.transform, "$part_warrior_talents");
        Transform WarriorCharacteristics = Utils.FindChild(SkillTree_UI.transform, "$part_warrior_characteristics");
        SetButton(WarriorCharacteristics, "$button_warrior_1", new()
        {
            {"$button_center", LineCoreWarrior},
            {"$button_rogue_talent_1", LineWarrior1Left},
            {"$button_warrior_talent_1", LineWarrior1Right}
        }, 0.4f, "Warrior1");
        SetButton(WarriorCharacteristics, "$button_warrior_2", new(){{"$button_warrior_1", LineCoreWarrior}}, 0.53f, "Warrior2");
        SetButton(WarriorCharacteristics, "$button_warrior_3", new(){{"$button_warrior_2", LineWarrior2Right}}, 1f, "Warrior3");
        SetButton(WarriorCharacteristics, "$button_warrior_4", new(){{"$button_warrior_2", LineWarrior2Left}}, 1f, "Warrior4");
        SetButton(WarriorCharacteristics, "$button_warrior_5", new()
        {
            {"$button_warrior_talent_2", LineWarrior3Right},
            {"$button_rogue_6", LineRadial7}
        }, 1f, "Warrior5");
        SetButton(WarriorCharacteristics, "$button_warrior_6", new()
        {
            {"$button_warrior_talent_2", LineWarrior4Left},
            {"$button_core_5", LineRadial8}
        }, 1f, "Warrior6");
        SetButton(WarriorTalents, "$button_warrior_talent_1", new()
        {
            {"$button_warrior_1", LineWarrior1Left},
            {"$button_core_1", LineUp1Right}
        }, 1f, "WarriorStrength");
        SetButton(WarriorTalents, "$button_warrior_talent_2", new()
        {
            {"$button_warrior_2", LineCoreWarrior},
            {"$button_warrior_5", LineWarrior3Right},
            {"$button_warrior_6",LineWarrior4Left}
        }, 0.7f, "WarriorVitality");
        SetButton(WarriorTalents, "$button_warrior_talent_3", new(){{"$button_warrior_5", LineWarrior3RightUp}}, 1f, "WarriorResistance");
        SetButton(WarriorTalents, "$button_warrior_talent_4", new(){{"$button_warrior_6", LineWarrior4LeftUp}}, 1f, "MonkeyWrench");
        SetButton(WarriorTalents, "$button_warrior_talent_5", new(){{"$button_warrior_talent_2", LineCoreWarrior}}, 1f, "DualWield");
    }

    private static void SetButton(Transform parent, string name, Dictionary<string, Image> lines, float amount, string key)
    {
        Button? button = parent.Find(name).GetComponent<Button>();
        ButtonFillLineMap[button] = lines;
        ButtonCoreLineAmountMap[button] = amount;
        Transform? checkmark = Utils.FindChild(button.transform, "Checkmark");
        button.onClick.AddListener(() =>
        {
            if (CheckedTalents.Contains(button)) return;
            
            if (EndTalents.Keys.Contains(name))
            {
                if (!CanBuyEndAbility(name))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_need_connected_talents");
                    return;
                }
            }
            Dictionary<string, Image> validatedLines = new();
            foreach (Button talent in CheckedTalents)
            {
                if (lines.TryGetValue(talent.name, out Image line))
                {
                    validatedLines[talent.name] = line;
                }
            }

            if (validatedLines.Count == 0)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_need_previous_talent");
                return;
            }
            
            if (TalentManager.AllTalents.TryGetValue(key, out Talent ability))
            {
                int cost = ability.m_cost;
                if (cost > TalentManager.GetAvailableTalentPoints())
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_not_enough_tp");
                    return;
                }
                PlayerManager.m_playerTalents[ability.m_key] = ability;
                PlayerManager.m_tempPlayerData.m_boughtTalents.Add(ability.m_key);
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_purchased");
                if (ability.m_key == "MonkeyWrench")
                {
                    LoadTwoHanded.ModifyTwoHandedWeapons();
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_two_handed");
                }
                CharacteristicManager.ReloadCharacteristics();
                TalentBook.ShowUI();
                if (ability.m_triggerNow)
                {
                    StatusEffect effect = ObjectDB.instance.GetStatusEffect(ability.m_statusEffect.name.GetStableHashCode());
                    if (effect)
                    {
                        if (!Player.m_localPlayer.GetSEMan().HaveStatusEffect(effect.name))
                        {
                            Player.m_localPlayer.GetSEMan().AddStatusEffect(effect);
                        }
                    }
                }
                else if (ability.m_isAbility)
                {
                    if (SpellBook.IsAbilityInBook(ability))
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_in_book");
                        return;
                    }
                    if (SpellBook.m_abilities.Count > 7)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_spell_book_full");
                        return;
                    }
                    SpellBook.m_abilities.Add(new AbilityData()
                    {
                        m_data = ability
                    });
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_added_spell");
                }
            }
            else
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_failed_to_get_talent");
                return;
            }
            checkmark.gameObject.SetActive(true);
            foreach (KeyValuePair<string, Image> line in validatedLines)
            {
                line.Value.fillAmount = line.Value.transform.parent.name == "$line_core" ? amount : 1f;
            }
            CheckedTalents.Add(button);
        });
    }

    private static bool CanBuyEndAbility(string button)
    {
        return !EndTalents.TryGetValue(button, out List<string> requirements) || requirements.All(requirement => CheckedTalents.Find(x => x.name == requirement));
    }

    private static void SetTextFont()
    {
        Font? NorseBold = GetFont("Norsebold");
        Text[] texts = SkillTree_UI.GetComponentsInChildren<Text>();
        AddFonts(texts, NorseBold);
    }
    
    public static void AddFonts(Text[] array, Font? font)
    {
        foreach (Text text in array) text.font = font;
    }
    
    public static Font? GetFont(string name)
    {
        Font[]? fonts = Resources.FindObjectsOfTypeAll<Font>();
        return fonts.FirstOrDefault(x => x.name == name);
    }
    
    public static string GetTooltip(Talent talent)
    {
        StringBuilder stringBuilder = new StringBuilder();
        switch (talent.m_type)
        {
            case TalentType.Characteristic:
                string key = DefaultData.LocalizeCharacteristics[talent.m_characteristic];
                int value = talent.m_characteristicValue * talent.m_level;
                stringBuilder.Append($"+ <color=orange>{value}</color> {key}");
                return Localization.instance.Localize(stringBuilder.ToString());
            default:
                stringBuilder.Append(talent.m_description);
                break;
        }
        stringBuilder.Append("\n");

        if (talent.m_skill is not Skills.SkillType.None)
        {
            stringBuilder.Append($"<color=#1D5DEC>$skill_{talent.m_skill.ToString().ToLower()}</color>\n");
        }

        if (talent.m_ttl != null)
        {
            stringBuilder.Append($"$almanac_cooldown: <color=orange>{talent.m_ttl.Value}</color>sec\n");
        }

        if (talent.m_duration != null)
        {
            stringBuilder.Append($"$almanac_duration: <color=orange>{talent.m_duration.Value}</color>sec\n");
        }

        if (talent.m_eitrCost != null)
        {
            stringBuilder.Append($"$se_eitr $almanac_cost: <color=orange>{talent.m_eitrCost.Value}</color>\n");
        }

        if (talent.m_staminaCost != null)
        {
            stringBuilder.Append($"$se_stamina $almanac_cost: <color=orange>{talent.m_staminaCost.Value}</color>\n");
        }

        if (talent.m_healthCost != null)
        {
            stringBuilder.Append($"$se_health $almanac_cost: <color=orange>{talent.m_healthCost.Value}</color>\n");
        }
        
        foreach (KeyValuePair<StatusEffectData.Modifier, ConfigEntry<float>> kvp in talent.m_modifiers)
        {
            string key = DefaultData.LocalizeModifiers[kvp.Key];
            switch (kvp.Key)
            {
                case StatusEffectData.Modifier.MaxCarryWeight 
                    or StatusEffectData.Modifier.Vitality 
                    or StatusEffectData.Modifier.Stamina 
                    or StatusEffectData.Modifier.Eitr:
                    float amount = kvp.Value.Value * talent.m_level;
                    stringBuilder.Append($"{key}: <color=orange>{amount}</color>\n");
                    break;
                case StatusEffectData.Modifier.Heal 
                    or StatusEffectData.Modifier.DamageAbsorb:
                    float value = Spells.ApplySkill(talent.m_skill, kvp.Value.Value * talent.m_level);
                    stringBuilder.Append($"{key}: <color=orange>{value}</color>\n");
                    break;
                case StatusEffectData.Modifier.Speed
                    or StatusEffectData.Modifier.Attack
                    or StatusEffectData.Modifier.HealthRegen
                    or StatusEffectData.Modifier.StaminaRegen
                    or StatusEffectData.Modifier.RaiseSkills
                    or StatusEffectData.Modifier.Noise
                    or StatusEffectData.Modifier.RunStaminaDrain
                    or StatusEffectData.Modifier.EitrRegen
                    or StatusEffectData.Modifier.Reflect:
                    float skilledPercentage = Spells.ApplySkill(talent.m_skill, kvp.Value.Value * talent.m_level) * 100;
                    stringBuilder.Append($"{key}: <color=orange>{skilledPercentage}</color>%\n");
                    break;
                default:
                    float percentage = kvp.Value.Value * 100;
                    stringBuilder.Append($"{key}: <color=orange>{percentage}</color>%\n");
                    break;
            }
        }
        // Get damages applies talent level
        if (talent.m_talentDamages != null)
        {
            HitData.DamageTypes damages = TalentManager.GetDamages(talent);
            if (damages.HaveDamage())
            {
                damages.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(talent.m_skill), 0.1f, 1f));
                if (damages.m_blunt > 0)
                {
                    stringBuilder.Append($"$inventory_blunt: <color=orange>{damages.m_blunt}</color>\n");
                }

                if (damages.m_slash > 0)
                {
                    stringBuilder.Append($"$inventory_slash: <color=orange>{damages.m_slash}</color>\n");
                }

                if (damages.m_pierce > 0)
                {
                    stringBuilder.Append($"$inventory_pierce: <color=orange>{damages.m_pierce}</color>\n");
                }

                if (damages.m_fire > 0)
                {
                    stringBuilder.Append($"$inventory_fire: <color=orange>{damages.m_fire}</color>\n");
                }

                if (damages.m_frost > 0)
                {
                    stringBuilder.Append($"$inventory_frost: <color=orange>{damages.m_frost}</color>\n");
                }

                if (damages.m_lightning > 0)
                {
                    stringBuilder.Append($"$inventory_lightning: <color=orange>{damages.m_lightning}</color>\n");
                }

                if (damages.m_poison > 0)
                {
                    stringBuilder.Append($"$inventory_poison: <color=orange>{damages.m_poison}</color>\n");
                }

                if (damages.m_spirit > 0)
                {
                    stringBuilder.Append($"$inventory_spirit: <color=orange>{damages.m_spirit}</color>\n");
                }
            }
        }

        foreach (KeyValuePair<HitData.DamageType, ConfigEntry<HitData.DamageModifier>> kvp in talent.m_resistances)
        {
            string key = DefaultData.LocalizeDamageType[kvp.Key];
            string mod = DefaultData.LocalizeDamageModifier[kvp.Value.Value];
            stringBuilder.Append($"<color=orange>{mod}</color> VS <color=orange>{key}</color>\n");
        }

        if (talent.m_chance != null)
        {
            stringBuilder.Append($"$almanac_chance: <color=orange>{talent.m_chance.Value * talent.m_level}</color>%\n");
        }

        if (talent.m_heal != null)
        {
            stringBuilder.Append($"$almanac_heal: <color=orange>{Spells.ApplySkill(talent.m_skill, talent.m_heal.Value * talent.m_level)}</color>\n");
        }

        if (talent.m_comfortAmount != null)
        {
            stringBuilder.Append($"$almanac_comfort: <color=orange>{talent.m_comfortAmount.Value * talent.m_level}</color>\n");
        }

        if (talent.m_key == "CoreChef")
        {
            stringBuilder.Append($"$text_food_boost: <color=orange>{(AlmanacClassesPlugin._MasterChefIncrease.Value * talent.m_level) * 100}</color>%\n");
        }

        if (talent.m_key == "CoreLumberjack")
        {
            stringBuilder.Append($"$inventory_chop: <color=orange>{(AlmanacClassesPlugin._LumberjackIncrease.Value * talent.m_level) * 100}</color>%\n");
        }

        return Localization.instance.Localize(stringBuilder.ToString());
    }
}