using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities.Warrior;
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
            VFX.Init();
            RavenTutorialManager.Init();
            MonkeyWrench.Init();
            TalentManager.Init();
            StatusEffectManager.Init(__instance);
            ExperienceManager.LoadExperienceOrbs();
        }
    }
}