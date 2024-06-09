using System.Collections;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Ranger;

public static class RangerTrap
{
    public static void TriggerSpawnTrap(HitData.DamageTypes damages, float delay)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedTrap(delay, damages));
    }

    private static IEnumerator DelayedTrap(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(2f);
        Vector3 location = Player.m_localPlayer.transform.position;
        LoadedAssets.TrapArmedEffects.Create(location, Quaternion.identity);
        GameObject trap = Object.Instantiate(LoadedAssets.CustomTrap, location, Quaternion.identity);
        if (trap.TryGetComponent(out ZNetView zNetView))
        {
            if (zNetView.IsValid()) zNetView.GetZDO().Persistent = false;
        }
        if (trap.TryGetComponent(out Trap component))
        {
            component.m_aoe.m_damage = damages;
            component.m_aoe.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.Bows),0.1f, 1f));
            component.m_triggeredByPlayers = false;
            component.m_aoe.m_hitFriendly = false;
            component.m_aoe.m_owner = Player.m_localPlayer;
            component.m_startsArmed = true;
        }

        yield return new WaitForSeconds(delay);
        if (trap.TryGetComponent(out WearNTear wearNTear))
        {
            wearNTear.Destroy();
        }
    }
}