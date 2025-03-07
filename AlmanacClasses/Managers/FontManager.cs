using System;
using System.Collections.Generic;
using System.Linq;
using AlmanacClasses.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.Managers;

public static class FontManager
{
    public enum FontOptions
    {
        Norse, 
        NorseBold, 
        AveriaSerifLibre,
        AveriaSerifLibreBold,
        AveriaSerifLibreLight,
        LegacyRuntime
    }
    
    private static readonly Dictionary<FontOptions, Font?> m_fonts = new();
    private static readonly List<TalentText> m_allTexts = new();
    
    private static string GetFontName(FontOptions option) => option switch
    {
        FontOptions.Norse => "Norse",
        FontOptions.AveriaSerifLibre => "AveriaSerifLibre-Regular",
        FontOptions.AveriaSerifLibreBold => "AveriaSerifLibre-Bold",
        FontOptions.AveriaSerifLibreLight => "AveriaSerifLibre-Light",
        FontOptions.NorseBold => "Norsebold",
        FontOptions.LegacyRuntime => "LegacyRuntime",
        _ => "AveriaSerifLibre-Regular"
    };

    private static Font? GetFont(FontOptions option)
    {
        if (m_fonts.TryGetValue(option, out Font? font)) return font;
        Font[]? fonts = Resources.FindObjectsOfTypeAll<Font>();
        var match = fonts.FirstOrDefault(x => x.name == GetFontName(option));
        m_fonts[option] = match;
        return match;
    }

    public static void OnFontChange(object sender, EventArgs args)
    {
        var font = GetFont(AlmanacClassesPlugin._Font.Value);
        foreach (var text in m_allTexts) text.Update(font);
        SpellElement.UpdateFont(font);
        PassiveButton.OnFontChange(font);
        InventoryButton.OnFontChange(font);
    }

    public static void SetFont(Text[] array)
    {
        foreach (Text text in array)
        {
            var _ = new TalentText(text, GetFont(AlmanacClassesPlugin._Font.Value));
        }
    }

    private class TalentText
    {
        private readonly Text m_text;

        public TalentText(Text text, Font? font)
        {
            m_text = text;
            Update(font);
            m_allTexts.Add(this);
        }

        public void Update(Font? font) => m_text.font = font;
    }
}