using AlmanacClasses.Classes;
using AlmanacClasses.FileSystem;
using HarmonyLib;

namespace AlmanacClasses.Patches;

public static class ZNetPatches
{
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Start))]
    private static class ZNet_Start_Patch
    {
        private static void Postfix(ZNet __instance)
        {
            if (!__instance) return;
            SyncManager.InitSynchronizedFiles();
        }
    }
}