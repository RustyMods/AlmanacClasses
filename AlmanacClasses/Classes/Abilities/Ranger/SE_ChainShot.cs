using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public class SE_ChainShot : StatusEffect
{
    private const string m_key = "ChainShot";
    private Talent m_talent = null!;
    private int m_count;
    private readonly List<Character> m_hitCharacters = new();
    private float m_resetTimer;
    public override void Setup(Character character)
    {
        if (TalentManager.m_talents.TryGetValue(m_key, out Talent talent))
        {
               m_ttl = talent.GetLength(talent.GetLevel());
            m_startEffects = talent.GetEffectList();
            m_talent = talent;
        }
        base.Setup(character);
    }

    public override void UpdateStatusEffect(float dt)
    {
        base.UpdateStatusEffect(dt);
        m_resetTimer += dt;
        if (m_resetTimer < 10f) return;
        m_resetTimer = 0.0f;
        ResetCount();
    }

    public void ResetCount()
    {
        m_count = 0;
        m_hitCharacters.Clear();
        m_resetTimer = 0.0f;
    }

    [HarmonyPatch(typeof(Projectile), nameof(Projectile.OnHit))]
    private static class Projectile_OnHit_Patch
    {
        private static void Prefix(Projectile __instance, Collider collider)
        {
            if (!PlayerManager.m_playerTalents.TryGetValue("ChainShot", out Talent talent)) return;
            if (talent.m_status is not { } status || !Player.m_localPlayer.GetSEMan().HaveStatusEffect(status.NameHash())) return;
            if (__instance.m_owner != Player.m_localPlayer) return;
            if (__instance.m_skill is not Skills.SkillType.Bows or Skills.SkillType.Crossbows) return;
            if (Player.m_localPlayer.GetSEMan().GetStatusEffect(status.NameHash()) is not SE_ChainShot chainShot) return;
            if (!Projectile.FindHitObject(collider).TryGetComponent(out Character hit)) return;
            chainShot.m_hitCharacters.Add(hit);
            if (chainShot.m_count > talent.GetProjectileCount(talent.GetLevel()))
            {
                chainShot.ResetCount();
                return;
            }
            List<Character> charactersInRange = new();
            Character.GetCharactersInRange(__instance.transform.position, 30f, charactersInRange);
            Character? target = null;
            float num = 30f;
            foreach (var character in charactersInRange)
            {
                if (character.IsPlayer() || chainShot.m_hitCharacters.Contains(character)) continue;
                var distance = Vector3.Distance(character.transform.position, __instance.transform.position);
                if (distance < num)
                {
                    num = distance;
                    target = character;
                }
            }

            if (target is null)
            {
                chainShot.ResetCount();
                return;
            }

            var arrow = Instantiate(__instance.gameObject, hit.GetTopPoint() + hit.transform.up * 1.01f, __instance.transform.rotation);
            var projectile = arrow.GetComponent<IProjectile>();
            var hitData = __instance.m_originalHitData.Clone();
            hitData.ApplyModifier(0.5f);
            var velocity = (target.GetCenterPoint() - arrow.transform.position).normalized * 25f;
            projectile.Setup(__instance.m_owner, velocity, __instance.m_hitNoise, hitData, __instance.m_weapon, __instance.m_ammo);
            chainShot.m_hitCharacters.Add(target);
            ++chainShot.m_count;
        }
    }
}