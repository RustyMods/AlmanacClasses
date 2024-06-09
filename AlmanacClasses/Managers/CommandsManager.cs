using System;
using System.Collections.Generic;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities.Ranger;
using AlmanacClasses.Data;
using AlmanacClasses.UI;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Managers;

public static class CommandsManager
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
    private static class Terminal_Awake_Patch
    {
        private static void Postfix()
        {
            AddCommands();
        }
    }

    private static void AddCommands()
    {
        Terminal.ConsoleCommand ClassesCommands = new("class_talents", "Almanac Class System Commands", (Terminal.ConsoleEventFailable)(args =>
        {
            if (args.Length < 2) return false;
            switch (args[1])
            {
                case "reset":
                    PlayerManager.m_tempPlayerData.m_experience = 0;
                    PlayerManager.m_tempPlayerData.m_boughtTalents.Clear();
                    PlayerManager.m_playerTalents.Clear();
                    CharacteristicManager.m_tempCharacteristics = new(DefaultData.defaultCharacteristics);
                    SpellBook.DestroyElements();
                    SpellBook.m_abilities.Clear();
                    LoadUI.ResetTalents(true);
                    TalentManager.InitializeTalents();
                    AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Reset all classes data");
                    break;
                
                case "help":
                    List<string> info = new()
                    {
                        "reset: completely resets local player talent data",
                    };
                    foreach(string text in info) AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(text);
                    break;
            }
    
            return true;
        }),optionsFetcher:()=>new(){"help", "reset"});
    
        Terminal.ConsoleCommand WriteFiles = new("talents_write", "Almanac Class System File Commands", (Terminal.ConsoleEventFailable)(args =>
        {
            if (args.Length < 2) return false;
            switch (args[1])
            {
                case "experience":
                    ExperienceManager.WriteExperienceMap();
                    break;
                case "help":
                    List<string> info = new()   
                    {
                        "experience : writes current experience map to file to tweak"
                    };
                    foreach (string text in info)
                    {
                        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(text);
                    }
                    break;
            }
            return true;
        }),optionsFetcher:()=>new(){"help","experience"});
    
        Terminal.ConsoleCommand AdminCommands = new("talents_test", "Almanac Class System Admin only commands",
            (Terminal.ConsoleEventFailable)(args =>
            {
                if (args.Length < 2) return false;
                switch (args[1])
                {
                    case "help":
                        List<string> info = new()
                        {
                            "spawn [prefab] [level]: Spawns a friendly creature, no cost must be enabled",
                            "experience [amount]: Increases experience, no cost must be enabled",
                            "emote [name]: runs custom emote, if command emote list, it prints available emotes"
                        };
                        foreach (string text in info)
                        {
                            AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(text);
                        }
                        break;
                    case "emote":
                        if (args[2] == "list")
                        {
                            if (!Player.m_localPlayer.TryGetComponent(out ZSyncAnimation zSyncAnimation))
                            {
                                AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Failed to get local player z sync animation");
                                return false;
                            }
    
                            RuntimeAnimatorController? controller = zSyncAnimation.m_animator.runtimeAnimatorController;
                            foreach (AnimationClip? animation in controller.animationClips)
                            {
                                AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(animation.name);
                            }
                        }
                        Player.m_localPlayer.m_zanim.SetTrigger(args[2]);
                        break;
                    case "experience":
                        if (!Player.m_localPlayer.NoCostCheat())
                        {
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No cost must be enabled");
                            return false;
                        }
                        if (!int.TryParse(args[2], out int amount)) return false;
                        PlayerManager.m_tempPlayerData.m_experience += amount;
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Increased experience by " + amount);
                        break;
                    case "spawn":
                        if (!Player.m_localPlayer.NoCostCheat())
                        {
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No cost must be enabled");
                            return false;
                        }
                        GameObject creature = ZNetScene.instance.GetPrefab(args[2]);
                        if (!creature) return false;
                        if (!int.TryParse(args[3], out int level)) return false;
                        if (!creature.GetComponent<MonsterAI>()) return false;
                        RangerSpawn.TriggerHunterSpawn(creature, level);
                        break;
                }
    
                return true;
            }), optionsFetcher: ()=>new List<string>()
            {
                "help", "spawn", "emote", "experience"
            }, isSecret: true, onlyAdmin: true);
    }
}