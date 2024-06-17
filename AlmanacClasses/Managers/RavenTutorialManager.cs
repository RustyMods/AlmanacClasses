using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Managers;

public static class RavenTutorialManager
{
    private static readonly int Flyin = Animator.StringToHash("flyin");
    private static readonly int Teleportin = Animator.StringToHash("teleportin");

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
        component.m_ravenPrefab = ravens;
        component.m_text.m_key = "almanac_class_system";
        component.m_text.m_topic = topic;
        component.m_text.m_label = topic;
        component.m_text.m_text = text;
        component.m_text.m_priority = 1000;
    }

    private static GameObject? GetRavens()
    {
        List<GameObject> allObjects = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
        GameObject Ravens = allObjects.Find(item => item.name == "Ravens" && item.transform.GetChild(0).name == "Hugin");
        return !Ravens ? null : Ravens;
    }
    
    [HarmonyPatch(typeof(Raven), nameof(Raven.Spawn))]
    private static class Raven_Spawn_Postfix
    {
        private static void Postfix(Raven __instance, Raven.RavenText text, bool forceTeleport)
        {
            if (!__instance) return;
            if (Utils.GetMainCamera() == null) return;
            if (text.m_topic != "$piece_class_altar") return;
            if (__instance.IsSpawned()) return;
            __instance.m_groundObject = text.m_guidePoint.gameObject;
            __instance.transform.position = text.m_guidePoint.transform.position;

            __instance.m_currentText = text;
            __instance.m_hasTalked = false;
            __instance.m_randomTextTimer = 99999f;

            if (__instance.m_currentText.m_key.Length > 0 && Player.m_localPlayer.HaveSeenTutorial(__instance.m_currentText.m_key))
            {
                __instance.m_hasTalked = true;
            }

            Vector3 forward = (Player.m_localPlayer.transform.position - __instance.transform.position) with
            {
                y = 0.0f
            };
            forward.Normalize();
                
            __instance.transform.rotation = Quaternion.LookRotation(forward);

            if (forceTeleport)
            {
                __instance.m_animator.SetTrigger(Teleportin);
            }
            else if (text.m_static)
            {
                __instance.m_animator.SetTrigger(__instance.IsUnderRoof() ? "teleportin" : "flyin");
            }
            else
            {
                __instance.m_animator.SetTrigger(Flyin);
            }
            
        }
    }

    [HarmonyPatch(typeof(Raven), nameof(Raven.FlyAway))]
    private static class Raven_FlyAway_Prefix
    {
        private static bool Prefix(Raven __instance, bool forceTeleport = false)
        {
            if (__instance.m_currentText.m_topic != "$piece_class_altar") return true;
            if (!forceTeleport) return true;
            Chat.instance.ClearNpcText(__instance.gameObject);
            return false;
        }
    }

    [HarmonyPatch(typeof(Raven), nameof(Raven.IsSpawned))]
    private static class Raven_IsSpawned_Prefix
    {
        private static void Prefix(Raven __instance)
        {
            if (__instance.m_currentText is not { m_topic: "$piece_class_altar" }) return;
            __instance.m_randomTexts = new List<string>()
            {
                "$raven_greeting", 
                "$raven_tooltip_1", 
                "$raven_tooltip_2", 
                "$raven_tooltip_3", 
                "$raven_random_1", 
                "$raven_random_2", 
                "$raven_random_3"
            };
        }
    }
}