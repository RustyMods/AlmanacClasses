using AlmanacClasses.LoadAssets;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities;

/// <summary>
/// Added to creature when spawned, or loaded into scene, if has key saved on znetview
/// Make sure it is added after all the components are loaded (awake)
/// </summary>
[RequireComponent(typeof(Humanoid))]
[RequireComponent(typeof(ZSyncAnimation))]
[RequireComponent(typeof(ZNetView))]
[RequireComponent(typeof(MonsterAI))]
public class Friendly : MonoBehaviour
{
    public static readonly int Hash = "FriendlySpawn".GetStableHashCode();
    public static readonly int DeathDelayHash = "DeathDelay".GetStableHashCode();

    public ZNetView m_nview = null!;
    public Humanoid m_humanoid = null!;
    public Tameable m_tame = null!;
    public ZSyncAnimation m_syncAnimation = null!;
    public void Start()
    {
        m_nview = GetComponent<ZNetView>();
        m_humanoid = GetComponent<Humanoid>();
        if (!TryGetComponent(out m_tame))
        {
            m_tame = gameObject.AddComponent<Tameable>();
        }
        m_syncAnimation = GetComponent<ZSyncAnimation>();
        
        m_humanoid.m_name = m_nview.GetZDO().GetString(ZDOVars.s_tamedName, m_humanoid.m_name);
        m_humanoid.m_faction = Character.Faction.Players;
        m_humanoid.m_boss = false;
        m_tame.m_character.SetTamed(true);
        var owner = Player.GetPlayer(m_nview.GetZDO().GetLong(ZDOVars.s_owner)) ?? Player.m_localPlayer;
        if(owner) m_tame.Command(owner, false);
        m_tame.m_sootheEffect = VFX.SoothEffects.m_effectList;
        m_tame.m_unSummonEffect = VFX.UnSummonEffects.m_effectList;
        m_tame.m_levelUpOwnerSkill = Skills.SkillType.BloodMagic;
        m_tame.m_commandable = true;
        m_tame.m_startsTamed = true;
        m_syncAnimation.SetBool("wakeup", true);
        m_tame.m_monsterAI.Alert();
        m_tame.m_monsterAI.SetAggravated(true, BaseAI.AggravatedReason.Damage);
        m_tame.m_monsterAI.m_passiveAggresive = true;
        m_tame.m_monsterAI.m_attackPlayerObjects = false;
        if (TryGetComponent(out CharacterDrop characterDrop))
        {
            characterDrop.m_drops.Clear();
        }
        Invoke(nameof(SelfDestruct), m_nview.GetZDO().GetFloat(DeathDelayHash, 10f));
    }

    public void Setup()
    {
        m_humanoid.m_name = m_nview.GetZDO().GetString(ZDOVars.s_tamedName, m_humanoid.m_name);
        m_humanoid.m_faction = Character.Faction.Players;
        m_humanoid.m_boss = false;
        m_tame.m_character.SetTamed(true);
        m_tame.Command(Player.GetPlayer(m_nview.GetZDO().GetLong(ZDOVars.s_owner)) ?? Player.m_localPlayer, false);
        m_tame.m_sootheEffect = VFX.SoothEffects.m_effectList;
        m_tame.m_unSummonEffect = VFX.UnSummonEffects.m_effectList;
        m_tame.m_levelUpOwnerSkill = Skills.SkillType.BloodMagic;
        m_tame.m_commandable = true;
        m_tame.m_startsTamed = true;
        m_syncAnimation.SetBool("wakeup", true);
        m_tame.m_monsterAI.Alert();
        m_tame.m_monsterAI.SetAggravated(true, BaseAI.AggravatedReason.Damage);
        m_tame.m_monsterAI.m_passiveAggresive = true;
        m_tame.m_monsterAI.m_attackPlayerObjects = false;
        if (TryGetComponent(out CharacterDrop characterDrop))
        {
            characterDrop.m_drops.Clear();
        }
    }

    public void SelfDestruct()
    {
        m_humanoid.SetHealth(0);
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.Start))]
    private static class Humanoid_Awake_Patch
    {
        private static void Postfix(Humanoid __instance)
        {
            if (!__instance || !__instance.m_nview.IsValid() || __instance.IsPlayer()) return;
            if (!__instance.m_nview.GetZDO().GetBool(Hash)) return;
            __instance.gameObject.AddComponent<Friendly>();
        }
    }
}