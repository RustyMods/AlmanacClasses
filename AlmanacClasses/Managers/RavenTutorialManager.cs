using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlmanacClasses.Managers;

public static class RavenTutorialManager
{
    public static void InitCustomTutorials()
    {
        GameObject altar = ZNetScene.instance.GetPrefab("AlmanacClassAltar");
        AddCustomTutorials(
            altar, 
            "$piece_class_altar", 
            "$almanac_class_altar_info"
            );
    }
    private static void AddCustomTutorials(GameObject prefab,string topic, string text)
    {
        GameObject? ravens = GetRavens();
        if (ravens == null) return;
        Transform guide = Utils.FindChild(prefab.transform, "guidepoint");
        if (!guide.TryGetComponent(out GuidePoint component)) return;
        component.m_text.m_alwaysSpawn = false;
        component.m_ravenPrefab = ravens;
        component.m_text.m_topic = topic;
        component.m_text.m_label = topic;
        component.m_text.m_text = text;
    }

    private static GameObject? GetRavens()
    {
        List<GameObject> allObjects = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
        GameObject Ravens = allObjects.Find(item => item.name == "Ravens" && item.transform.GetChild(0).name == "Hugin");
        return !Ravens ? null : Ravens;
    }
}