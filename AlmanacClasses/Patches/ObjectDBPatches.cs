using AlmanacClasses.Classes;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.Managers;
using HarmonyLib;
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
            ExperienceManager.LoadExperienceOrbs();
            TalentManager.InitializeTalents();
        }
    }
}