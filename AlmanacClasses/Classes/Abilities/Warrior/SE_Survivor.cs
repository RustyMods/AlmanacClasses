using HarmonyLib;
using UnityEngine;

namespace AlmanacClasses.Classes.Abilities.Warrior;

public class SE_Survivor : StatusEffect
{
    private readonly string m_key = "Survivor";
    private Talent m_talent = null!;
    public override void Setup(Character character)
    {
        if (!TalentManager.m_talents.TryGetValue(m_key, out Talent talent)) return;
        m_ttl = 10f;
        m_startEffects = talent.GetEffectList();
        m_name = talent.GetName();
        m_talent = talent;
        base.Setup(character);
    }
    public override void OnDamaged(HitData hit, Character attacker)
    {
        float timePercentage = GetRemaningTime() / m_ttl;
        hit.ApplyModifier(1f - timePercentage);
    }
}

public static class Survivor
{
    [HarmonyPatch(typeof(Character), nameof(Character.GetHealth))]
    private static class Player_GetHealth_Patch
    {
        private static void Postfix(Character __instance, ref float __result)
        {
            if (!__instance) return;
            if (__instance != Player.m_localPlayer) return;
            if (__result > 0.0) return;
            if (PlayerManager.m_playerTalents.TryGetValue("Survivor", out Talent talent))
            {
                if (__instance.GetSEMan().HaveStatusEffect(talent.m_statusEffectHash)) return;
                float chance = talent.GetChance(talent.GetLevel());
                float random = Random.Range(0, 101f);
                if (random < chance)
                {
                    float quarter = __instance.GetMaxHealth() / 4f;
                    __instance.Heal(quarter);
                    __result = quarter;
                    __instance.GetSEMan().AddStatusEffect(talent.m_statusEffectHash);
                }
            }
        }
    }
}