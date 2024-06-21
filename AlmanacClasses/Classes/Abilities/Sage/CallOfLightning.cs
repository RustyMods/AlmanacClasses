using System.Collections;
using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Sage;

public static class CallOfLightning
{
    private static GameObject[]? startEffects;
    public static bool TriggerLightningAOE(Talent talent)
    {
        Transform transform = Player.m_localPlayer.transform;
        Vector3 location = Player.m_localPlayer.GetLookDir() * 10f + transform.position;
        List<Character> characters = new();
        Character.GetCharactersInRange(location, 10f, characters);
        if (characters.Count <= 0)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_no_targets");
            return false;
        }
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedCast(4f, talent, characters));

        return true;
    }

    private static IEnumerator DelayedCast(float delay, Talent talent, List<Character> characters)
    {
        HitData.DamageTypes damages = talent.GetDamages(talent.GetLevel());
        Transform transform = Player.m_localPlayer.transform;
        if (talent.UseEffects())
        {
            startEffects = LoadedAssets.FX_Electric.Create(transform.position, transform.rotation, transform, 1.5f);
        }
        yield return new WaitForSeconds(delay);
        for (int index = 0; index < characters.Count; index++)
        {
            if (index > 5) break;
            Character? character = characters[index];
            if (character == null || character.IsDead()) continue;
            if (character.IsPlayer()) continue;
            if (character.GetFaction() is Character.Faction.Players) continue;
            yield return new WaitForSeconds(1f);
            GameObject spell = Object.Instantiate(LoadedAssets.lightning_AOE, character.transform.position, Quaternion.identity);

            Transform AOE_ROD = Utils.FindChild(spell.transform, "AOE_ROD");
            if (AOE_ROD.TryGetComponent(out Aoe AOE_Component))
            {
                SetAOEComponent(AOE_Component, damages);
            }
        }

        if (startEffects == null || !ZNetScene.instance) yield break;
        foreach (GameObject effect in startEffects)
        {
            if (!effect) continue;
            if (!effect.TryGetComponent(out ZNetView component)) continue;
            if (!component.IsValid()) continue;
            component.ClaimOwnership();
            component.Destroy();
        }
    }
    private static void SetAOEComponent(Aoe component, HitData.DamageTypes damages)
    {
        component.m_owner = Player.m_localPlayer;
        component.m_skill = Skills.SkillType.ElementalMagic;
        component.m_canRaiseSkill = true;
        component.m_useTriggers = true;
        component.m_triggerEnterOnly = true;
        component.m_damage = damages;
        component.m_blockable = true;
        component.m_dodgeable = true;
        component.m_hitProps = false;
        component.m_hitOwner = false;
        component.m_hitParent = false;
        component.m_hitFriendly = false;
    }
}