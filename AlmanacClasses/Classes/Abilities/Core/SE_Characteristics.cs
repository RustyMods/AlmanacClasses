namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_Characteristics : StatusEffect
{
    public override void ModifyMaxCarryWeight(float baseLimit, ref float limit)
    {
        limit += CharacteristicManager.GetCarryWeightRatio();
    }

    public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
    {
        hitData.m_damage.m_blunt *= CharacteristicManager.GetStrengthModifier();
        hitData.m_damage.m_slash *= CharacteristicManager.GetStrengthModifier();
        hitData.m_damage.m_pierce *= CharacteristicManager.GetStrengthModifier();

        hitData.m_damage.m_fire *= CharacteristicManager.GetIntelligenceModifier();
        hitData.m_damage.m_frost *= CharacteristicManager.GetIntelligenceModifier();
        hitData.m_damage.m_lightning *= CharacteristicManager.GetIntelligenceModifier();
        hitData.m_damage.m_poison *= CharacteristicManager.GetIntelligenceModifier();
        hitData.m_damage.m_spirit *= CharacteristicManager.GetIntelligenceModifier();
    }
}