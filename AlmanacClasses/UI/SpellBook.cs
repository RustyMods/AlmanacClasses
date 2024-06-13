using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AlmanacClasses.UI;

public static class SpellBook
{
    private static GameObject m_spellBar = null!;
    private static GameObject m_element = null!;
    public static Dictionary<int, AbilityData> m_abilities = new();
    private static RectTransform m_spellBarPos = null!;
    private static Text[]? m_texts;
    public static void OnSpellBarPosChange(object sender, EventArgs e)
    {
        m_spellBarPos.anchoredPosition = AlmanacClassesPlugin._SpellBookPos.Value;
        LoadUI.MenuInfoPanel.transform.position = AlmanacClassesPlugin._SpellBookPos.Value + new Vector2(0f, 150f);
    }

    public static void OnLogout()
    {
        ClearSpellBook();
    }

    private static void ClearSpellBook() => m_abilities.Clear();
    public static bool IsAbilityInBook(Talent ability) => m_abilities.Any(talent => ability == talent.Value.m_data);
    public static bool RemoveAbility(Talent ability)
    {
        if (!IsAbilityInBook(ability)) return false;
        KeyValuePair<int, AbilityData> kvp = m_abilities.FirstOrDefault(kvp => kvp.Value.m_data == ability);
        DestroyElements();
        if (!m_abilities.Remove(kvp.Key)) return false;
        NormalizeBook();
        return true;
    }

    private static void NormalizeBook()
    {
        Dictionary<int, AbilityData> newAbilities = new Dictionary<int, AbilityData>();
        int newKey = 0;
        foreach (KeyValuePair<int, AbilityData> item in m_abilities)
        {
            newAbilities[newKey] = item.Value;
            ++newKey;
        }
        m_abilities = newAbilities;
    }
    public static void LoadElements(Font? font)
    {
        if (!Hud.instance) return;
        m_spellBar = Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_UI"), Hud.instance.transform, false);
        m_spellBarPos = m_spellBar.GetComponent<RectTransform>();
        m_spellBarPos.anchoredPosition = AlmanacClassesPlugin._SpellBookPos.Value;
        m_spellBar.AddComponent<SpellBarMove>();
        m_element = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_element");
        m_element.AddComponent<SpellElementChange>();
        
        m_texts = m_element.GetComponentsInChildren<Text>();
        LoadUI.AddFonts(m_texts, font);
        UpdateFontSize();
    }

