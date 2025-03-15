using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Sage;

public static class Smite
{
    private static readonly LayerMask m_layerMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");
    private static int m_count;
    private static readonly List<Character> m_hitCharacters = new();
    
    public static void TriggerSmite(HitData.DamageTypes damages)
    {
        Transform transform = Player.m_localPlayer.transform;
        Vector3 pos = transform.position + transform.up * 1.5f + Player.m_localPlayer.GetLookDir();
        GameObject spell = Object.Instantiate(VFX.Lightning, pos , Quaternion.identity);
        if (spell.TryGetComponent(out Projectile projectile))
        {
            projectile.name = "smite";
            projectile.m_ttl = 1f;
            projectile.m_rayRadius = 0.1f;
            projectile.m_aoe = 0.25f;
            projectile.m_gravity = 4f;
            projectile.m_hitNoise = 40f;
            projectile.m_owner = Player.m_localPlayer;
            projectile.m_skill = Skills.SkillType.ElementalMagic;
            projectile.transform.localRotation = Quaternion.LookRotation(Player.m_localPlayer.GetLookDir());

            Vector3 target = Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 1000f;
            
            HitData hitData = new()
            {
                m_damage = damages,
                m_pushForce = 0f,
                m_dodgeable = false,
                m_blockable = false,
                m_skill = Skills.SkillType.ElementalMagic
            };
            hitData.SetAttacker(Player.m_localPlayer);
            Vector3 velocity = (target - pos).normalized * 25f;
            projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);
        }
    }

    private static void ResetCount()
    {
        m_count = 0;
        m_hitCharacters.Clear();
    }
    
    [HarmonyPatch(typeof(Projectile), nameof(Projectile.OnHit))]
    private static class Projectile_OnHit_Patch
    {
        private static void Prefix(Projectile __instance, Collider collider)
        {
            if (!__instance.name.Contains("smite")) return;
            if (!PlayerManager.m_playerTalents.TryGetValue("Smite", out Talent talent)) return;
            if (__instance.m_owner != Player.m_localPlayer) return;
            if (!Projectile.FindHitObject(collider).TryGetComponent(out Character hit)) return;
            m_hitCharacters.Add(hit);
            if (m_count > talent.GetProjectileCount(talent.GetLevel()))
            {
                ResetCount();
                return;
            }
            List<Character> charactersInRange = new();
            Character.GetCharactersInRange(__instance.transform.position, 30f, charactersInRange);
            Character? target = null;
            float num = 30f;
            foreach (var character in charactersInRange)
            {
                if (character.IsPlayer() || m_hitCharacters.Contains(character)) continue;
                var distance = Vector3.Distance(character.transform.position, __instance.transform.position);
                if (distance < num)
                {
                    num = distance;
                    target = character;
                }
            }

            if (target is null)
            {
                ResetCount();
                return;
            }

            var lightning = Object.Instantiate(__instance.gameObject, hit.GetTopPoint() + hit.transform.up, __instance.transform.rotation);
            var projectile = lightning.GetComponent<IProjectile>();
            var hitData = __instance.m_originalHitData.Clone();
            hitData.ApplyModifier(0.5f);
            var velocity = (target.GetCenterPoint() - lightning.transform.position).normalized * 25f;
            projectile.Setup(__instance.m_owner, velocity, __instance.m_hitNoise, hitData, null, null);
            m_hitCharacters.Add(target);
            ++m_count;
        }
    }
}