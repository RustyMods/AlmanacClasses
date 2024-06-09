namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_ExperienceOrb : StatusEffect
{
    public float m_experienceTimer;

    public override void UpdateStatusEffect(float dt)
    {
        m_time += dt;
        m_experienceTimer += dt;
        if (m_experienceTimer < 1f) return;
        m_experienceTimer = 0.0f;
        PlayerManager.AddExperience(1);
    }
}