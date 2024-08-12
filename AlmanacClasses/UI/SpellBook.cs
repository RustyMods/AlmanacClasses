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

    public static void ClearSpellBook()
    {
        m_abilities.Clear();
        UpdateAbilities();
    }

    public static bool IsAbilityInBook(Talent ability) => m_abilities.Values.Any(talent => ability == talent.m_data);
    
    public static bool RemoveAbility(Talent ability)
    {
        if (!IsAbilityInBook(ability)) return false;

        KeyValuePair<int, AbilityData> kvp = default;
        foreach (KeyValuePair<int, AbilityData> item in m_abilities)
        {
            if (item.Value.m_data != ability) continue;
            kvp = item;
            break;
        }

        if (!m_abilities.Remove(kvp.Key)) return false;
        NormalizeBook();
        UpdateAbilities();
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

    private static Dictionary<int, GameObject> GetExistingElements()
    {
        Dictionary<int, GameObject> existingElements = new Dictionary<int, GameObject>();
        foreach (Transform child in m_spellBar.transform.Find("$part_content"))
        {
            if (!child.TryGetComponent(out SpellElementChange component)) continue;
            existingElements[component.index] = child.gameObject;
        }

        return existingElements;
    }

    public static void UpdateAbilities() 
    {
        if (!Player.m_localPlayer || Player.m_localPlayer.IsDead() || Minimap.IsOpen()) return;

        Dictionary<int, GameObject> existingElements = GetExistingElements();

        foreach (KeyValuePair<int, AbilityData> kvp in m_abilities)
        {
            if (existingElements.TryGetValue(kvp.Key, out GameObject element))
            {
                if (!element.TryGetComponent(out SpellElementChange component)) continue;
                component.data = kvp.Value;
                component.index = kvp.Key;
            }
            else
            {
                element = Object.Instantiate(m_element, m_spellBar.transform.Find("$part_content"));
                if (!element.TryGetComponent(out SpellElementChange component)) continue;
                component.data = kvp.Value;
                component.index = kvp.Key;
            }
            
            Sprite? icon = kvp.Value.m_data.GetSprite();
            Image iconImage = Utils.FindChild(element.transform, "$image_icon").GetComponent<Image>();
            Text hotkeyText = Utils.FindChild(element.transform, "$text_hotkey").GetComponent<Text>();
            kvp.Value.m_go = element;
            iconImage.sprite = icon;
            UpdateCooldownDisplay(kvp.Value);
            hotkeyText.text = GetKeyCode(kvp.Key);
        }

        foreach (int key in existingElements.Keys)
        {
            if (m_abilities.ContainsKey(key)) continue;
            Object.Destroy(existingElements[key]);
        }
    }

    private static void UpdateCooldownDisplay(AbilityData abilityData)
    {
        if (abilityData.m_go == null || !Player.m_localPlayer) return;
        try
        {
            if (!Utils.FindChild(abilityData.m_go.transform, "$image_gray").TryGetComponent(out Image gray)) return;
            if (!Utils.FindChild(abilityData.m_go.transform, "$image_fill").TryGetComponent(out Image fill)) return;
            if (!Utils.FindChild(abilityData.m_go.transform, "$text_timer").TryGetComponent(out Text timer)) return;
            if (AbilityManager.m_cooldownMap.TryGetValue(abilityData.m_data.m_key, out float ratio))
            {
                if (abilityData.m_data.m_statusEffectHash == 0)
                {
                    gray.fillAmount = 0f;
                }
                else
                {
                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(abilityData.m_data.m_statusEffectHash))
                    {
                        StatusEffect? effect = Player.m_localPlayer.GetSEMan()
                            .GetStatusEffect(abilityData.m_data.m_statusEffectHash);
                        float time = effect.GetRemaningTime();
                        float normal = Mathf.Clamp01(time / effect.m_ttl);
                        gray.fillAmount = time > 0 ? normal : 0f;
                    }
                    else
                    {
                        gray.fillAmount = 0f;
                    }
                }

                fill.fillAmount = ratio;
                int cooldownTime = (int)(abilityData.m_data.GetCooldown(abilityData.m_data.GetLevel()) * ratio);
                timer.text = GetCooldownColored(cooldownTime);
                if (cooldownTime <= 0)
                {
                    timer.text = Localization.instance.Localize("$info_ready");
                }
            }
            else
            {
                gray.fillAmount = 0f;
                fill.fillAmount = 0f;
                timer.text = Localization.instance.Localize("$info_ready");
            }
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to update ability cooldown");
        }
    }

    public static void UpdateCooldownDisplayForAbility(Talent ability)
    {
        AbilityData? abilityData = m_abilities.Values.FirstOrDefault(data => data.m_data == ability);
        if (abilityData == null) return;
        UpdateCooldownDisplay(abilityData);
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

    [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.UpdateTextsList))]
    static class CompendiumAddActiveEffectsPatch
    {
        private static void Postfix(TextsDialog __instance)
        {
            if (!Player.m_localPlayer) return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var talent in m_abilities.Values)
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
            foreach (var kvp in PlayerManager.m_playerTalents)
            {
                if (kvp.Value.m_type is not TalentType.Passive) continue;
                var data = kvp.Value;
                if (!data.m_passiveActive) continue;
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