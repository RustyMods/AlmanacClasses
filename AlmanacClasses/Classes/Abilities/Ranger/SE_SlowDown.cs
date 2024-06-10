using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public class SE_SlowDown : StatusEffect
{
    private readonly string m_key = "RangerHunter";
    private Talent m_talent = null!;

    private float m_pingTimer;
    
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_talent = talent;
        m_ttl = 10f;
        if (LoadedAssets.SE_Finder != null) m_startEffects = LoadedAssets.SE_Finder.m_pingEffectMed;
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        UpdatePing(dt);
    }

    private void UpdatePing(float dt)
    {
        m_pingTimer += dt;
        if (m_pingTimer < 1f) return;
        m_pingTimer = 0.0f;
        TriggerStartEffects();
    }

    public override void ModifySpeed(float baseSpeed, ref float speed, Character character, Vector3 dir)
    {
        speed *= m_talent.GetSpeedReduction(m_talent.GetLevel());
    }
}