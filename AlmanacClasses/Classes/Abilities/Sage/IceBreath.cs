using System.Collections;
using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Sage;

public static class IceBreath
{
    public static void TriggerIceBreath(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(StartIceBreath(damages));
    }
    private static IEnumerator StartIceBreath(HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(0.1f);
        Transform transform = Player.m_localPlayer.transform;
        VFX.DragonBreath.Create(Player.m_localPlayer.GetHeadPoint() + transform.forward * 3f, transform.rotation, transform);
        float timeout = 5f;
        float count = 0f;
        while (count < timeout)
        {
            Vector3 position = transform.position;
            Vector3 target = position + transform.forward * 10f;
            List<Character> characters = new();
            Character.GetCharactersInRange(target, 10f, characters);
            foreach (Character? character in characters)
            {
                if (character.IsPlayer()) continue;
                HitData hitData = new()
                {
                    m_blockable = true,
                    m_dodgeable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                    m_damage = damages,
                    m_pushForce = 100f,
                    m_ranged = true,
                    m_hitType = HitData.HitType.PlayerHit,
                    m_dir = transform.forward
                };
                hitData.SetAttacker(Player.m_localPlayer);
                character.ApplyDamage(hitData, true, true);
                if (character.m_nview && character.m_nview.IsValid())
                {
                    character.m_nview.ClaimOwnership();
                    if (character.GetSEMan().HaveStatusEffect("SE_IceBreaker".GetStableHashCode()))
                    {
                        character.GetSEMan().RemoveStatusEffect("SE_IceBreaker".GetStableHashCode());
                    }
                    character.GetSEMan().AddStatusEffect("SE_IceBreaker".GetStableHashCode());
                }
            }
            yield return new WaitForSeconds(1f);
            ++count;
        }
    }
}