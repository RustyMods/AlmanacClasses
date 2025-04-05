using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class SpellSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static GameObject? m_draggedSpellImage = null;
    public static SpellSlot? m_selectedSlot = null;
    public static bool m_hoveringSlot;

    // Prefab GameObjects
    public RectTransform? m_rect;
    public Image? m_icon;
    public Text? m_hotkey;
    public Image? m_gray;
    public Image? m_fill;
    public Text? m_timer;
    public Text? m_title;
    public Text[]? m_texts;

    public Talent? m_talent;
    public int m_index;
    private bool m_updating;
    public bool m_loaded;
    public void Awake() => Init();

    public void Init()
    {
        if (m_loaded) return;
        m_rect = GetComponent<RectTransform>();
        m_icon = Utils.FindChild(transform, "$image_icon").GetComponent<Image>();
        m_hotkey = Utils.FindChild(transform, "$text_hotkey").GetComponent<Text>();
        m_gray = Utils.FindChild(transform, "$image_gray").GetComponent<Image>();
        m_fill = Utils.FindChild(transform, "$image_fill").GetComponent<Image>();
        m_timer = Utils.FindChild(transform, "$text_timer").GetComponent<Text>();
        m_timer.transform.position += new Vector3(0f, -15f);
        m_title = Utils.FindChild(transform, "$text_title").GetComponent<Text>();
        m_texts = GetComponentsInChildren<Text>();
        HideName();
        m_loaded = true;
    }
    
    public void Update()
    {
        if (!Player.m_localPlayer || m_talent is null) return;
        try
        {
            if (AbilityManager.m_cooldownMap.TryGetValue(m_talent.m_key, out float ratio))
            {
                m_updating = true;
                if (m_talent.m_status is not { } status)
                {
                    SetBorder(0f);
                }
                else
                {
                    if (Player.m_localPlayer.GetSEMan().HaveStatusEffect(status.NameHash()))
                    {
                        StatusEffect? effect = Player.m_localPlayer.GetSEMan().GetStatusEffect(status.NameHash());
                        float time = effect.GetRemaningTime();
                        float normal = Mathf.Clamp01(time / effect.m_ttl);
                        SetBorder(time > 0 ? normal : 0f);
                    }
                    else
                    {
                        SetBorder(0f);
                    }
                }

                int cooldownTime = (int)(m_talent.GetCooldown(m_talent.GetLevel()) * ratio);
                SetTimer(GetCooldownColored(cooldownTime));
                SetFillAmount(ratio);
                if (cooldownTime <= 0)
                {
                    SetTimer(Localization.instance.Localize("$info_ready"));
                }
            }
            else if (m_updating)
            {
                SetBorder(0f);
                SetFillAmount(0f);
                SetTimer(Localization.instance.Localize("$info_ready"));
                SetTimer("");
                m_updating = false;
            }
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to update ability cooldown");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO: Right-click to remove from spellbook?
        if (eventData.button != PointerEventData.InputButton.Left || Input.GetKey(KeyCode.LeftAlt))
            return;

        if (m_selectedSlot != null)
        {
            if (m_selectedSlot == this)
            {
                // User tried to drop the spell on its original position, cancel it
                m_selectedSlot = null;
                Destroy(m_draggedSpellImage);
                m_draggedSpellImage = null;
                ShowSpellInfo();
            }
            else
            {
                if (m_talent == null)
                {
                    SetAbility(m_selectedSlot.m_talent);
                    m_selectedSlot.SetAbility(null);
                }
                else
                {
                    Swap(m_selectedSlot);
                }
                m_selectedSlot = null;
                Destroy(m_draggedSpellImage);
                m_draggedSpellImage = null;
            }
        }
        else
        {
            if (m_talent == null) return;
            m_selectedSlot = this;
            m_draggedSpellImage = new GameObject("SpellDummyIcon");
            m_draggedSpellImage.transform.SetParent(Hud.instance.transform, false);
            var image = m_draggedSpellImage.AddComponent<Image>();
            image.sprite = m_talent.GetSprite();
            image.raycastTarget = false;
            if (m_rect != null)
            {
                var rectTransform = m_draggedSpellImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = m_rect.sizeDelta * 0.75f;
            }
            SpellInfo.m_instance.Hide();
        }
    }

    public void Swap(SpellSlot other)
    {
        (m_talent, other.m_talent) = (other.m_talent, m_talent);
        other.UpdateData();
        UpdateData();
    }

    public void SetAbility(Talent? ability)
    {
        m_talent = ability;
        UpdateData();
    }

    public void UpdateData()
    {
        try
        {
            if (!m_loaded) return;
            if (m_talent == null)
            {
                SetName("");
                SetBorder(0f);
                SetHotkey("");
                SetTimer("");
                SetFillAmount(0f);
                SetIconVisibility(false);
            }
            else
            {
                SetName(m_talent.GetName());
                SetHotkey(GetKeyCode(m_index));
                SetIcon(m_talent.GetSprite());
                SetIconVisibility(true);
            }
        }
        catch
        {
            AlmanacClassesPlugin.AlmanacClassesLogger.LogDebug("Failed to update spell slot data");
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

    private static string AddAltKey(string key)
    {
        var spellModifier = AlmanacClassesPlugin._SpellAlt.Value;
        if (spellModifier is KeyCode.None) return key;
    
        switch (spellModifier)
        {
            case KeyCode.LeftAlt:
                return $"L.Alt + {key}";
            case KeyCode.LeftControl:
                return $"L.Ctrl + {key}";
            case KeyCode.RightAlt:
                return $"R.Alt + {key}";
            case KeyCode.RightControl:
                return $"R.Ctrl + {key}";
            case KeyCode.LeftShift:
                return $"L.Shift + {key}";
            case KeyCode.RightShift:
                return $"R.Shift + {key}";
            default:
                return $"{spellModifier} + {key}";
        }  
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Input.GetKey(KeyCode.LeftAlt) && m_draggedSpellImage == null)
        {
            ShowSpellInfo();
        }
        if (m_icon != null) m_icon.rectTransform.sizeDelta *= 1.15f;
        m_hoveringSlot = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        HideName();
        SpellInfo.m_instance.Hide();
        if (m_icon != null) m_icon.rectTransform.sizeDelta /= 1.15f;
        m_hoveringSlot = false;
    }

    private void ShowSpellInfo()
    {
        if (m_talent == null) return;
        
        if (!SkillTree.IsPanelVisible())
        {
            SpellInfo.m_instance.Show();
            SpellInfo.m_instance.SetName(Localization.instance.Localize($"<color=orange>{m_talent.GetName()}</color>"));
            SpellInfo.m_instance.SetDescription(m_talent.GetTooltip());
        }
        else
        {
            ShowName();
        }
    }

    public void SetIcon(Sprite? sprite)
    {
        if (m_icon is null) return;
        m_icon.sprite = sprite;
    }

    public void SetIconVisibility(bool isEnabled)
    {
        if (m_icon is null) return;
        m_icon.enabled = isEnabled;
    }

    public void SetHotkey(string text)
    {
        if (m_hotkey is null) return;
        m_hotkey.text = text;
    }

    public void SetBorder(float amount)
    {
        if (m_gray is null) return;
        m_gray.fillAmount = amount;
    }

    public void SetFillAmount(float amount)
    {
        if (m_fill is null) return;
        m_fill.fillAmount = amount;
    }

    public void SetTimer(string text)
    {
        if (m_timer is null) return;
        m_timer.text = text;
    }

    public void SetName(string text)
    {
        if (m_title is null) return;
        m_title.text = Localization.instance.Localize(text);
    }

    public void ShowName()
    {
        if (m_title == null) return;
        m_title.gameObject.SetActive(true);
    }

    public void HideName()
    {
        if (m_title == null) return;
        m_title.gameObject.SetActive(false);
    }
    public void SetFont(Font? font)
    {
        if (m_texts == null) return;
        foreach (var text in m_texts) text.font = font;
    }
    public static void UpdateFont(Font? font)
    {
        foreach (var instance in SpellBook.m_slots.Values)
        {
            instance.SetFont(font);
        }
    }
}