using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlmanacClasses.LoadAssets;

public class AltarEffectFade : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    public float m_fadeDuration = 1f;
    public ParticleSystem[] m_particles = null!;
    public List<Material> m_materials = new();
    public Color m_baseColor = new Color(1f, 0.5f, 0f, 1f);
    public bool m_active;
    public float m_intensity;

    public void Awake()
    {
        m_particles = Utils.FindChild(transform, "vfx").gameObject.GetComponentsInChildren<ParticleSystem>();
        List<Material> mats = Utils.FindChild(transform, "startplatform_mat")
            .GetComponentsInChildren<MeshRenderer>()
            .SelectMany(mesh => mesh.materials)
            .ToList();

        m_materials.AddRange(mats);

        SetEmission(0f);
        SetParticleSystems(false, false);
    }

    public void Update()
    {
        Player closestPlayer = Player.GetClosestPlayer(transform.position, 5f);
        m_intensity = Mathf.MoveTowards(m_intensity, closestPlayer is not null ? 1f : 0.0f, Time.deltaTime / m_fadeDuration);
        SetParticleSystems(closestPlayer is not null);
        SetEmission(m_intensity);
    }

    public void SetEmission(float intensity)
    {
        foreach (var mat in m_materials)
        {
            mat.SetColor(EmissionColor, m_baseColor *  intensity);
        }
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