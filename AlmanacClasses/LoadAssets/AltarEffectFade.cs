using UnityEngine;

namespace AlmanacClasses.LoadAssets;

public class AltarEffectFade : MonoBehaviour
{
    public float m_fadeDuration = 1f;
    public ParticleSystem[] m_particles = null!;
    public Material m_rune = null!;

    public Color m_baseColor;
    public bool m_active;
    public float m_intensity;

    public void Awake()
    {
        GameObject effects = Utils.FindChild(transform, "vfx").gameObject;
        
        m_particles = effects.GetComponentsInChildren<ParticleSystem>();
        m_rune = Utils.FindChild(transform, "emissive").GetComponent<MeshRenderer>().material;
        m_baseColor = m_rune.color;
        m_rune.color = new Color(m_baseColor.r, m_baseColor.g, m_baseColor.b, 0f);

        SetParticleSystems(false, false);
    }

    public void Update()
    {
        Player closestPlayer = Player.GetClosestPlayer(transform.position, 5f);
        SetParticleSystems(closestPlayer != null);

        m_intensity = Mathf.MoveTowards(m_intensity, closestPlayer != null ? 1f : 0.0f, Time.deltaTime / m_fadeDuration);
        m_rune.color = new Color(m_baseColor.r, m_baseColor.g, m_baseColor.b, m_intensity * 1f);
    }

    public void SetParticleSystems(bool active, bool checkBool = true)
    {
        if (m_active == active && checkBool) return;
        m_active = active;
        foreach (ParticleSystem particle in m_particles)
        {
            ParticleSystem.EmissionModule particleEmission = particle.emission;
            particleEmission.enabled = active;
        }
    }
}