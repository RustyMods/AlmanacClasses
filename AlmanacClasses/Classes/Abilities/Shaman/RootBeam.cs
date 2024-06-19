using System.Collections;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Shaman;

public static class RootBeam
{
    private static readonly LayerMask m_layerMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

    public static void TriggerRootBeam(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(StartRootBeam(0.3f, damages));
    }

    private static IEnumerator StartRootBeam(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        
        int amount = 0;
        int maxAmount = 10;
        while (amount < maxAmount)
        {
            Transform transform = Player.m_localPlayer.transform;
            Vector3 position = transform.position;
            Vector3 forward = Player.m_localPlayer.GetLookDir() * 5f;
            Vector3 target = position + forward + transform.up * 1.4f;
            GameObject beam = Object.Instantiate(LoadedAssets.GDKingRoots, target, Quaternion.identity);
            if (beam.TryGetComponent(out Projectile projectile))
            {
                projectile.name = "RootBeam";
                projectile.m_ttl = 50f;
                projectile.m_rayRadius = 0.5f;
                projectile.m_aoe = 0.2f;
                projectile.m_owner = Player.m_localPlayer;
                projectile.m_skill = Skills.SkillType.ElementalMagic;
                projectile.transform.localRotation = Quaternion.LookRotation(forward);

                bool flag = Physics.Raycast(target, forward, out RaycastHit hit, float.PositiveInfinity, m_layerMask);
            
                Vector3 hitTarget = !flag || !hit.collider ? target + transform.forward * 1000f : hit.point;

                HitData hitData = new()
                {
                    m_damage = damages,
                    m_pushForce = 0.4f,
                    m_dodgeable = true,
                    m_blockable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                };
                hitData.SetAttacker(Player.m_localPlayer);
                
                Vector3 velocity = (hitTarget - target).normalized * 25f;
                projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);

                yield return new WaitForSeconds(delay);
                ++amount;
            }
        }
    }
}