
namespace AlmanacClasses.Classes.Abilities;

public class SE_Hunter : StatusEffect
{
    public float m_pingTimer;

    public override void UpdateStatusEffect(float dt)
    {
        m_time += dt;
        m_pingTimer += dt;
        if (m_pingTimer < 1f) return;
        m_pingTimer = 0.0f;
        TriggerStartEffects();
        if (m_character.TryGetComponent(out AnimalAI animalAI))
        {
            animalAI.m_hearRange = 0.0f;
            animalAI.SetAlerted(false);
        }

        if (m_character.TryGetComponent(out MonsterAI monsterAI))
        {
            monsterAI.m_hearRange = 0.0f;
            monsterAI.SetAlerted(false);
        }
    }

    public override void ModifySpeed(float baseSpeed, ref float speed)
    {
        speed *= 0.5f;
    }

    public override void Stop()
    {
        RemoveStartEffects();
    }
}