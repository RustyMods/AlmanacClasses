using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Data;
using HarmonyLib;
using PieceManager;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AlmanacClasses.UI;

public static class SpellBook
{
    private static GameObject m_spellBar = null!;
    private static GameObject m_element = null!;
    public static readonly List<AbilityData> m_abilities = new();

    private static RectTransform m_spellBarPos = null!;
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");

    public static void OnSpellBarPosChange(object sender, EventArgs e)
    {
        m_spellBarPos.anchoredPosition = AlmanacClassesPlugin._SpellBookPos.Value;
        LoadUI.MenuInfoPanel.transform.position = AlmanacClassesPlugin._SpellBookPos.Value + new Vector2(0f, 150f);
    }

    public static bool IsAbilityInBook(Talent ability) => m_abilities.Any(talent => ability == talent.m_data);
    
    public static void LoadElements()
    {
        if (!Hud.instance) return;
        Font? NorseBold = LoadUI.GetFont("Norsebold");
        
        m_spellBar = Object.Instantiate(AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_UI"), Hud.instance.transform, false);
        m_spellBarPos = m_spellBar.GetComponent<RectTransform>();
        m_spellBarPos.anchoredPosition = AlmanacClassesPlugin._SpellBookPos.Value;
        m_spellBar.AddComponent<SpellBarMove>();
        m_element = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("SpellBar_element");
        Image? grayMat = Utils.FindChild(m_element.transform, "$image_gray").GetComponent<Image>();
        try
        {
            grayMat.material.shader = MaterialReplacer.FindShaderWithName(grayMat.material.shader, "Custom/icon");
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to find Custom/icon shader");
        }
        grayMat.material.SetFloat(Saturation, -0.5f);
        m_element.AddComponent<SpellElementChange>();
        
        Text[] texts = m_element.GetComponentsInChildren<Text>();
        LoadUI.AddFonts(texts, NorseBold);
    }
    public static void UpdateAbilities()
    {
        if (!Player.m_localPlayer || Player.m_localPlayer.IsDead() || Minimap.IsOpen()) return;
        DestroyElements();
        for (int index = 0; index < m_abilities.Count; index++)
        {
            AbilityData ability = m_abilities[index];
            GameObject element = Object.Instantiate(m_element, m_spellBar.transform.Find("$part_content"));
            
            if (element.TryGetComponent(out SpellElementChange component))
            {
                component.data = ability;
                component.index = index;
            }
            
            ability.m_go = element;
            Utils.FindChild(element.transform, "$image_icon").GetComponent<Image>().sprite = ability.m_data.m_sprite;
            Image? gray = Utils.FindChild(element.transform, "$image_gray").GetComponent<Image>();
            gray.sprite = ability.m_data.m_sprite;
            if (AbilityManager.m_cooldownMap.TryGetValue(ability.m_data.m_name, out float cooldown))
            {
                gray.fillAmount = cooldown;
                Utils.FindChild(element.transform, "$image_fill").GetComponent<Image>().fillAmount = cooldown;
                Utils.FindChild(element.transform, "$text_timer").GetComponent<Text>().text = ((int)((ability.m_data.m_ttl?.Value ?? 10f) * cooldown)).ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                gray.fillAmount = 0f;
                Utils.FindChild(element.transform, "$image_fill").GetComponent<Image>().fillAmount = 0f;
                Utils.FindChild(element.transform, "$text_timer").GetComponent<Text>().text = Localization.instance.Localize("$info_ready");
            }

            string keyCode = "";
            switch (index)
            {
                case 0:
                    keyCode = AlmanacClassesPlugin._Spell1.Value.ToString();
                    break;
                case 1:
                    keyCode = AlmanacClassesPlugin._Spell2.Value.ToString();
                    break;
                case 2:
                    keyCode = AlmanacClassesPlugin._Spell3.Value.ToString();
                    break;
                case 3:
                    keyCode = AlmanacClassesPlugin._Spell4.Value.ToString();
                    break;
                case 4:
                    keyCode = AlmanacClassesPlugin._Spell5.Value.ToString();
                    break;
                case 5:
                    keyCode = AlmanacClassesPlugin._Spell6.Value.ToString();
                    break;
                case 6:
                    keyCode = AlmanacClassesPlugin._Spell7.Value.ToString();
                    break;
                case 7:
                    keyCode = AlmanacClassesPlugin._Spell8.Value.ToString();
                    break;
            }

            Utils.FindChild(element.transform, "$text_hotkey").GetComponent<Text>().text = keyCode;
        }
    }

    public static void DestroyElements()
    {
        foreach (AbilityData? ability in m_abilities)
        {
            if (!ability.m_go) continue;
            Object.Destroy(ability.m_go);
        }
    }
    
    [HarmonyPatch(typeof(TextsDialog), nameof(TextsDialog.UpdateTextsList))]
    static class CompendiumAddActiveEffectsPatch
    {
        private static void Postfix(TextsDialog __instance)
        {
            if (!Player.m_localPlayer) return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (AbilityData talent in m_abilities)
            {
                Talent data = talent.m_data;
                stringBuilder.Append($"<color=orange>{data.m_name}</color>\n");
                stringBuilder.Append(LoadUI.GetTooltip(data));
                stringBuilder.Append("\n");
            }
            TextsDialog.TextInfo text = new TextsDialog.TextInfo("Spell Book", Localization.instance.Localize(stringBuilder.ToString()));
            if (GetPassives(out string output))
            {
                TextsDialog.TextInfo passive = new TextsDialog.TextInfo("Passive Abilities", output);
                __instance.m_texts.Insert(0, passive);
            }
            __instance.m_texts.Insert(0, text);
        }

        private static bool GetPassives(out string output)
        {
            output = "";
            int count = 0;
            StringBuilder stringBuilder = new StringBuilder();
            StatusEffect status = Player.m_localPlayer.GetSEMan().GetStatusEffect("SE_Characteristic".GetStableHashCode());
            if (status)
            {
                stringBuilder.Append(status.GetTooltipString());
                stringBuilder.Append("\n");
                ++count;
            }
            foreach (KeyValuePair<string, Talent> kvp in PlayerManager.m_playerTalents)
            {
                if (kvp.Value.m_type is not TalentType.Passive) continue;
                Talent data = kvp.Value;
                stringBuilder.Append($"<color=orange>{data.m_name}</color>\n");
                stringBuilder.Append(LoadUI.GetTooltip(kvp.Value));
                stringBuilder.Append("\n");
                ++count;
            }
            
            if (count == 0) return false;
            output = Localization.instance.Localize(stringBuilder.ToString());
            return true;
        }
    }
}

public class AbilityData
{
    public Talent m_data = null!;
    public GameObject m_go = null!;
}