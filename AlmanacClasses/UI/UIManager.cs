using UnityEngine;

namespace AlmanacClasses.UI;

/// <summary>
/// Helper class to manage the state of modded UI elements, such as showing and hiding.
/// </summary>
public class UIManager : MonoBehaviour
{
    private bool m_isInitialized;
    
    public SpellBook? SpellBook;
    public PassiveBar? PassiveBar;
    public ExperienceBar? ExperienceBar;
    public SpellInfo? SpellInfoPanel;

    private void Awake()
    {
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("UIManager::Awake");
    }

    private void Update()
    {
        if (Player.m_localPlayer == null || Hud.m_instance == null)
            return;
        
        if (Player.m_localPlayer.InCutscene() || Hud.m_instance.m_userHidden)
            SetVisible(false);
        else
            SetVisible(true);
    }

    public void Initialize(SpellBook spellBook, PassiveBar passiveBar, ExperienceBar experienceBar, SpellInfo spellInfo)
    {
        if (m_isInitialized)
            return;
        
        SpellBook = spellBook;
        PassiveBar = passiveBar;
        ExperienceBar = experienceBar;
        SpellInfoPanel = spellInfo;
        
        m_isInitialized = true;
        
        AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("UIManager Initialized");
    }

    public bool IsVisible()
    {
        return SpellBook.IsVisible() && PassiveBar.IsVisible() && ExperienceBar.IsVisible();
    }

    public void SetVisible(bool shouldBeVisible)
    {
        if (!m_isInitialized || SpellBook == null || PassiveBar == null || ExperienceBar == null|| SpellInfoPanel == null)
            return;
        
        if (IsVisible() == shouldBeVisible)
            return;

        if (shouldBeVisible)
        {
            SpellBook.SetVisible(true);
            PassiveBar.SetVisible(true);
            ExperienceBar.SetVisible(true);
        }
        else
        {
            SpellBook.SetVisible(false);
            PassiveBar.SetVisible(false);
            ExperienceBar.SetVisible(false);
        }
    }
}