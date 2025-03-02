using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.UI;

namespace AlmanacClasses.UI;

public class CharacteristicButtons : MonoBehaviour
{
    public Text m_title = null!;
    public Text m_count = null!;
    public Button m_remove = null!;
    public Button m_add = null!;
    public Characteristic m_type = Characteristic.None;
    
    public void Init()
    {
        m_title = transform.Find("$text_title").GetComponent<Text>();
        m_count = transform.Find("$part_buttons/$text_count").GetComponent<Text>();
        m_remove = transform.Find("$part_buttons/$button_remove").GetComponent<Button>();
        m_add = transform.Find("$part_buttons/$button_add").GetComponent<Button>();
        if (m_type is Characteristic.None) return;
        m_title.gameObject.AddComponent<HoverCharacteristic>().m_type = m_type;
        var addHover = m_add.gameObject.AddComponent<HoverCharacteristic>();
        addHover.m_type = m_type;
        m_add.onClick.AddListener(() =>
        {
            if (CharacteristicManager.GetRemainingPoints() <= 0) return;
            CharacteristicManager.Add(m_type, 1);
            SetAmount(CharacteristicManager.GetCharacteristic(m_type));
            addHover.UpdateText();
        });
        var removeHover = m_remove.gameObject.AddComponent<HoverCharacteristic>();
        removeHover.m_type = m_type;
        m_remove.onClick.AddListener(() =>
        {
            if (CharacteristicManager.GetCharacteristic(m_type) <= 0) return;
            CharacteristicManager.Remove(m_type, 1);
            SetAmount(CharacteristicManager.GetCharacteristic(m_type));
            removeHover.UpdateText();
        });
        m_remove.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
        m_add.gameObject.AddComponent<ButtonSfx>().m_sfxPrefab = LoadUI.m_vanillaButtonSFX.m_sfxPrefab;
    }

    public void SetTitle(string text) => m_title.text = Localization.m_instance.Localize(text);
    public void SetAmount(int amount) => m_count.text = $"<color=orange>{amount}</color>";
    public void UpdateAmount() => SetAmount(CharacteristicManager.GetCharacteristic(m_type));
    public void DisplayAdd(bool enable) => m_add.gameObject.SetActive(enable);
    public void DisplayRemove(bool enable) => m_remove.gameObject.SetActive(enable);
    public static void UpdateAllButtons()
    {
        bool displayAdd = CharacteristicManager.GetRemainingPoints() > 0;
        foreach (var button in CharacteristicPanel.m_instance.m_components)
        {
            button.DisplayAdd(displayAdd);
            button.DisplayRemove(CharacteristicManager.GetCharacteristic(button.m_type) > 0);
        }
    }
}