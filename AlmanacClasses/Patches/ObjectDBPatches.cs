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

            StatusEffect monkeyWrench = ScriptableObject.CreateInstance<SE_MonkeyWrench>();
            monkeyWrench.name = "SE_MonkeyWrench";
            monkeyWrench.m_name = "Monkey Wrench";
            monkeyWrench.m_icon = SpriteManager.HourGlass_Icon;
            monkeyWrench.m_tooltip = "Reduces damage output";
            if (!__instance.m_StatusEffects.Contains(monkeyWrench))
            {
                __instance.m_StatusEffects.Add(monkeyWrench);
            }

            StatusEffect dualWield = ScriptableObject.CreateInstance<SE_DualWield>();
            dualWield.name = "SE_DualWield";
            dualWield.m_name = "Dual Wield";
            dualWield.m_icon = SpriteManager.HourGlass_Icon;
            dualWield.m_tooltip = "Reduces damage output";
            if (!__instance.m_StatusEffects.Contains(dualWield))
            {
                __instance.m_StatusEffects.Add(dualWield);
            }

            StatusEffect survivor = ScriptableObject.CreateInstance<SE_Survivor>();
            survivor.name = "SE_Survivor";
            survivor.m_name = "$talent_survivor";
            survivor.m_tooltip = "Reduces incoming damage";
            survivor.m_icon = SpriteManager.HourGlass_Icon;
            survivor.m_ttl = 100f;
            survivor.m_startEffects = LoadedAssets.VFX_SongOfSpirit;

            if (!__instance.m_StatusEffects.Contains(survivor))
            {
                __instance.m_StatusEffects.Add(survivor);
            }

            StatusEffect battleFury = ScriptableObject.CreateInstance<StatusEffect>();
            battleFury.name = "SE_BattleFury";
            battleFury.m_name = "Battle Fury";
            battleFury.m_ttl = 2f;
            battleFury.m_startEffects = LoadedAssets.FX_BattleFury;

            if (!__instance.m_StatusEffects.Contains(battleFury))
            {
                __instance.m_StatusEffects.Add(battleFury);
            }
            
            ExperienceManager.CreateExperienceOrb(10, "ExperienceOrb_Simple", "Simple Orb", new Color(1f, 0.9f, 0f, 1f), new Color32(255, 0, 0, 255), new Color(1f, 0.5f, 0.5f, 0.6f), new Color(1f, 0.7f, 0.5f, 1f), SpriteManager.HourGlass_Icon);
            ExperienceManager.CreateExperienceOrb(25, "ExperienceOrb_Magic", "Magic Orb", new Color(0.3f, 1f, 0f, 1f), new Color32(255, 255, 0, 255), new Color(0f, 0.5f, 0.5f, 0.6f), new Color(0.5f, 1f, 0f, 1f), SpriteManager.HourGlass_Icon);
            ExperienceManager.CreateExperienceOrb(50, "ExperienceOrb_Epic", "Epic Orb", new Color(0f, 0.2f, 0.8f, 1f), new Color32(150, 0, 250, 255), new Color(0.8f, 0f, 0.5f, 0.6f), new Color(1f, 0.7f, 0.5f, 1f), SpriteManager.HourGlass_Icon);
            ExperienceManager.CreateExperienceOrb(100, "ExperienceOrb_Legendary", "Legendary Orb", new Color(1f, 0.9f, 1f, 1f), new Color32(150, 150, 255, 255), new Color(0.6f, 1f, 1f, 0.6f), new Color(0.5f, 0.7f, 1f, 1f), SpriteManager.HourGlass_Icon);
            // ExperienceManager.CreateExperienceOrb(150, "ExperienceOrb_Plains", "Goblin Orb", new Color(1f, 0.9f, 0.4f, 1f), new Color32(255, 255, 0, 255), new Color(0.5f, 1f, 0.5f, 0.6f), new Color(0.5f, 0.7f, 0.5f, 1f));
            // ExperienceManager.CreateExperienceOrb(300, "ExperienceOrb_Mistlands", "Runic Orb", new Color(0f, 0.9f, 1f, 1f), new Color32(100, 150, 200, 255), new Color(0f, 0.5f, 1f, 0.6f), new Color(0f, 0.7f, 0.5f, 1f));
        }
    }
}