using System.Collections;
using AlmanacClasses.LoadAssets;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

public static class SpawnSystem
{
    public static IEnumerator DelayedMultipleSpawn(GameObject creature, string name, int level, float delay, int max)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 location = Player.m_localPlayer.GetLookDir() * 5f + Player.m_localPlayer.transform.position;
        int count = 0;
        while (count < max)
        {
            var random = Random.insideUnitSphere * 10f;
            Vector3 pos = location + new Vector3(random.x, 10f, random.z);
            ZoneSystem.instance.GetSolidHeight(pos, out float height, 1000);
            if (height >= 0.0 && Mathf.Abs(height - pos.y) <= 2f && Vector3.Distance(location, Player.m_localPlayer.transform.position) >= 2f)
            {
                pos.y = height;
            }
            LoadedAssets.StaffPreSpawnEffects.Create(pos, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
            GameObject critter = Object.Instantiate(creature, pos, Quaternion.identity);
            
            if (critter.TryGetComponent(out ZNetView component))
            {
                component.GetZDO().Set(Friendly.Hash, true);
                component.GetZDO().Set(ZDOVars.s_tamedName, name);
                component.GetZDO().Set(ZDOVars.s_level, level);
                component.GetZDO().Set(ZDOVars.s_owner, Player.m_localPlayer.GetPlayerID());
                component.GetZDO().Set(Friendly.DeathDelayHash, delay);
            }
            ++count;
        }
    }
    public static IEnumerator DelayedSpawn(GameObject creature, string name, int level, float delay)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 location = GetSpawnPosition();
        LoadedAssets.StaffPreSpawnEffects.Create(location, Quaternion.identity);
        yield return new WaitForSeconds(2.5f);
        GameObject critter = Object.Instantiate(creature, location, Quaternion.identity);
        if (critter.TryGetComponent(out ZNetView component))
        {
            component.GetZDO().Set(Friendly.Hash, true);
            component.GetZDO().Set(ZDOVars.s_tamedName, name);
            component.GetZDO().Set(ZDOVars.s_level, level);
            component.GetZDO().Set(ZDOVars.s_owner, Player.m_localPlayer.GetPlayerID());
            component.GetZDO().Set(Friendly.DeathDelayHash, delay * level);
        }
    }

    public static void StartSpawnAnimation()
    {
        if (Player.m_localPlayer.InEmote()) Player.m_localPlayer.StopEmote();
        Player.m_localPlayer.m_zanim.SetTrigger("staff_summon");
        LoadedAssets.FX_SummonSkeleton.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
    }
    private static Vector3 GetSpawnPosition()
    {
        Vector3 location = Player.m_localPlayer.GetLookDir() * 5f + Player.m_localPlayer.transform.position;
        ZoneSystem.instance.GetSolidHeight(location, out float height, 1000);
        if (height >= 0.0 && Mathf.Abs(height - location.y) <= 2f && Vector3.Distance(location, Player.m_localPlayer.transform.position) >= 2f)
        {
            location.y = height;
        }

        return location;
    }
}