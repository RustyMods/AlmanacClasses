using AlmanacClasses.Classes;
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
            if (!__instance || !ZNetScene.instance) return;
            SpriteManager.LoadSpriteResources();
            LoadedAssets.InitVFX();
            RavenTutorialManager.InitCustomTutorials();
            LoadTwoHanded.InitTwoHandedWeapons();
            StatusEffectManager.InitStatusEffects(__instance);
            
            ExperienceManager.CreateExperienceOrb(10, "ExperienceOrb_Simple", "Simple Orb", new Color(1f, 0.9f, 0f, 1f), new Color32(255, 0, 0, 255), new Color(1f, 0.5f, 0.5f, 0.6f), new Color(1f, 0.7f, 0.5f, 1f), SpriteManager.HourGlass_Icon);
            ExperienceManager.CreateExperienceOrb(25, "ExperienceOrb_Magic", "Magic Orb", new Color(0.3f, 1f, 0f, 1f), new Color32(255, 255, 0, 255), new Color(0f, 0.5f, 0.5f, 0.6f), new Color(0.5f, 1f, 0f, 1f), SpriteManager.HourGlass_Icon);
            ExperienceManager.CreateExperienceOrb(50, "ExperienceOrb_Epic", "Epic Orb", new Color(0f, 0.2f, 0.8f, 1f), new Color32(150, 0, 250, 255), new Color(0.8f, 0f, 0.5f, 0.6f), new Color(1f, 0.7f, 0.5f, 1f), SpriteManager.HourGlass_Icon);
            ExperienceManager.CreateExperienceOrb(100, "ExperienceOrb_Legendary", "Legendary Orb", new Color(1f, 0.9f, 1f, 1f), new Color32(150, 150, 255, 255), new Color(0.6f, 1f, 1f, 0.6f), new Color(0.5f, 0.7f, 1f, 1f), SpriteManager.HourGlass_Icon);
            // ExperienceManager.CreateExperienceOrb(150, "ExperienceOrb_Plains", "Goblin Orb", new Color(1f, 0.9f, 0.4f, 1f), new Color32(255, 255, 0, 255), new Color(0.5f, 1f, 0.5f, 0.6f), new Color(0.5f, 0.7f, 0.5f, 1f));
            // ExperienceManager.CreateExperienceOrb(300, "ExperienceOrb_Mistlands", "Runic Orb", new Color(0f, 0.9f, 1f, 1f), new Color32(100, 150, 200, 255), new Color(0f, 0.5f, 1f, 0.6f), new Color(0f, 0.7f, 0.5f, 1f));

            TalentManager.InitializeTalents();
        }
    }
}