using System.Collections;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Sage;

public class StoneThrow
{
    private static readonly LayerMask m_layerMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

    public static void TriggerStoneThrow(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedStoneThrowSpawn(1f, damages));
    }
    
    private static IEnumerator DelayedStoneThrowSpawn(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        Transform transform = Player.m_localPlayer.transform;
        Vector3 pos = transform.position + transform.up * 5f + Player.m_localPlayer.GetLookDir();
        GameObject spell = Object.Instantiate(LoadedAssets.TrollStone, pos, Quaternion.identity);
        if (spell.TryGetComponent(out Projectile projectile))
        {
            projectile.name = "StoneThrow";
            projectile.m_ttl = 10f;
            projectile.m_rayRadius = 1f;
            projectile.m_aoe = 5f;
            projectile.m_gravity = 5f;
            projectile.m_hitNoise = 40f;
            projectile.m_owner = Player.m_localPlayer;
            projectile.m_skill = Skills.SkillType.ElementalMagic;
            projectile.m_raiseSkillAmount = 10f;
            projectile.transform.localRotation = Quaternion.LookRotation(Player.m_localPlayer.GetLookDir());

            bool flag = !Physics.Raycast(Player.m_localPlayer.GetEyePoint(), Player.m_localPlayer.GetLookDir(), out RaycastHit hit, 1000f, m_layerMask) || !hit.collider;

            Vector3 target = flag
                ? Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 1000f
                : hit.point;

            HitData hitData = new()
            {
                m_damage = damages,
                m_pushForce = 10f,
                m_dodgeable = true,
                m_blockable = true,
                m_skill = Skills.SkillType.ElementalMagic,
                m_skillRaiseAmount = 1f,
                m_skillLevel = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.ElementalMagic)
            };
            hitData.SetAttacker(Player.m_localPlayer);
            hitData.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));

            Vector3 velocity = (target - pos).normalized * 25f;

            projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);
        }
    }
}