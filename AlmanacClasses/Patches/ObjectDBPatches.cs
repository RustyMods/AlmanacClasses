using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Patches;

public static class ObjectDBPatches
{
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class ObjectDB_Awake_Patch
    {
        private static void Postfix(ObjectDB __instance)
        {
            if (!__instance) return;
            if (!ZNetScene.instance) return;
            SpriteManager.LoadSpriteResources();
            LoadedAssets.InitVFX();
            CharacteristicManager.InitCharacteristics(out StatusEffect? effect);
            RavenTutorialManager.InitCustomTutorials();
            LoadTwoHanded.InitTwoHandedWeapons();
            
            StatusEffect bleed = ScriptableObject.CreateInstance<SE_Bleed>();
            bleed.name = "SE_Bleed";
            bleed.m_ttl = 10f;
            bleed.m_name = "Bleeding";
            bleed.m_icon = SpriteManager.Bleeding_Icon;
            bleed.m_startEffects = LoadedAssets.BleedEffects;
            if (!__instance.m_StatusEffects.Contains(bleed))
            {
                __instance.m_StatusEffects.Add(bleed);
            }

            StatusEffect heal = ScriptableObject.CreateInstance<StatusEffect>();
            heal.name = "SE_Heal";
            heal.m_ttl = 10f;
            heal.m_name = "Shaman Heal";
            heal.m_startEffects = LoadedAssets.FX_HealthPotion;

            if (!__instance.m_StatusEffects.Contains(heal))
            {
                __instance.m_StatusEffects.Add(heal);
            }

            StatusEffect ice = ScriptableObject.CreateInstance<StatusEffect>();
            ice.name = "SE_IceBreaker";
            ice.m_ttl = 10f;
            ice.m_name = "Iced";
            ice.m_startEffects = LoadedAssets.DragonBreathHit;
            if (!__instance.m_StatusEffects.Contains(ice))
            {
                __instance.m_StatusEffects.Add(ice);
            }

            if (LoadedAssets.SE_Finder != null)
            {
                StatusEffect ping = ScriptableObject.CreateInstance<SE_Hunter>();
                ping.name = "SE_Hunter";
                ping.m_name = "Hunted";
                ping.m_startEffects = LoadedAssets.SE_Finder.m_pingEffectMed;
                ping.m_ttl = 10f;
                if (!__instance.m_StatusEffects.Contains(ping))
                {
                    __instance.m_StatusEffects.Add(ping);
                }
            }
        }
    }
}