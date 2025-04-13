using System.Collections.Generic;
using AlmanacClasses.Classes;
using AlmanacClasses.Managers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AlmanacClasses.UI;

public static class LoadUI
{
    public static bool m_hudInitialized;
    public static UIManager? UIManager;
    
    [Header("Assets")]
    public static ButtonSfx m_vanillaButtonSFX = null!;
    public static Button m_vanillaButton = null!;

    [Header("Fill Lines")]
    #region All Fill Lines Images
    #region Line Up
    private static Image LineCoreUp = null!;
    private static Image LineCoreUp2 = null!;
    private static Image LineCoreUp3 = null!;
    private static Image LineCoreUp4 = null!;
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
    private static Image LineCoreBard2 = null!;
    private static Image LineCoreBard3 = null!;
    private static Image LineCoreBard4 = null!;
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
    private static Image LineCoreShaman2 = null!;
    private static Image LineCoreShaman3 = null!;
    private static Image LineCoreShaman4 = null!;
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
    private static Image LineCoreSage2 = null!;
    private static Image LineCoreSage3 = null!;
    private static Image LineCoreSage4 = null!;
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
    private static Image LineCoreDown2 = null!;
    private static Image LineCoreDown3 = null!;
    private static Image LineCoreDown4 = null!;

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
    private static Image LineCoreRanger2 = null!;
    private static Image LineCoreRanger3 = null!;
    private static Image LineCoreRanger4 = null!;

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
    private static Image LineCoreRogue2 = null!;
    private static Image LineCoreRogue3 = null!;
    private static Image LineCoreRogue4 = null!;

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
    private static Image LineCoreWarrior2 = null!;
    private static Image LineCoreWarrior3 = null!;
    private static Image LineCoreWarrior4 = null!;

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

