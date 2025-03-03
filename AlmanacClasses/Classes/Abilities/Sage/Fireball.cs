using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Sage;

public static class Fireball
{
    private static readonly LayerMask m_layerMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

    public static void TriggerFireball(HitData.DamageTypes damages)
    {
        var transform = Player.m_localPlayer.transform;
        Vector3 pos = transform.position + transform.up * 1.5f + Player.m_localPlayer.GetLookDir();
        GameObject spell = Object.Instantiate(VFX.Fireball, pos , Quaternion.identity);
        if (spell.TryGetComponent(out Projectile projectile))
        {
            projectile.name = "Fireball";
            projectile.m_ttl = 10f;
            projectile.m_rayRadius = 1f;
            projectile.m_aoe = 5f;
            projectile.m_gravity = 5f;
            projectile.m_hitNoise = 40f;
            projectile.m_owner = Player.m_localPlayer;
            projectile.m_skill = Skills.SkillType.ElementalMagic;
            projectile.transform.localRotation = Quaternion.LookRotation(Player.m_localPlayer.GetLookDir());

            Vector3 target = Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 1000f;

            HitData hitData = new()
            {
                m_damage = damages,
                m_pushForce = 10f,
                m_dodgeable = true,
                m_blockable = true,
                m_skill = Skills.SkillType.ElementalMagic
            };
            hitData.SetAttacker(Player.m_localPlayer);
            Vector3 velocity = (target - pos).normalized * 25f;
            projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);
        }
    }
    
}