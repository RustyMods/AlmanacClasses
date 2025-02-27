using System.Collections.Generic;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class FillLines
{
    public static readonly Dictionary<Image, FillLines> m_allLines = new();
    public static readonly Dictionary<TalentButton, Dictionary<string, Image>> m_fillLineMap = new();
    private readonly Image m_image;
    private readonly List<TalentButton> m_buttons = new List<TalentButton>();

    public FillLines(Image image)
    {
        m_image = image;
        m_allLines[image] = this;
    }

    public void AddButton(string buttonKey)
    {
        if (buttonKey == "$button_center") return;
        if (!TalentButton.m_allButtons.TryGetValue(buttonKey, out TalentButton button)) return;
        if (m_buttons.Contains(button)) return;
        m_buttons.Add(button);
        button.m_fillLine = this;
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

    public static void UpdateFillLines()
    {
        foreach (var fillLine in m_allLines.Values)
        {
            if (fillLine.AreChecked()) fillLine.SetFill(1f);
        }
    }
}