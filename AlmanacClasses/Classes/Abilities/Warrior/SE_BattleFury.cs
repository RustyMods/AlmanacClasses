using AlmanacClasses.Managers;
using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_BattleFury : StatusEffect
{
    private readonly string m_key = "BattleFury";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = talent.GetLength(talent.GetLevel());
        m_startEffects = talent.GetEffectList();
        m_talent = talent;
        base.Setup(character);
    }
}

public static class BattleFury
{
    public static void CheckBattleFury(Character instance)
    {
        if (!PlayerManager.m_playerTalents.TryGetValue("BattleFury", out Talent talent)) return;

        if (instance.m_lastHit == null) return;
        if (instance.m_lastHit.GetAttacker() == null) return;
        if (instance.m_lastHit.GetAttacker() != Player.m_localPlayer) return;
        
        float chance = talent.GetChance(talent.GetLevel());
        float random = Random.Range(0, 101);
        if (random > chance) return;
        var amount = talent.GetStamina(talent.GetLevel());
        Player.m_localPlayer.AddStamina(amount);
        
        Transform transform = Player.m_localPlayer.transform;
        DisplayText.ShowText(Color.yellow, Player.m_localPlayer.GetTopPoint(), $"+{amount} $se_stamina");
        talent.GetEffectList().Create(transform.position, transform.rotation, transform);
    }
}