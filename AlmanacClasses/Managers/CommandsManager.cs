using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities.Ranger;
using AlmanacClasses.LoadAssets;
using AlmanacClasses.UI;
using BepInEx;
using HarmonyLib;

namespace AlmanacClasses.Managers;

public static class CommandsManager
{
    private static readonly Dictionary<string, TalentCommand> m_commands = new();

    [HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
    private static class Terminal_Awake_Patch
    {
        private static void Postfix()
        {
            Terminal.ConsoleCommand _ = new Terminal.ConsoleCommand("talents", "use help to list out commands",
                (Terminal.ConsoleEventFailable)(
                    args =>
                    {
                        if (args.Length < 2) return false;
                        if (!m_commands.TryGetValue(args[1], out TalentCommand command)) return false;
                        return command.Run(args);
                    }), optionsFetcher: () => m_commands.Keys.ToList());

            TalentCommand reset = new TalentCommand("reset", "resets all player almanac class talent data", _ =>
            {
                PlayerManager.m_tempPlayerData.m_experience = 0;
                ExperienceBar.UpdateExperienceBar();
                PlayerManager.m_tempPlayerData.m_boughtTalents.Clear();
                PlayerManager.m_playerTalents.Clear();
                CharacteristicManager.ResetCharacteristics();
                SpellBook.ClearSpellBook();
                SpellBook.m_abilities.Clear();
                LoadUI.ResetTalents(true);
                TalentManager.InitializeTalents();
                AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Reset all classes data");
                return true;
            });
            TalentCommand help = new TalentCommand("help", "list of available talent commands", _ =>
            {
                foreach (TalentCommand command in m_commands.Values)
                {
                    if (command.m_isSecret) continue;
                    AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo($"{command.m_input} - {command.m_description}");
                }

                return true;
            });
            TalentCommand write = new TalentCommand("write", "Saves experience map to disk", _ =>
            {
                ExperienceManager.WriteExperienceMap();
                return true;
            });
            TalentCommand add = new TalentCommand("add", "[amount<int>] adds experience to local player", args =>
            {
                if (!Player.m_localPlayer.NoCostCheat())
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No cost must be enabled");
                    return false;
                }

                if (args.Length < 3) return false;
                if (!int.TryParse(args[2], out int amount)) return false;
                PlayerManager.m_tempPlayerData.m_experience += amount;
                ExperienceBar.UpdateExperienceBar();
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Increased experience by " + amount);
                return true;
            });
            TalentCommand spawn = new TalentCommand("spawn", "[prefabName<string>] [level<int>] spawns a friendly creature, no cost must be enabled",
                args =>
                {
                    if (!Player.m_localPlayer.NoCostCheat())
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No cost must be enabled");
                        return false;
                    }

                    if (ZNetScene.instance.GetPrefab(args[2]) is not { } creature) return false;
                    if (!int.TryParse(args[3], out int level)) return false;
                    if (!creature.GetComponent<MonsterAI>()) return false;
                    RangerSpawn.TriggerHunterSpawn(creature, level);
                    return true;
                }, optionsFetcher: () =>
                {
                    if (!ZNetScene.instance) return new();
                    List<string> list = (from prefab in ZNetScene.instance.m_prefabs
                        where prefab.GetComponent<Character>()
                        select prefab.name).ToList();
                    list.Sort();
                    return list;
                });
            TalentCommand emote = new TalentCommand("emote", "[name<string>] runs custom emote, if you use talents emote list, it will print the list of available emotes",
                args =>
                {
                    if (args[2] == "list")
                    {
                        List<string> animations = new();
                        animations.AddRange(AnimationManager.animations);
                        animations.Sort();
                        foreach (var list in AnimationReplaceManager.AllAnimationSets)
                        {
                            animations.AddRange(list);
                        }

                        foreach (string animation in animations)
                        {
                            AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(animation);
                        }
                    }
                    else
                    {
                        Player.m_localPlayer.m_zanim.SetTrigger(args[2]);
                    }

                    return true;
                }, optionsFetcher: () =>
                {
                    if (!Player.m_localPlayer) return new();
                    List<string> list = new();
                    list.AddRange(AnimationManager.animations);
                    foreach (var animationList in AnimationReplaceManager.AllAnimationSets)
                    {
                        list.AddRange(animationList);
                    }
                    list.Add("list");
                    list.Sort();
                    return list;
                });
            TalentCommand give = new TalentCommand("give", "[playerName<string>] [amount<int>] gives player experience, admin only, no cost must be enabled",
                args =>
                {
                    if (!Player.m_localPlayer.NoCostCheat())
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No cost must be enabled");
                        return false;
                    }

                    if (args.Length < 3) return false;
                    string name = string.Join(" ", args.Args.Skip(2).Take(args.Args.Length - 3));

                    if (Player.GetAllPlayers().FirstOrDefault(play => play.GetHoverName() == name) is not { } player)
                    {
                        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo(
                            "Failed to find player matching name: " + name);
                        return false;
                    }

                    if (!int.TryParse(args[args.Length - 1], out int amount)) return false;
                    ExperienceManager.Command_GiveExperience(player, amount);
                    AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Gave " + player.GetHoverName() + " " + amount +
                                                                      " experience");
                    return true;
                }, optionsFetcher: () =>
                {
                    var list = Player.GetAllPlayers().Select(player => player.GetHoverName()).ToList();
                    list.Sort();
                    return list;
                });
            TalentCommand size = new TalentCommand("size",
                "prints to console the kilobyte size of almanac class system data saved on player file",
                _ =>
                {
                    if (!Player.m_localPlayer) return false;
                    if (!Player.m_localPlayer.m_customData.TryGetValue(PlayerManager.m_playerDataKey, out string data))
                    {
                        AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("No classes data found");
                        return true;
                    }

                    int size = Encoding.UTF8.GetByteCount(data);
                    double kilobytes = size / 1024.0;
                    AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Almanac Classes data size: " + kilobytes + " kilobytes");
                    return true;
                });
            TalentCommand save = new TalentCommand("save", "saves almanac class data to player", _ =>
            {
                PlayerManager.SavePlayerData();
                AlmanacClassesPlugin.AlmanacClassesLogger.LogInfo("Client: Saved player data");
                return true;
            });
        }
    }

    [HarmonyPatch(typeof(Terminal), nameof(Terminal.updateSearch))]
    private static class Terminal_UpdateSearch_Patch
    {
        private static bool Prefix(Terminal __instance, string word)
        {
            if (__instance.m_search == null) return true;
            string[] strArray = __instance.m_input.text.Split(' ');
            if (strArray.Length < 3) return true;
            if (strArray[0] != "talents") return true;
            return HandleSearch(__instance, word, strArray);
        }
        
        private static bool HandleSearch(Terminal __instance, string word, string[] strArray)   
        {
            if (!m_commands.TryGetValue(strArray[1], out TalentCommand command)) return true;
            if (command.HasOptions() && strArray.Length == 3)
            {
                List<string> list = command.FetchOptions();
                List<string> filteredList;
                string currentSearch = strArray[2];
                if (!currentSearch.IsNullOrWhiteSpace())
                {
                    int indexOf = list.IndexOf(currentSearch);
                    filteredList = indexOf != -1 ? list.GetRange(indexOf, list.Count - indexOf) : list;
                    filteredList = filteredList.FindAll(x => x.ToLower().Contains(currentSearch.ToLower()));
                }
                else filteredList = list;
                if (filteredList.Count <= 0) __instance.m_search.text = command.m_description;
                else
                {
                    __instance.m_lastSearch.Clear();
                    __instance.m_lastSearch.AddRange(filteredList);
                    __instance.m_lastSearch.Remove(word);
                    __instance.m_search.text = "";
                    int maxShown = 10;
                    int count = Math.Min(__instance.m_lastSearch.Count, maxShown);
                    for (int index = 0; index < count; ++index)
                    {
                        string text = __instance.m_lastSearch[index];
                        __instance.m_search.text += text + " ";
                    }

                    if (__instance.m_lastSearch.Count <= maxShown) return false;
                    int remainder = __instance.m_lastSearch.Count - maxShown;
                    __instance.m_search.text += $"... {remainder} more.";
                }
            }
            else __instance.m_search.text = command.m_description;
                
            return false;
        }
    }

    private class TalentCommand
    {
        public readonly string m_input;
        public readonly string m_description;
        public readonly bool m_isSecret;
        private readonly Func<Terminal.ConsoleEventArgs, bool> m_command;
        private readonly Func<List<string>>? m_optionFetcher;
        public bool Run(Terminal.ConsoleEventArgs args) => m_command(args);
        public List<string> FetchOptions() => m_optionFetcher == null ? new() :  m_optionFetcher();
        public bool HasOptions() => m_optionFetcher != null;
        public TalentCommand(string input, string description, Func<Terminal.ConsoleEventArgs, bool> command, Func<List<string>>? optionsFetcher = null, bool isSecret = false)
        {
            m_input = input;
            m_description = description;
            m_command = command;
            m_isSecret = isSecret;
            m_commands[m_input] = this;
            m_optionFetcher = optionsFetcher;
        }
    }
}