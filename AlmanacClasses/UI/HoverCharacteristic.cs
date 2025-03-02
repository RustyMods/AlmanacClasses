using System.Text;
using AlmanacClasses.Classes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlmanacClasses.UI;

public class HoverCharacteristic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Characteristic m_type = Characteristic.None;

    public void UpdateText()
    {
        if (m_type is Characteristic.None) return;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"$almanac_{m_type.ToString().ToLower()}\n\n");
        stringBuilder.Append($"${m_type.ToString().ToLower()}_desc\n");
        switch (m_type)
        {
            case Characteristic.Constitution:
                int health = (int)CharacteristicManager.GetHealthRatio();
                if (health > 0) stringBuilder.AppendFormat("$se_health: <color=orange>+{0}</color>\n", health);
                break;
            case Characteristic.Strength:
                int carryWeight = (int)CharacteristicManager.GetCarryWeightRatio();
                float physical = FormatPercentage(CharacteristicManager.GetStrengthModifier());
                if (carryWeight > 0) stringBuilder.AppendFormat("$se_max_carryweight: <color=orange>+{0}</color>\n", carryWeight);
                if (physical > 0) stringBuilder.AppendFormat("$almanac_physical: <color=orange>+{0:0.0}%</color>\n", physical);
                break;
            case Characteristic.Intelligence:
                float intel = FormatPercentage(CharacteristicManager.GetIntelligenceModifier());
                if (intel > 0) stringBuilder.AppendFormat("$almanac_elemental: <color=orange>+{0:0.0}%</color>\n", intel);
                break;
            case Characteristic.Dexterity:
                int stamina = (int)CharacteristicManager.GetStaminaRatio();
                float attackSpeed = FormatPercentage(CharacteristicManager.GetDexterityModifier());
                if (stamina > 0) stringBuilder.AppendFormat("$se_stamina: <color=orange>+{0}</color>\n", stamina);
                if (attackSpeed > 0) stringBuilder.AppendFormat("$almanac_attackspeedmod: <color=orange>+{0:0.0}%</color>", attackSpeed);
                break;
            case Characteristic.Wisdom:
                int eitr = (int)CharacteristicManager.GetEitrRatio();
                if (eitr > 0) stringBuilder.AppendFormat("$se_eitr: <color=orange>+{0}</color>", eitr);
                break;
        }
        CharacteristicPanel.m_instance.SetTooltip(stringBuilder.ToString());
    }
    
    public void OnPointerEnter(PointerEventData eventData) => UpdateText();
    
    public void OnPointerExit(PointerEventData eventData) => CharacteristicPanel.m_instance.SetDefaultTooltip();

    private float FormatPercentage(float value) => value * 100 - 100;

}