    private static void UpdateFontSize()
    {
        if (m_texts == null) return;
        foreach (Text component in m_texts)
        {
            component.fontSize = 14;
            component.resizeTextMinSize = 10;
            component.resizeTextForBestFit = true;
        };
    }
    public static void UpdateAbilities()
    {
        if (!Player.m_localPlayer || Player.m_localPlayer.IsDead() || Minimap.IsOpen()) return;
        DestroyElements();

        foreach (KeyValuePair<int, AbilityData> kvp in m_abilities)
        {
            GameObject element = Object.Instantiate(m_element, m_spellBar.transform.Find("$part_content"));
            if (element.TryGetComponent(out SpellElementChange component))
            {
                component.data = kvp.Value;
                component.index = kvp.Key;
            }
            
            kvp.Value.m_go = element;
            Sprite? icon = kvp.Value.m_data.GetSprite();
            Utils.FindChild(element.transform, "$image_icon").GetComponent<Image>().sprite = icon;
            Image? gray = Utils.FindChild(element.transform, "$image_gray").GetComponent<Image>();
            Image? fill = Utils.FindChild(element.transform, "$image_fill").GetComponent<Image>();
            Text timer = Utils.FindChild(element.transform, "$text_timer").GetComponent<Text>();

            if (AbilityManager.m_cooldownMap.TryGetValue(kvp.Value.m_data.m_key, out float ratio))
            {
                if (kvp.Value.m_data.m_statusEffectHash != 0)
                {
                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(kvp.Value.m_data.m_statusEffectHash))
                    {
                        StatusEffect effect = Player.m_localPlayer.GetSEMan().GetStatusEffect(kvp.Value.m_data.m_statusEffectHash);
                        float time = effect.GetRemaningTime();
                        float normal = Mathf.Clamp01(time / effect.m_ttl);
                        gray.fillAmount = time > 0 ? normal : 0f;
                    }
                }
                
                fill.fillAmount = ratio;
                timer.text = GetCooldownColored((int)(kvp.Value.m_data.GetCooldown(kvp.Value.m_data.GetLevel()) * ratio));
            }
            else
            {
                gray.fillAmount = 0f;
                fill.fillAmount = 0f;
                timer.text = Localization.instance.Localize("$info_ready");
            }
            Utils.FindChild(element.transform, "$text_hotkey").GetComponent<Text>().text = GetKeyCode(kvp.Key);
        }
    }

    private static string GetKeyCode(int index)
    {
        return AddAltKey(RemoveAlpha(index switch
        {
            0 => AlmanacClassesPlugin._Spell1.Value.ToString(),
            1 => AlmanacClassesPlugin._Spell2.Value.ToString(),
            2 => AlmanacClassesPlugin._Spell3.Value.ToString(),
            3 => AlmanacClassesPlugin._Spell4.Value.ToString(),
            4 => AlmanacClassesPlugin._Spell5.Value.ToString(),
            5 => AlmanacClassesPlugin._Spell6.Value.ToString(),
            6 => AlmanacClassesPlugin._Spell7.Value.ToString(),
            7 => AlmanacClassesPlugin._Spell8.Value.ToString(),
            _ => ""
        }));
    }

    private static string AddAltKey(string input) => AlmanacClassesPlugin._SpellAlt.Value is KeyCode.None ? input : $"{AlmanacClassesPlugin._SpellAlt.Value} + {input}";
    private static string RemoveAlpha(string input) => input.Replace("Alpha", string.Empty);
    private static string GetCooldownColored(int time)
    {
        return time switch
        {
            > 60 => $"<color=#FF5349>{time}</color>",
            > 30 => $"<color=#FFAA33>{time}</color>",
            > 10 => $"<color=#FFAA33>{time}</color>",
            _ => time.ToString()
        };
    }
    public static void DestroyElements()
    {
        foreach (AbilityData? ability in m_abilities.Values.Where(ability => ability.m_go)) Object.Destroy(ability.m_go);
    }
    
    [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.UpdateTextsList))]
    static class CompendiumAddActiveEffectsPatch
    {
        private static void Postfix(TextsDialog __instance)
        {
            if (!Player.m_localPlayer) return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (AbilityData talent in m_abilities.Values)
            {
                stringBuilder.Append($"<color=orange>{talent.m_data.GetName()}</color>\n");
                stringBuilder.Append(talent.m_data.GetTooltip());
                stringBuilder.Append("\n");
            }
            TextsDialog.TextInfo text = new TextsDialog.TextInfo("$title_spell_book", Localization.instance.Localize(stringBuilder.ToString()));
            TextsDialog.TextInfo passive = new TextsDialog.TextInfo("$title_passive_abilities", GetPassives());
            __instance.m_texts.Insert(0, passive);
            __instance.m_texts.Insert(0, text);
        }

        private static string GetPassives()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(CharacteristicManager.GetTooltip() + "\n");
            foreach (KeyValuePair<string, Talent> kvp in PlayerManager.m_playerTalents)
            {
                if (kvp.Value.m_type is not TalentType.Passive) continue;
                Talent data = kvp.Value;
                stringBuilder.Append($"<color=orange>{data.GetName()}</color>\n");
                stringBuilder.Append(data.GetTooltip());
                stringBuilder.Append("\n");
            }
            
            return Localization.instance.Localize(stringBuilder.ToString());
        }
    }
}

public class AbilityData
{
    public Talent m_data = null!;
    public GameObject m_go = null!;
}