// using System.Collections.Generic;
// using HarmonyLib;
// using UnityEngine;
//
// namespace AlmanacClasses.Classes.Abilities.Ranger;
//
// public class SE_ChainShot : StatusEffect
// {
//     private readonly string m_key = "ChainShot";
//     private Talent m_talent = null!;
//     private static int m_count;
//
//     public override void Setup(Character character)
//     {
//         if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
//         m_ttl = talent.GetLength(talent.GetLevel());
//         m_startEffects = talent.GetEffectList();
//         m_talent = talent;
//         base.Setup(character);
//     }
//
//     [HarmonyPatch(typeof(Projectile), nameof(Projectile.OnHit))]
//     private static class Projectile_OnHit_Patch
//     {
//         private static void Prefix(Projectile __instance, Collider collider)
//         {
//             if (__instance.m_owner != Player.m_localPlayer) return;
//             if (__instance.m_skill is not Skills.SkillType.Bows or Skills.SkillType.Crossbows) return;
//             if (!Projectile.FindHitObject(collider).GetComponent<Character>()) return;
//             if (m_count > 3)
//             {
//                 m_count = 0;
//                 return;
//             }
//             // if (!PlayerManager.m_playerTalents.TryGetValue("ChainShot", out Talent talent)) return;
//             List<Character> charactersInRange = new();
//             Character.GetCharactersInRange(__instance.transform.position, 30f, charactersInRange);
//             Character? target = null;
//             float num = 30f;
//             foreach (var character in charactersInRange)
//             {
//                 if (character.IsPlayer()) continue;
//                 var distance = Vector3.Distance(character.transform.position, __instance.transform.position);
//                 if (distance < num)
//                 {
//                     num = distance;
//                     target = character;
//                 }
//             }
//
//             if (target is null)
//             {
//                 m_count = 0;
//                 return;
//             }
//             var projectile = Instantiate(__instance.gameObject, __instance.transform.position,
//                 __instance.transform.rotation).GetComponent<IProjectile>();
//             var hitData = __instance.m_originalHitData.Clone();
//             hitData.ApplyModifier(0.5f);
//             var normalized = (target.GetCenterPoint() - __instance.transform.position).normalized;
//             var aimDir = Vector3.RotateTowards(__instance.transform.forward, normalized, 1.5707964f, 1f);
//             projectile.Setup(__instance.m_owner, aimDir * 100f, __instance.m_hitNoise, hitData, __instance.m_weapon, __instance.m_ammo);
//             ++m_count;
//         }
//     }
//     
// }