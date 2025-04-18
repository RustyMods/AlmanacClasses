﻿using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using BepInEx;

namespace AlmanacClasses.Managers;

public static class AnimationManager
{
    public static readonly List<string> animations = new()
    {   "gpower", "staff_summon", "emote_sit", 
        "emote_dance", "emote_despair", "emote_cry",
        "emote_point", "emote_flex", "emote_wave",
        "emote_challenge", "emote_cheer", "emote_nonono",
        "emote_thumbsup", "emote_blowkiss", "emote_bow",
        "emote_cower", "emote_comehere", "emote_headbang",
        "emote_kneel", "emote_laugh", "emote_roar", 
        "emote_shrug", "swing_sledge", "bow_fire",
        "spear_poke", "throw_bomb", "swing_pickaxe",
        "swing_axe0", "swing_axe1", "swing_axe2", 
        "swing_longsword0", "swing_longsword1", "swing_longsword2",
        "sword_secondary", "atgeir_attack0", "atgeir_attack1",
        "atgeir_attack2", "atgeir_secondary", "battleaxe_attack0",
        "battleaxe_attack1", "battleaxe_attack2", "battleaxe_secondary",
        "unarmed_kick", "jump", "knife_stab0", "knife_stab1",
        "knife_secondary", "mace_secondary", "dual_knives0", "dual_knives1",
        "dual_knives2", "staff_fireball0", "staff_fireball1", "equip_hip"
    };
    
    private static readonly List<string> emotes = new()
    {
        "sit", "dance", "despair", "cry", "point", 
        "flex", "wave", "challenge", "cheer", 
        "nonono", "thumbsup", "blowkiss", "bow", 
        "cower", "comehere", "headbang", "kneel",
        "laugh", "roar", "shrug"
    };

    private static bool IsEmote(string input) => emotes.Contains(input);

    public static void DoAnimation(string animation)
    {
        if (animation.IsNullOrWhiteSpace()) return;
        if (IsEmote(animation))
        {
            if (Player.m_localPlayer.InEmote()) Player.m_localPlayer.StopEmote();
            Player.m_localPlayer.StartEmote(animation);
        }
        else
        {
            Player.m_localPlayer.m_zanim.SetTrigger(animation);
        }
    }

    public static void LoadCustomAnimations()
    {
        AnimationReplaceManager.AddAnimationSet("classesbundle", "LutePlay");
        AnimationReplaceManager.AddAnimationSet("classesbundle", "StoneThrow");
        AnimationReplaceManager.AddAnimationSet("classesbundle", "Heal");
        AnimationReplaceManager.AddAnimationSet("classesbundle", "LightningStrike");
        AnimationReplaceManager.AddAnimationSet("classesbundle", "MeteorStrike");
        AnimationReplaceManager.AddAnimationSet("classesbundle", "SetTrap");
        AnimationReplaceManager.AddAnimationSet("classesbundle", "Summon");
    }
}