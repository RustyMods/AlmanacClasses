using System.Collections.Generic;
using AlmanacClasses.Classes;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class FillLines
{
    public static readonly Dictionary<Image, FillLines> m_allLines = new();
    public static readonly Dictionary<TalentButton, Dictionary<string, Image>> m_fillLineMap = new();
    private readonly Image m_image;
    private readonly List<TalentButton> m_buttons = new List<TalentButton>();
    private static bool m_initLineFillSet;
    public FillLines(Image image)
    {
        m_image = image;
        m_allLines[image] = this;
    }
    
    public static void SetInitialFillLines()
    {
        if (m_initLineFillSet) return;
        TalentButton.ClearAll();
        foreach (KeyValuePair<string, Talent> kvp in PlayerManager.m_playerTalents)
        {
            string buttonName = kvp.Value.m_button;
            if (!TalentButton.m_allButtons.TryGetValue(buttonName, out TalentButton talentButton)) continue;
            talentButton.SetCheckmark(true);
        }

        Update();
        m_initLineFillSet = true;
    }
    
    public static void OnLogout() => m_initLineFillSet = false;
    public void AddButton(string buttonKey)
    {
        if (buttonKey == "$button_center") return;
        if (!TalentButton.m_allButtons.TryGetValue(buttonKey, out TalentButton button)) return;
        if (m_buttons.Contains(button)) return;
        m_buttons.Add(button);
    }

    public bool AreChecked() => m_buttons.TrueForAll(button => button.IsChecked());

    public void SetFill(float amount) => m_image.fillAmount = amount;

    public static void Reset()
    {
        foreach (var line in m_allLines.Values)
        {
            line.SetFill(0f);
        }
    }

    public static void Update()
    {
        foreach (var fillLine in m_allLines.Values)
        {
            if (fillLine.AreChecked()) fillLine.SetFill(1f);
        }
    }
}