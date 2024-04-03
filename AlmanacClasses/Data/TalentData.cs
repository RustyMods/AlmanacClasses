using System;
using System.Collections.Generic;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.Data;

public class TalentDamages
{
    public ConfigEntry<float>? blunt;
    public ConfigEntry<float>? pierce;
    public ConfigEntry<float>? slash;
    public ConfigEntry<float>? chop;
    public ConfigEntry<float>? pickaxe;
    public ConfigEntry<float>? fire;
    public ConfigEntry<float>? frost;
    public ConfigEntry<float>? lightning;
    public ConfigEntry<float>? poison;
    public ConfigEntry<float>? spirit;
}
public class Talent
{
    public string m_key = null!;
    public string m_name = "";
    public string m_description = "";
    public int m_level = 1;
    public TalentType m_type = TalentType.None;
    public Characteristic m_characteristic = Characteristic.None;
    public Skills.SkillType m_skill = Skills.SkillType.None;
    public int m_characteristicValue = 0;
    public StatusEffectManager.Data m_statusEffect = new();

    public Dictionary<StatusEffectData.Modifier, ConfigEntry<float>> m_modifiers = new();
    public Dictionary<HitData.DamageType, ConfigEntry<HitData.DamageModifier>> m_resistances = new();
    public string m_animation = "";
    public float m_damageInterval = 1f;
    public float m_radius = 0f;
    public ConfigEntry<float>? m_chance;
    public StatusEffect.StatusAttribute m_attribute = StatusEffect.StatusAttribute.None;
    public ConfigEntry<float>? m_heal;

    public string m_ability = "";
    public ConfigEntry<int>? m_comfortAmount;
    public Sprite? m_sprite = null;
    public ConfigEntry<float>? m_ttl;
    public bool m_findCharacter = false;
    public int m_cost = 3;
    public string m_buttonName = "";
    public bool m_triggerNow = false;
    public bool m_isAbility = false;
    public ConfigEntry<float>? m_duration;

    public ConfigEntry<int>? m_eitrCost;
    public ConfigEntry<int>? m_staminaCost;
    public ConfigEntry<int>? m_healthCost;

    public TalentDamages? m_talentDamages;
    public StatusEffect? m_effect;

    public ConfigEntry<AlmanacClassesPlugin.Toggle>? m_triggerStartEffects;

    public void InitTalent()
    {
        if (LoadUI.ButtonMap.TryGetValue(m_buttonName, out Button button))
        {
            Transform icon = button.gameObject.transform.Find("icon");
            if (icon)
            {
                if (icon.TryGetComponent(out Image image))
                {
                    if (image.sprite)
                    {
                        if (!m_sprite)
                        {
                            m_sprite = image.sprite;
                        }
                        m_statusEffect.m_icon = image.sprite;
                    }
                }
            }
        }
        StatusEffect? effect = null;
        switch (m_type)
        {
            case TalentType.Finder:
                effect = TimedFinder.CreateCustomFinder(m_key, m_name, m_description, m_ttl?.Value ?? 25f, m_findCharacter);
                break;
            case TalentType.StatusEffect:
                m_statusEffect.talent = this;
                m_statusEffect.Init(out StatusEffect? talentEffect);
                effect = talentEffect;
                break;
            case TalentType.Passive:
                if (m_triggerNow)
                {
                    m_statusEffect.talent = this;
                    m_statusEffect.Init(out StatusEffect? passiveEffect);
                    effect = passiveEffect;
                }
                break;
        }
        
        m_effect = effect;
        TalentManager.AllTalents[m_key] = this;
    }
}

[Serializable]
public class PlayerData
{
    public int m_experience = 0;
    public HashSet<string> m_boughtTalents = new();
    public int m_prestige = 1;
    public int m_prestigePoints = 0;
}

public enum Characteristic
{
    None,
    Constitution,
    Intelligence,
    Strength,
    Dexterity,
    Wisdom,
}

public enum TalentType
{
    None,
    Characteristic,
    Ability,
    StatusEffect,
    Comfort,
    Finder,
    Prestige,
    Passive
}