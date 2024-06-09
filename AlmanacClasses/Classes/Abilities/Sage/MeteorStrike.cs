using System.Collections;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Sage;

public class MeteorStrike
{
    private static readonly LayerMask m_layerMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

    public static void TriggerMeteor(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedMeteorSpawn(1f, damages));
    }
    private static IEnumerator DelayedMeteorSpawn(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        float radius = Player.m_localPlayer.GetRadius();
        Transform transform = Player.m_localPlayer.transform;
        GameObject[] instance =  LoadedAssets.VFX_SongOfSpirit.Create(Player.m_localPlayer.GetCenterPoint(), transform.rotation, transform, radius * 2f);
        int max = 3;
        int count = 0;
        while (count < max)
        {
            Vector3 pos = transform.position + transform.up * 50f + Player.m_localPlayer.GetLookDir();
            GameObject spell = Object.Instantiate(LoadedAssets.Meteor, pos, Quaternion.identity);
            if (spell.TryGetComponent(out Projectile projectile))
            {
                projectile.name = "MeteorStrike";
                projectile.m_ttl = 60f;
                projectile.m_rayRadius = 1f;
                projectile.m_aoe = 5f;
                projectile.m_hitNoise = 100f;
                projectile.m_owner = Player.m_localPlayer;
                projectile.m_skill = Skills.SkillType.ElementalMagic;
                projectile.m_raiseSkillAmount = 1f;
                projectile.transform.localRotation = Quaternion.LookRotation(Player.m_localPlayer.GetLookDir());

                bool flag = !Physics.Raycast(Player.m_localPlayer.GetEyePoint(), Player.m_localPlayer.GetLookDir(), out RaycastHit hit, 1000f, m_layerMask) || !hit.collider;
                
                Vector3 target =  flag ? Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 1000f : hit.point;
                target += Random.insideUnitSphere * 15f;

                HitData hitData = new()
                {
                    m_damage = damages,
                    m_pushForce = 0.5f,
                    m_dodgeable = true,
                    m_blockable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                    m_skillRaiseAmount = 1f,
                    m_skillLevel = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.ElementalMagic)
                };
                hitData.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));
                hitData.SetAttacker(Player.m_localPlayer);

                Vector3 velocity = (target - pos).normalized * 25f;
                
                projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);
                
                yield return new WaitForSeconds(delay * 0.5f);
            }
            ++count;
        }

        yield return new WaitForSeconds(2f);
        foreach (GameObject effect in instance)
        {
            ZNetScene.instance.Destroy(effect);
        }
    }
}