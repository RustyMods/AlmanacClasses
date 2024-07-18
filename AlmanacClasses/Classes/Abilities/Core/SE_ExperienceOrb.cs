using BepInEx.Configuration;

namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_ExperienceOrb : StatusEffect
{
    public ConfigEntry<int> m_amount = null!;

    public override void Setup(Character character)
    {
        base.Setup(character);
        PlayerManager.AddExperience(m_amount.Value);
    }

    public override string GetTooltipString()
    {
        return $"$info_orbs_desc: <color=orange>{m_amount.Value}</color>";
    }
}