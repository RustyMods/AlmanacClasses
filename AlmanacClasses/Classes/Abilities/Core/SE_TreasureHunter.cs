using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Core;

public class SE_TreasureHunter : StatusEffect
{
    private readonly string m_key = "TreasureHunter";
    private Talent m_talent = null!;
    
    public EffectList m_pingEffectNear = new EffectList();
    public EffectList m_pingEffectMed = new EffectList();
    public EffectList m_pingEffectFar = new EffectList();
    public float m_closeFrequency = 1f;
    public float m_distantFrequency = 5f;
    public float m_updateBeaconTimer;
    public float m_pingTimer;
    public Beacon? m_beacon;

    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_talent = talent;
        m_ttl = talent.GetLength(talent.GetLevel());
        
        if (LoadedAssets.SE_Finder != null)
        {
          m_pingEffectNear = LoadedAssets.SE_Finder.m_pingEffectNear;
          m_pingEffectMed = LoadedAssets.SE_Finder.m_pingEffectMed;
          m_pingEffectFar = LoadedAssets.SE_Finder.m_pingEffectFar;
        }
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        m_updateBeaconTimer += dt;
        if (m_updateBeaconTimer > 1.0)
        {
            m_updateBeaconTimer = 0.0f;
            Beacon closestBeaconInRange = Beacon.FindClosestBeaconInRange(m_character.transform.position);
            if (closestBeaconInRange != m_beacon)
            {
              m_beacon = closestBeaconInRange;
              if (m_beacon) m_pingTimer = 0.0f;
            }
        }
        if (m_beacon == null) return;
        float num1 = Utils.DistanceXZ(m_character.transform.position, m_beacon.transform.position);
        float t = Mathf.Clamp01(num1 / m_beacon.m_range);
        float num2 = Mathf.Lerp(m_closeFrequency, m_distantFrequency, t);
        m_pingTimer += dt;
        if (m_pingTimer <= num2) return;
        m_pingTimer = 0.0f;
        Transform transform = m_character.transform;
        if (t < 0.20000000298023224)
          m_pingEffectNear.Create(transform.position, transform.rotation, transform);
        else if (t < 0.6000000238418579)
          m_pingEffectMed.Create(m_character.transform.position, transform.rotation, transform);
        else
          m_pingEffectFar.Create(m_character.transform.position, transform.rotation, transform);
    }
}