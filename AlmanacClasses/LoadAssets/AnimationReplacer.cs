using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace KG_Managers
{
    public static class AnimationReplaceManager
    {
        private static bool FirstInit;
        private static RuntimeAnimatorController VanillaController;
        private static readonly Dictionary<string, KeyValuePair<RuntimeAnimatorController, string>> Controllers = new();
        private static readonly Dictionary<string, AnimationClip> AllExternalAnimations = new();
        private static readonly List<List<string>> AllAnimationSets = new();
        private static string _asmName;
        private static string AssemblyName() => _asmName ??= Assembly.GetExecutingAssembly().GetName().Name;

        private static AssetBundle GetAssetBundle(string filename)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));
            using Stream? stream = execAssembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(stream);
        }
        
        public static void AddAnimationSet(string AssetBundle, string attack1, string attack2 = null, string attack3 = null)
        {
            UnityEngine.AssetBundle asset =
                UnityEngine.AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.name == AssetBundle) ??
                GetAssetBundle(AssetBundle);
            var newSet = new List<string> { attack1 };
            AllExternalAnimations[attack1] = asset.LoadAsset<AnimationClip>(attack1);
            if (attack2 != null) newSet.Add(attack2);
            if (attack3 != null) newSet.Add(attack3);
            if (attack2 != null) AllExternalAnimations[attack2] = asset.LoadAsset<AnimationClip>(attack2);
            if (attack3 != null) AllExternalAnimations[attack3] = asset.LoadAsset<AnimationClip>(attack3);
            AllAnimationSets.Add(newSet);
        }

        public static void AddAnimationSet(AnimationClip attack1, AnimationClip attack2 = null,
            AnimationClip attack3 = null)
        {
            var newSet = new List<string> { attack1.name };
            AllExternalAnimations[attack1.name] = attack1;
            if (attack2 != null) newSet.Add(attack2.name);
            if (attack3 != null) newSet.Add(attack3.name);
            if (attack2 != null) AllExternalAnimations[attack2.name] = attack2;
            if (attack3 != null) AllExternalAnimations[attack3.name] = attack3;
            AllAnimationSets.Add(newSet);
        }

        private static void ReplacePlayerRAC(Animator anim, RuntimeAnimatorController rac)
        {
            if (anim.runtimeAnimatorController == rac) return;
            anim.runtimeAnimatorController = rac;
            anim.Update(0f);
        }

        private static RuntimeAnimatorController MakeAOC(Dictionary<string, string> replacement,
            RuntimeAnimatorController ORIGINAL)
        {
            var aoc = new AnimatorOverrideController(ORIGINAL);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var animation in aoc.animationClips)
            {
                string name = animation.name;
                if (replacement.TryGetValue(name, out string value))
                {
                    var newClip = UnityEngine.Object.Instantiate(AllExternalAnimations[value]);
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, newClip));
                }
                else
                {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animation, animation));
                }
            }

            aoc.ApplyOverrides(anims);
            return aoc;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        [HarmonyPriority(10000)]
        private static class Player_Start_Patch
        {
            private static void Postfix(ref Player __instance)
            {
                if (FirstInit) return;
                FirstInit = true;
                VanillaController = MakeAOC(new(), __instance.m_animator.runtimeAnimatorController);
                VanillaController.name = "VanillaController";
                foreach (var animationSet in AllAnimationSets)
                {
                    Dictionary<string, string> replacementMap = new();
                    for (var i = 0; i < animationSet.Count; i++)
                        replacementMap.Add($"Attack{i + 1}", animationSet[i]);
                    RuntimeAnimatorController controller =
                        MakeAOC(replacementMap, __instance.m_animator.runtimeAnimatorController);
                    controller.name = AssemblyName();
                    for (var i = 0; i < animationSet.Count; i++)
                    {
                        Controllers[animationSet[i]] =
                            new KeyValuePair<RuntimeAnimatorController, string>(controller, $"swing_longsword{i}");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.RPC_SetTrigger))]
        static class ZSyncAnimation_RPC_SetTrigger_Patch
        {
            static bool Prefix(ZSyncAnimation __instance, string name)
            {
                if (Controllers.TryGetValue(name, out var controller))
                {
                    ReplacePlayerRAC(__instance.m_animator, controller.Key);
                    __instance.m_animator.SetTrigger(controller.Value);
                    return false;
                }

                if (__instance.m_animator.runtimeAnimatorController.name == AssemblyName())
                    ReplacePlayerRAC(__instance.m_animator, VanillaController);
                return true;
            }
        }
    }
}