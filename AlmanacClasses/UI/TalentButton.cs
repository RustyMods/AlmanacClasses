using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

/// <summary>
/// Attached to Buttons
/// Contains all the necessary methods to control the buttons
/// </summary>
[RequireComponent(typeof(Button))]
public class TalentButton : MonoBehaviour
{
    public static readonly List<Selectable> m_selectables = new();
    public static readonly List<TalentButton> m_checkedTalents = new();
    public static readonly Dictionary<string, TalentButton> m_allButtons = new();
    public static readonly Dictionary<TalentButton, Sprite> m_buttonOriginalSpriteMap = new();
    public static TalentButton m_centerButton = null!;
    
    public Button m_button = null!;
    private GameObject? m_checkmark;
    public Image m_iconBackground = null!;
    public Image? m_icon;
    public Image? m_checkmarkIcon;
    public void Init()
    {
        m_button = GetComponent<Button>();
        m_selectables.Add(m_button);
        if (Utils.FindChild(transform, "Checkmark") is { } checkmark)
        {
            m_checkmark = checkmark.gameObject;
            m_checkmarkIcon = m_checkmark.GetComponent<Image>();
        }

        Transform background = transform.Find("background");
        if (!background) background = transform.Find("Background");
        m_iconBackground = background.GetComponent<Image>();
        m_allButtons[name] = this;
        if (name == "$button_center") m_centerButton = this;
        if (transform.Find("icon") is { } icon && icon.TryGetComponent(out Image iconImage))
        {
            m_buttonOriginalSpriteMap[this] = iconImage.sprite;
            m_icon = iconImage;
        }
    }

    public void SetCheckmark(bool enable)
    {
        if (m_checkmark is not null) m_checkmark.SetActive(enable);
        if (enable) m_checkedTalents.Add(this);
        else m_checkedTalents.Remove(this);
    }

    public bool IsChecked() => m_checkedTalents.Contains(this);
    public static void ClearAll()
    {
        foreach (var button in m_allButtons.Values) button.SetCheckmark(false);
        m_checkedTalents.Add(m_centerButton);
    }

    public void Select(Talent ability)
    {
        if (!PlayerManager.m_playerTalents.ContainsKey(ability.m_key)) return;
        Prestige.SelectedTalent = ability;
        SetAllButtonColors(Color.white);
        SetButtonColor(new Color(1f, 0.5f, 0f, 1f));
    }

    public void SetButtonColor(Color color) => m_iconBackground.color = color;

    public void SetButtonIcons(Sprite sprite)
    {
        if (m_icon == null || m_checkmarkIcon == null) return;
        m_icon.sprite = sprite;
        m_checkmarkIcon.sprite = sprite;
    }

    public static void SetAllButtonColors(Color color)
    {
        foreach (var button in m_allButtons.Values)
        {
            button.m_iconBackground.color = color;
        }
    }

    public static bool CheckCost(int cost)
    {
        if (cost <= TalentManager.GetAvailableTalentPoints()) return true;
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_not_enough_tp");
        return false;
    }
    
    public static bool IsEndAbility(string name)
    {
        if (!LoadUI.EndTalents.ContainsKey(name)) return true;
        if (CanBuyEndAbility(name)) return true;
        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_need_connected_talents");
        return false;
    }

    public static bool IsConnected(Dictionary<string, Image> lines, bool msg = true)
    {
        foreach (var buttonName in lines.Keys)
        {
            if (buttonName == "$button_center") return true;
            if (!m_allButtons.TryGetValue(buttonName, out TalentButton button)) continue;
            if (button.IsChecked()) return true;
        }

        if (msg) Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_need_previous_talent");
        return false;
    }

    private static bool CanBuyEndAbility(string button)
    {
        return !LoadUI.EndTalents.TryGetValue(button, out List<string> requirements) || requirements.All(requirement => m_checkedTalents.Find(x => x.name == requirement));
    }

    private static void ButtonEvent(TalentButton talentButton, Dictionary<string, Image> lines, string key)
    {
        if (!TalentManager.m_talents.TryGetValue(key, out Talent ability))
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_failed_to_get_talent");
            return;
        }

        talentButton.Select(ability);
        if (talentButton.IsChecked()) return;
        if (!IsEndAbility(talentButton.name)) return;
        if (!IsConnected(lines)) return;
        if (!CheckCost(ability.GetCost())) return;
        ability.m_onPurchase?.Invoke();
        TalentManager.PurchaseTalent(ability);
        switch (ability.m_type)
        {
            case TalentType.Passive:
                if (ability.m_status is { } status) Player.m_localPlayer.GetSEMan().AddStatusEffect(status.NameHash());
                if (ability.m_addToPassiveBar) PassiveBar.m_instance.Add(ability);
                break;
            case TalentType.Ability or TalentType.StatusEffect:
                SpellInventory.m_instance.Add(ability, SpellBook.Add(ability), true);
                break;
            case TalentType.Characteristic:
                CharacteristicManager.UpdateCharacteristicPoints();
                CharacteristicButtons.UpdateAllButtons();
                break;
        }

        talentButton.SetCheckmark(true);
        FillLines.Update();
        SkillTree.m_instance.Show();
    }
    
    public static void RemapButton(string name, Dictionary<string, Image> lines, string key)
    {
        if (!m_allButtons.TryGetValue(name, out TalentButton talentButton)) return;
        talentButton.m_button.onClick.RemoveAllListeners();
        talentButton.m_button.onClick.AddListener(() =>
        {
            ButtonEvent(talentButton, lines, key);
        });
        if (!talentButton.TryGetComponent(out ButtonSfx component)) return;
        component.Start();
    }
    
    [Description("Map a button to the Skill Tree UI")]
    public static void SetButton(Transform parent, string name, Dictionary<string, Image> lines, string key)
    {
        TalentButton talentButton = parent.Find(name).GetComponent<TalentButton>();
        talentButton.m_button.onClick.AddListener(() =>
        {
            ButtonEvent(talentButton, lines, key);
        });

        FillLines.m_fillLineMap[talentButton] = lines;

        foreach (var kvp in lines)
        {
            if (FillLines.m_allLines.TryGetValue(kvp.Value, out FillLines fillLine))
            {
                fillLine.AddButton(kvp.Key);
                fillLine.AddButton(name);
            }
            else
            {
                var fill = new FillLines(kvp.Value);
                fill.AddButton(kvp.Key);
                fill.AddButton(name);
            }
        }
    }

    public static bool IsTalentButton(Selectable button) => m_selectables.Contains(button);
}