    public static readonly Dictionary<string, List<string>> EndTalents = new()
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
    public static void InitHud()
    {
        if (m_hudInitialized || !Hud.instance) return;
        
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Initializing HUD");
        Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("Experience_Bar"), Hud.instance.transform, false).AddComponent<ExperienceBar>().Init();
        Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_UI"), Hud.instance.transform, false).AddComponent<SpellBook>().Init();
        Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("PassiveBar_UI"), Hud.instance.transform, false).AddComponent<PassiveBar>().Init();
        Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("ElementHover_UI"), Hud.instance.transform, false).AddComponent<SpellInfo>().Init();
        
        FontManager.SetFont(ExperienceBar.m_instance.m_texts);
        FontManager.SetFont(SpellBook.m_instance.m_elementTexts.ToArray());
        FontManager.SetFont(PassiveBar.m_element.GetComponentsInChildren<Text>());
        FontManager.SetFont(SpellInfo.m_instance.m_texts);
        SpellBook.m_instance.UpdateFontSize();

        UIManager = new GameObject("UIManager").AddComponent<UIManager>();
        UIManager.Initialize(SpellBook.m_instance, PassiveBar.m_instance, ExperienceBar.m_instance, SpellInfo.m_instance);
        
        m_hudInitialized = true;
    }
    public static void InitSkillTree(InventoryGui instance)
    {
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Client: Initializing talent UI");
        Transform vanillaButton = Utils.FindChild(Utils.FindChild(instance.transform, "TrophiesFrame"), "Closebutton");
        m_vanillaButtonSFX = vanillaButton.GetComponent<ButtonSfx>();
        m_vanillaButton = vanillaButton.GetComponent<Button>();
        
        Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("Almanac_SkillTree"), instance.transform, false).AddComponent<SkillTree>().Init();
        FontManager.SetFont(SkillTree.m_instance.m_texts);
        FontManager.SetFont(SpellInventory.m_element.GetComponentsInChildren<Text>());
        RegisterButtons();
        RegisterFillLines();
        SetAllButtonEvents();
    }
    
    #region Set Line Methods
    private static void RegisterFillLines()
    {
        Transform lines = Utils.FindChild(SkillTree.m_instance.transform, "$part_lines");
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
        LineCoreUp = lineUp.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreUp2 = lineUp.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreUp3 = lineUp.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreUp4 = lineUp.Find("$line_core_4/LineFill").GetComponent<Image>();
        LineUp1Right = lineUp.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineUp1Left = lineUp.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineUp2Right = lineUp.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineUp2Left = lineUp.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineUp3Right = lineUp.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineUp3RightUp = lineUp.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineUp4Left = lineUp.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineUp4LeftUp = lineUp.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineBard(Transform lines)
    {
        Transform lineBard = Utils.FindChild(lines, "$part_lines_up_right");
        LineCoreBard = lineBard.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreBard2 = lineBard.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreBard3 = lineBard.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreBard4 = lineBard.Find("$line_core_4/LineFill").GetComponent<Image>();
        LineBard1Right = lineBard.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineBard1Left = lineBard.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineBard2Right = lineBard.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineBard2Left = lineBard.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineBard3Right = lineBard.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineBard3RightUp = lineBard.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineBard4Left = lineBard.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineBard4LeftUp = lineBard.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineShaman(Transform lines)
    {
        Transform lineShaman = Utils.FindChild(lines, "$part_lines_right");
        LineCoreShaman = lineShaman.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreShaman2 = lineShaman.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreShaman3 = lineShaman.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreShaman4 = lineShaman.Find("$line_core_4/LineFill").GetComponent<Image>();
        LineShaman1Right = lineShaman.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineShaman1Left = lineShaman.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineShaman2Right = lineShaman.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineShaman2Left = lineShaman.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineShaman3Right = lineShaman.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineShaman3RightUp = lineShaman.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineShaman4Left = lineShaman.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineShaman4LeftUp = lineShaman.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineSage(Transform lines)
    {
        Transform lineSage = Utils.FindChild(lines, "$part_lines_down_right");
        LineCoreSage = lineSage.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreSage2 = lineSage.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreSage3 = lineSage.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreSage4 = lineSage.Find("$line_core_4/LineFill").GetComponent<Image>();
        LineSage1Right = lineSage.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineSage1Left = lineSage.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineSage2Right = lineSage.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineSage2Left = lineSage.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineSage3Right = lineSage.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineSage3RightUp = lineSage.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineSage4Left = lineSage.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineSage4LeftUp = lineSage.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineDown(Transform lines)
    {
        Transform lineDown = lines.Find("$part_lines_down");
        LineCoreDown = lineDown.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreDown2 = lineDown.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreDown3 = lineDown.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreDown4 = lineDown.Find("$line_core_4/LineFill").GetComponent<Image>();
        
        LineDown1Right = lineDown.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineDown1Left = lineDown.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineDown2Right = lineDown.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineDown2Left = lineDown.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineDown3Right = lineDown.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineDown3RightDown = lineDown.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineDown4Left = lineDown.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineDown4LeftDown = lineDown.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineRanger(Transform lines)
    {
        Transform lineRanger = Utils.FindChild(lines, "$part_lines_down_left");
        LineCoreRanger = lineRanger.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreRanger2 = lineRanger.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreRanger3 = lineRanger.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreRanger4 = lineRanger.Find("$line_core_4/LineFill").GetComponent<Image>();

        LineRanger1Right = lineRanger.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineRanger1Left = lineRanger.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineRanger2Right = lineRanger.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineRanger2Left = lineRanger.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineRanger3Right = lineRanger.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineRanger3RightUp = lineRanger.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineRanger4Left = lineRanger.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineRanger4LeftUp = lineRanger.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineRogue(Transform lines)
    {
        Transform lineRogue = Utils.FindChild(lines, "$part_lines_left");
        LineCoreRogue = lineRogue.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreRogue2 = lineRogue.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreRogue3 = lineRogue.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreRogue4 = lineRogue.Find("$line_core_4/LineFill").GetComponent<Image>();

        LineRogue1Right = lineRogue.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineRogue1Left = lineRogue.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineRogue2Right = lineRogue.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineRogue2Left = lineRogue.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineRogue3Right = lineRogue.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineRogue3RightUp = lineRogue.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineRogue4Left = lineRogue.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineRogue4LeftUp = lineRogue.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
    }
    private static void SetLineWarrior(Transform lines)
    {
        Transform lineWarrior = Utils.FindChild(lines, "$part_lines_up_left");
        LineCoreWarrior = lineWarrior.Find("$line_core_1/LineFill").GetComponent<Image>();
        LineCoreWarrior2 = lineWarrior.Find("$line_core_2/LineFill").GetComponent<Image>();
        LineCoreWarrior3 = lineWarrior.Find("$line_core_3/LineFill").GetComponent<Image>();
        LineCoreWarrior4 = lineWarrior.Find("$line_core_4/LineFill").GetComponent<Image>();
        
        LineWarrior1Right = lineWarrior.Find("$line_1/$part_right/LineFill").GetComponent<Image>();
        LineWarrior1Left = lineWarrior.Find("$line_1/$part_left/LineFill").GetComponent<Image>();
        LineWarrior2Right = lineWarrior.Find("$line_2/$part_right/LineFill").GetComponent<Image>();
        LineWarrior2Left = lineWarrior.Find("$line_2/$part_left/LineFill").GetComponent<Image>();
        LineWarrior3Right = lineWarrior.Find("$line_3/$part_right/LineFill").GetComponent<Image>();
        LineWarrior3RightUp = lineWarrior.Find("$line_3/$part_right_up/LineFill").GetComponent<Image>();
        LineWarrior4Left = lineWarrior.Find("$line_4/$part_left/LineFill").GetComponent<Image>();
        LineWarrior4LeftUp = lineWarrior.Find("$line_4/$part_left_up/LineFill").GetComponent<Image>();
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
    }
    #endregion
    private static void RegisterButtons()
    {
        TalentButton.m_allButtons.Clear();
        Transform talents = Utils.FindChild(SkillTree.m_instance.transform, "$part_talents");
        foreach (Button button in talents.GetComponentsInChildren<Button>())
        {
            button.gameObject.AddComponent<TalentButton>().Init();
        }
    }
    private static void SetAllButtonEvents()
    {
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
        Transform? CoreCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_core_characteristics");
        Transform? CoreTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_core_talents");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_1", new Dictionary<string, Image>
        {
            {"$button_center", LineCoreUp}, 
            {"$button_bard_talent_1", LineUp1Left}, 
            {"$button_warrior_talent_1", LineUp1Right}
        }, "Core1");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_2", new Dictionary<string, Image> {{"$button_core_1", LineCoreUp2}},  "Core2");

        TalentButton.SetButton(CoreCharacteristics, "$button_core_3", new Dictionary<string, Image> {{"$button_core_2", LineUp2Right}},  "Core3");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_4", new Dictionary<string, Image> {{"$button_core_2", LineUp2Left}},  "Core4");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_5", new Dictionary<string, Image>
        {
            {"$button_lumberjack", LineUp3Right}, {"$button_warrior_6", LineRadial8}
        },  "Core5");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_6", new Dictionary<string, Image> {{"$button_lumberjack", LineUp4Left}, {"$button_bard_5", LineRadial1}},  "Core6");
        TalentButton.SetButton(CoreTalents, "$button_lumberjack", new Dictionary<string, Image> {{"$button_core_2", LineCoreUp3},{"$button_core_6", LineUp4Left}, {"$button_core_5", LineUp3Right}},  "AirBender");
        TalentButton.SetButton(CoreTalents, "$button_chef", new Dictionary<string, Image> {{"$button_lumberjack", LineCoreUp4}},  "MasterChef");
        TalentButton.SetButton(CoreTalents, "$button_comfort_1", new Dictionary<string, Image> {{"$button_core_5", LineUp3RightUp}},  "Resourceful");
        TalentButton.SetButton(CoreTalents, "$button_comfort_2", new Dictionary<string, Image> {{"$button_core_6", LineUp4LeftUp}},  "Comfort");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_7", new Dictionary<string, Image> {{"$button_center", LineCoreDown}, {"$button_sneak", LineDown1Right}, {"$button_merchant", LineDown1Left}},  "Core7");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_8", new Dictionary<string, Image> {{"$button_core_7", LineCoreDown2}},  "Core8");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_9", new Dictionary<string, Image> {{"$button_core_8", LineDown2Right}},  "Core9");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_10", new Dictionary<string, Image> {{"$button_core_8", LineDown2Left}},  "Core10");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_11", new Dictionary<string, Image> {{"$button_treasure", LineDown3Right}, {"$button_sage_6", LineRadial4}},  "Core11");
        TalentButton.SetButton(CoreCharacteristics, "$button_core_12", new Dictionary<string, Image> {{"$button_treasure", LineDown4Left}, {"$button_ranger_5", LineRadial5}},  "Core12");
        TalentButton.SetButton(CoreTalents, "$button_treasure", new Dictionary<string, Image> {{"$button_core_8", LineCoreDown3}, {"$button_core_11", LineDown3Right}, {"$button_core_12", LineDown4Left}},  "Forager");
        TalentButton.SetButton(CoreTalents, "$button_sneak", new Dictionary<string, Image> {{"$button_core_7", LineDown1Right}, {"$button_sage_1", LineSage1Left}},  "Wise");
        TalentButton.SetButton(CoreTalents, "$button_merchant", new Dictionary<string, Image> {{"$button_core_7", LineDown1Left}, {"$button_ranger_1", LineRanger1Right}},  "DoubleLoot");
        TalentButton.SetButton(CoreTalents, "$button_shield", new Dictionary<string, Image> {{"$button_core_11", LineDown3RightDown}},  "PackMule");
        TalentButton.SetButton(CoreTalents, "$button_rain", new Dictionary<string, Image> {{"$button_core_12", LineDown4LeftDown}},  "RainProof");
        TalentButton.SetButton(CoreTalents, "$button_sail", new Dictionary<string, Image> {{"$button_treasure", LineCoreDown4}},  "Trader");

    }
    private static void SetBardButtonEvents()
    {
        Transform? BardTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_bard_talents");
        Transform? BardCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_bard_characteristics");

        TalentButton.SetButton(BardCharacteristics, "$button_bard_1", new(){{"$button_center", LineCoreBard}, {"$button_bard_talent_1", LineBard1Right}, {"$button_shaman_talent_1", LineBard1Left}},  "Bard1");
        TalentButton.SetButton(BardCharacteristics, "$button_bard_2", new(){{"$button_bard_1", LineCoreBard2}},  "Bard2");
        TalentButton.SetButton(BardCharacteristics, "$button_bard_3", new(){{"$button_bard_2", LineBard2Right}},  "Bard3");
        TalentButton.SetButton(BardCharacteristics, "$button_bard_4", new(){{"$button_bard_2", LineBard2Left}},  "Bard4");
        TalentButton.SetButton(BardCharacteristics, "$button_bard_5", new(){{"$button_bard_talent_2", LineBard3Right}, {"$button_core_6", LineRadial1}},  "Bard5");
        TalentButton.SetButton(BardCharacteristics, "$button_bard_6", new(){{"$button_bard_talent_2", LineBard4Left},{"$button_shaman_5", LineRadial2}},  "Bard6");
        TalentButton.SetButton(BardTalents, "$button_bard_talent_1", new(){{"$button_core_1", LineUp1Left}, {"$button_bard_1", LineBard1Right}},  "SongOfSpeed");
        TalentButton.SetButton(BardTalents, "$button_bard_talent_2", new(){{"$button_bard_2", LineCoreBard3}, {"$button_bard_5", LineBard3Right}, {"$button_bard_6", LineBard4Left}},  "SongOfVitality");
        TalentButton.SetButton(BardTalents, "$button_bard_talent_3", new(){{"$button_bard_5", LineBard3RightUp}},  "SongOfDamage");
        TalentButton.SetButton(BardTalents, "$button_bard_talent_4", new(){{"$button_bard_6", LineBard4LeftUp}},  "SongOfHealing");
        TalentButton.SetButton(BardTalents, "$button_bard_talent_5", new(){{"$button_bard_talent_2", LineCoreBard4}},  "SongOfAttrition");
    }
    private static void SetShamanButtonEvents()
    {
        Transform ShamanTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_shaman_talents");
        Transform ShamanCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_shaman_characteristics");
        TalentButton.SetButton(ShamanCharacteristics, "$button_shaman_1", new()
        {
            {"$button_center", LineCoreShaman}, {"$button_shaman_talent_1", LineShaman1Right}, {"$button_sage_talent_1", LineShaman1Left}
        },  "Shaman1");
        TalentButton. SetButton(ShamanCharacteristics, "$button_shaman_2", new(){{"$button_shaman_1", LineCoreShaman2}},  "Shaman2");
        TalentButton.SetButton(ShamanCharacteristics, "$button_shaman_3", new(){{"$button_shaman_2", LineShaman2Right}},  "Shaman3");
        TalentButton.SetButton(ShamanCharacteristics, "$button_shaman_4", new(){{"$button_shaman_2", LineShaman2Left}},  "Shaman4");
        TalentButton.SetButton(ShamanCharacteristics, "$button_shaman_5", new(){{"$button_shaman_talent_2", LineShaman3Right}, {"$button_bard_6", LineRadial2}},  "Shaman5");
        TalentButton.SetButton(ShamanCharacteristics, "$button_shaman_6", new(){{"$button_shaman_talent_2", LineShaman4Left}, {"$button_sage_5", LineRadial3}},  "Shaman6");
        TalentButton.SetButton(ShamanTalents, "$button_shaman_talent_1", new(){{"$button_bard_1", LineBard1Left}, {"$button_shaman_1", LineShaman1Right}},  "ShamanHeal");
        TalentButton.SetButton(ShamanTalents, "$button_shaman_talent_2", new(){{"$button_shaman_2", LineCoreShaman3}, {"$button_shaman_5", LineShaman3Right}, {"$button_shaman_6",LineShaman4Left}},  "RootBeam");
        TalentButton.SetButton(ShamanTalents, "$button_shaman_talent_3", new(){{"$button_shaman_5", LineShaman3RightUp}},  "ShamanSpawn");
        TalentButton.SetButton(ShamanTalents, "$button_shaman_talent_4", new(){{"$button_shaman_6", LineShaman4LeftUp}},  "ShamanRegeneration");
        TalentButton.SetButton(ShamanTalents, "$button_shaman_talent_5", new(){{"$button_shaman_talent_2", LineCoreShaman4}},  "ShamanShield");
    }
    private static void SetSageButtonEvents()
    {
        Transform SageTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_sage_talents");
        Transform SageCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_sage_characteristics");
        TalentButton.SetButton(SageCharacteristics, "$button_sage_1", new(){{"$button_center", LineCoreSage}, {"$button_sage_talent_1", LineSage1Right}, {"$button_sneak", LineSage1Left}},  "Sage1");
        TalentButton.SetButton(SageCharacteristics, "$button_sage_2", new(){{"$button_sage_1", LineCoreSage2}},  "Sage2");
        TalentButton.SetButton(SageCharacteristics, "$button_sage_3", new(){{"$button_sage_2", LineSage2Right}},  "Sage3");
        TalentButton.SetButton(SageCharacteristics, "$button_sage_4", new(){{"$button_sage_2", LineSage2Left}},  "Sage4");
        TalentButton.SetButton(SageCharacteristics, "$button_sage_5", new(){{"$button_sage_talent_4", LineSage3Right}, {"$button_shaman_6", LineRadial3}},  "Sage5");
        TalentButton.SetButton(SageCharacteristics, "$button_sage_6", new(){{"$button_sage_talent_4", LineSage4Left}, {"$button_core_11", LineRadial4}},  "Sage6");
        TalentButton.SetButton(SageTalents, "$button_sage_talent_1", new(){{"$button_shaman_1", LineShaman1Left}, {"$button_sage_1", LineSage1Right}},  "StoneThrow");
        TalentButton.SetButton(SageTalents, "$button_sage_talent_4", new(){{"$button_sage_2", LineCoreSage3}, {"$button_sage_6", LineSage4Left}, {"$button_sage_5", LineSage3Right}},  "CallOfLightning");
        TalentButton.SetButton(SageTalents, "$button_sage_talent_3", new(){{"$button_sage_6", LineSage4LeftUp}},  "MeteorStrike");
        TalentButton.SetButton(SageTalents, "$button_sage_talent_2", new(){{"$button_sage_5", LineSage3RightUp}},  "GoblinBeam");
        TalentButton.SetButton(SageTalents, "$button_sage_talent_5", new(){{"$button_sage_talent_4", LineCoreSage4}},  "IceBreath");
    }
    private static void SetRangerButtonEvents()
    {
        Transform RangerTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_ranger_talents");
        Transform RangerCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_ranger_characteristics");
        TalentButton.SetButton(RangerCharacteristics, "$button_ranger_1", new()
        {
            {"$button_center", LineCoreRanger},
            {"$button_ranger_talent_1", LineRanger1Left},
            {"$button_merchant", LineRanger1Right}
        },  "Ranger1");
        TalentButton.SetButton(RangerCharacteristics, "$button_ranger_2", new(){{"$button_ranger_1", LineCoreRanger2}},  "Ranger2");

        TalentButton.SetButton(RangerCharacteristics, "$button_ranger_3", new(){{"$button_ranger_2", LineRanger2Right}},  "Ranger3");
        TalentButton.SetButton(RangerCharacteristics, "$button_ranger_4", new(){{"$button_ranger_2", LineRanger2Left}},  "Ranger4");
        TalentButton.SetButton(RangerCharacteristics, "$button_ranger_5", new()
        {
            {"$button_ranger_talent_2", LineRanger3Right},
            {"$button_core_12", LineRadial5}
        },  "Ranger5");
        TalentButton.SetButton(RangerCharacteristics, "$button_ranger_6", new()
        {
            {"$button_ranger_talent_2", LineRanger4Left},
            {"$button_rogue_5", LineRadial6}
        },  "Ranger6");
        TalentButton.SetButton(RangerTalents, "$button_ranger_talent_1", new()
        {
            {"$button_rogue_1", LineRogue1Right},
            {"$button_ranger_1", LineRanger1Left}
        },  "RangerHunter");
        TalentButton.SetButton(RangerTalents, "$button_ranger_talent_2", new()
        {
            {"$button_ranger_2", LineCoreRanger3},
            {"$button_ranger_5", LineRanger3Right},
            {"$button_ranger_6",LineRanger4Left}
        },  "LuckyShot");
        TalentButton.SetButton(RangerTalents, "$button_ranger_talent_3", new(){{"$button_ranger_5", LineRanger3RightUp}},  "RangerTamer");
        TalentButton.SetButton(RangerTalents, "$button_ranger_talent_4", new(){{"$button_ranger_6", LineRanger4LeftUp}},  "RangerTrap");
        TalentButton.SetButton(RangerTalents, "$button_ranger_talent_5", new(){{"$button_ranger_talent_2", LineCoreRanger4}},  "QuickShot");
    }
    private static void SetRogueButtonEvents()
    {
        Transform RogueTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_rogue_talents");
        Transform RogueCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_rogue_characteristics");
        TalentButton.SetButton(RogueCharacteristics, "$button_rogue_1", new()
        {
            {"$button_center", LineCoreRogue},
            {"$button_ranger_talent_1", LineRogue1Right},
            {"$button_rogue_talent_1", LineRogue1Left}
        },  "Rogue1");
        TalentButton.SetButton(RogueCharacteristics, "$button_rogue_2", new(){{"$button_rogue_1", LineCoreRogue2}},  "Rogue2");
        TalentButton.SetButton(RogueCharacteristics, "$button_rogue_3", new(){{"$button_rogue_2", LineRogue2Right}},  "Rogue3");
        TalentButton.SetButton(RogueCharacteristics, "$button_rogue_4", new(){{"$button_rogue_2", LineRogue2Left}},  "Rogue4");
        TalentButton.SetButton(RogueCharacteristics, "$button_rogue_5", new()
        {
            {"$button_rogue_talent_2", LineRogue3Right},
            {"$button_ranger_6", LineRadial6}
        },  "Rogue5");
        TalentButton.SetButton(RogueCharacteristics, "$button_rogue_6", new()
        {
            {"$button_rogue_talent_2", LineRogue4Left},
            {"$button_warrior_5", LineRadial7}
        },  "Rogue6");
        TalentButton.SetButton(RogueTalents, "$button_rogue_talent_1", new()
        {
            {"$button_rogue_1", LineRogue1Left},
            {"$button_warrior_1", LineWarrior1Right}
        },  "RogueSpeed");
        TalentButton.SetButton(RogueTalents, "$button_rogue_talent_2", new()
        {
            {"$button_rogue_2", LineCoreRogue3},
            {"$button_rogue_5", LineRogue3Right},
            {"$button_rogue_6",LineRogue4Left}
        },  "RogueReflect");
        TalentButton.SetButton(RogueTalents, "$button_rogue_talent_3", new(){{"$button_rogue_5", LineRogue3RightUp}},  "RogueBackstab");
        TalentButton.SetButton(RogueTalents, "$button_rogue_talent_4", new(){{"$button_rogue_6", LineRogue4LeftUp}},  "RogueStamina");
        TalentButton.SetButton(RogueTalents, "$button_rogue_talent_5", new(){{"$button_rogue_talent_2", LineCoreRogue4}},  "RogueBleed");
    }
    private static void SetWarriorButtonEvents()
    {
        Transform WarriorTalents = Utils.FindChild(SkillTree.m_instance.transform, "$part_warrior_talents");
        Transform WarriorCharacteristics = Utils.FindChild(SkillTree.m_instance.transform, "$part_warrior_characteristics");
        TalentButton.SetButton(WarriorCharacteristics, "$button_warrior_1", new()
        {
            {"$button_center", LineCoreWarrior},
            {"$button_warrior_talent_1", LineWarrior1Left},
            {"$button_rogue_talent_1", LineWarrior1Right}
        },  "Warrior1");
        TalentButton.SetButton(WarriorCharacteristics, "$button_warrior_2", new(){{"$button_warrior_1", LineCoreWarrior2}},  "Warrior2");
        TalentButton.SetButton(WarriorCharacteristics, "$button_warrior_3", new(){{"$button_warrior_2", LineWarrior2Right}},  "Warrior3");
        TalentButton.SetButton(WarriorCharacteristics, "$button_warrior_4", new(){{"$button_warrior_2", LineWarrior2Left}},  "Warrior4");
        TalentButton.SetButton(WarriorCharacteristics, "$button_warrior_5", new()
        {
            {"$button_warrior_talent_2", LineWarrior3Right},
            {"$button_rogue_6", LineRadial7}
        },  "Warrior5");
        TalentButton.SetButton(WarriorCharacteristics, "$button_warrior_6", new()
        {
            {"$button_warrior_talent_2", LineWarrior4Left},
            {"$button_core_5", LineRadial8}
        },  "Warrior6");
        TalentButton.SetButton(WarriorTalents, "$button_warrior_talent_1", new()
        {
            {"$button_warrior_1", LineWarrior1Left},
            {"$button_core_1", LineUp1Right}
        },  "WarriorStrength");
        TalentButton.SetButton(WarriorTalents, "$button_warrior_talent_2", new()
        {
            {"$button_warrior_2", LineCoreWarrior3},
            {"$button_warrior_5", LineWarrior3Right},
            {"$button_warrior_6",LineWarrior4Left}
        },  "WarriorVitality");
        TalentButton.SetButton(WarriorTalents, "$button_warrior_talent_2", new()
        {
            {"$button_warrior_2", LineCoreWarrior3},
            {"$button_warrior_5", LineWarrior3Right},
            {"$button_warrior_6",LineWarrior4Left}
        },  "WarriorVitality");
        TalentButton.SetButton(WarriorTalents, "$button_warrior_talent_3", new(){{"$button_warrior_5", LineWarrior3RightUp}},  "WarriorResistance");
        TalentButton.SetButton(WarriorTalents, "$button_warrior_talent_4", new(){{"$button_warrior_6", LineWarrior4LeftUp}},  "MonkeyWrench");
        TalentButton.SetButton(WarriorTalents, "$button_warrior_talent_5", new(){{"$button_warrior_talent_2", LineCoreWarrior4}},  "DualWield");

    }
    public static void ChangeButton(Talent talent, bool revert = false)
    {
        if (!TalentButton.m_allButtons.TryGetValue(talent.m_button, out TalentButton button)) return;
        if (!TalentButton.m_buttonOriginalSpriteMap.TryGetValue(button, out Sprite originalSprite)) return;
        if (!TalentManager.m_talentsByButton.TryGetValue(talent.m_button, out Talent original)) return;
        if (!TalentManager.m_altTalentsByButton.TryGetValue(talent.m_button, out Talent alt)) return;
        if (talent.m_type is TalentType.Passive && talent.m_status is { } status)
        {
            Player.m_localPlayer.GetSEMan().RemoveStatusEffect(status.NameHash());
        }
        if (!revert)
        {
            TalentButton.RemapButton(talent.m_button, FillLines.m_fillLineMap[button], alt.m_key);

            if (talent.m_altButtonSprite != null)
            {
                button.SetButtonIcons(talent.m_altButtonSprite);
            }
            if (!PlayerManager.m_playerTalents.ContainsKey(original.m_key)) return;
            PlayerManager.m_playerTalents.Remove(original.m_key);
            PlayerManager.m_tempPlayerData.m_boughtTalents[alt.m_key] = PlayerManager.m_tempPlayerData.m_boughtTalents[original.m_key];
            PlayerManager.m_tempPlayerData.m_boughtTalents.Remove(original.m_key);
            PlayerManager.m_playerTalents[alt.m_key] = alt;
            SpellBook.Remove(original);
            SpellInventory.m_instance.Remove(original);
            switch (talent.m_type)
            {
                case TalentType.Ability or TalentType.StatusEffect:
                    SpellInventory.m_instance.Add(alt, SpellBook.Add(alt), true);
                    break;
                case TalentType.Passive:
                    if (talent.m_status is { } passiveStatus) Player.m_localPlayer.GetSEMan().AddStatusEffect(passiveStatus.NameHash());
                    break;
            }
        }
        else
        {
            TalentButton.RemapButton(talent.m_button, FillLines.m_fillLineMap[button], original.m_key);
            button.SetButtonIcons(originalSprite);
            if (!PlayerManager.m_playerTalents.ContainsKey(alt.m_key)) return;
            PlayerManager.m_playerTalents.Remove(alt.m_key);
            PlayerManager.m_tempPlayerData.m_boughtTalents[original.m_key] = PlayerManager.m_tempPlayerData.m_boughtTalents[alt.m_key];
            PlayerManager.m_tempPlayerData.m_boughtTalents.Remove(alt.m_key);
            PlayerManager.m_playerTalents[original.m_key] = original;
            SpellBook.Remove(alt);
            SpellInventory.m_instance.Remove(alt);
            switch (original.m_type)
            {
                case TalentType.Ability or TalentType.StatusEffect:
                    SpellInventory.m_instance.Add(original, SpellBook.Add(original), true);
                    break;
                case TalentType.Passive:
                    if (talent.m_status is { } passiveStatus)
                        Player.m_localPlayer.GetSEMan().AddStatusEffect(passiveStatus.NameHash());
                    break;
            }
        }
    }
}