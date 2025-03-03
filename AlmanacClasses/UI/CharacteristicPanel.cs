using System.Collections.Generic;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class CharacteristicPanel : MonoBehaviour
{
    public static CharacteristicPanel m_instance = null!;
    public Text m_title = null!;
    public CharacteristicButtons m_constitution = null!;
    public CharacteristicButtons m_intelligence = null!;
    public CharacteristicButtons m_dexterity = null!;
    public CharacteristicButtons m_strength = null!;
    public CharacteristicButtons m_wisdom = null!;
    public List<CharacteristicButtons> m_components = new();
    public void Init()
    {
        m_instance = this;
        m_title = transform.Find("$text_title").GetComponent<Text>();
        m_constitution = transform.Find("$part_content/$part_constitution").gameObject.AddComponent<CharacteristicButtons>();
        m_intelligence = transform.Find("$part_content/$part_intelligence").gameObject.AddComponent<CharacteristicButtons>();
        m_dexterity = transform.Find("$part_content/$part_dexterity").gameObject.AddComponent<CharacteristicButtons>();
        m_strength = transform.Find("$part_content/$part_strength").gameObject.AddComponent<CharacteristicButtons>();
        m_wisdom = transform.Find("$part_content/$part_wisdom").gameObject.AddComponent<CharacteristicButtons>();
        m_components = new() { m_constitution, m_intelligence, m_dexterity, m_strength, m_wisdom };
        m_constitution.m_type = Characteristic.Constitution;
        m_intelligence.m_type = Characteristic.Intelligence;
        m_dexterity.m_type = Characteristic.Dexterity;
        m_strength.m_type = Characteristic.Strength;
        m_wisdom.m_type = Characteristic.Wisdom;
        foreach(var component in m_components) component.Init();
        m_constitution.SetTitle("$almanac_constitution");
        m_intelligence.SetTitle("$almanac_intelligence");
        m_dexterity.SetTitle("$almanac_dexterity");
        m_strength.SetTitle("$almanac_strength");
        m_wisdom.SetTitle("$almanac_wisdom");
    }

    public void SetTitle(string text) => m_title.text = Localization.m_instance.Localize(text);
    public void SetTooltip(string text) =>
        SkillTree.m_instance.StatsTooltip.text = Localization.m_instance.Localize(text);
    public void SetDefaultTooltip()
    {
        SkillTree.m_instance.StatsTooltip.text = Localization.m_instance.Localize(
            "$almanac_characteristic_desc \n" 
            + "[<color=orange>L.Alt</color>] $text_add_or_remove 5\n"
            + "[<color=orange>L.Ctr</color>] $text_add_or_remove 10");
    }

    public void UpdateTexts()
    {
        foreach(var component in m_components) component.UpdateAmount();
    }
}