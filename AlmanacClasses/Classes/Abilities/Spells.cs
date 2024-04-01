using System;
using System.Collections;
using System.Collections.Generic;
using AlmanacClasses.LoadAssets;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AlmanacClasses.Classes.Abilities;

public static class Spells
{
    private static readonly LayerMask m_layerMask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock");

    public static void TriggerIceBreath(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(StartIceBreath(damages));
    }
    private static IEnumerator StartIceBreath(HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(0.1f);
        Transform transform = Player.m_localPlayer.transform;
        LoadedAssets.DragonBreath.Create(Player.m_localPlayer.GetHeadPoint() + transform.forward * 3f, transform.rotation, transform);
        float timeout = 5f;
        float count = 0f;
        while (count < timeout)
        {
            Vector3 position = transform.position;
            Vector3 target = position + transform.forward * 10f;
            List<Character> characters = new();
            Character.GetCharactersInRange(target, 10f, characters);
            foreach (Character? character in characters)
            {
                if (character.IsPlayer()) continue;
                HitData hitData = new()
                {
                    m_blockable = true,
                    m_dodgeable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                    m_skillRaiseAmount = 1f,
                    m_skillLevel = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.ElementalMagic),
                    m_damage = damages,
                    m_pushForce = 100f,
                    m_ranged = true,
                    m_hitType = HitData.HitType.PlayerHit,
                    m_dir = transform.forward
                };
                hitData.SetAttacker(Player.m_localPlayer);
                character.ApplyDamage(hitData, true, true);
                if (character.m_nview && character.m_nview.IsValid())
                {
                    character.m_nview.ClaimOwnership();
                    if (character.GetSEMan().HaveStatusEffect("SE_IceBreaker".GetStableHashCode()))
                    {
                        character.GetSEMan().RemoveStatusEffect("SE_IceBreaker".GetStableHashCode());
                    }
                    character.GetSEMan().AddStatusEffect("SE_IceBreaker".GetStableHashCode());
                }
            }
            yield return new WaitForSeconds(1f);
            ++count;
        }
    }
    public static bool TriggerLightningAOE(HitData.DamageTypes damages)
    {
        Transform transform = Player.m_localPlayer.transform;
        Vector3 location = Player.m_localPlayer.GetLookDir() * 10f + transform.position;

        List<Character> characters = new();
        Character.GetCharactersInRange(location, 10f, characters);
        if (characters.Count <= 0)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No targets");
            return false;
        }
        int count = 0;
        for (int index = 0; index < characters.Count; index++)
        {
            if (index > 5) break;
            Character? character = characters[index];
            if (character.IsPlayer()) continue;
            if (character.GetFaction() is Character.Faction.Players) continue;
            AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedLightningSpawn(character.transform.position, 1f * index, damages));
            ++count;
        }

        if (count == 0)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "No targets");
            return false;
        }

        return true;
    }
    private static IEnumerator DelayedLightningSpawn(Vector3 pos, float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        GameObject spell = Object.Instantiate(LoadedAssets.lightning_AOE, pos, Quaternion.identity);
        Transform AOE_ROD = spell.transform.Find("AOE_ROD");
        if (AOE_ROD.TryGetComponent(out Aoe AOE_Component))
        {
            SetAOEComponent(AOE_Component, damages);
        }

        Transform AOE_AREA = spell.transform.Find("AOE_AREA");
        if (AOE_AREA.TryGetComponent(out Aoe aoe))
        {
            SetAOEComponent(aoe, damages);
        }
    }
    private static void SetAOEComponent(Aoe component, HitData.DamageTypes damages)
    {
        component.m_owner = Player.m_localPlayer;
        component.m_skill = Skills.SkillType.ElementalMagic;
        component.m_canRaiseSkill = true;
        
        component.m_useTriggers = true;
        component.m_triggerEnterOnly = true;
        
        component.m_damage = damages;
        component.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));
        
        component.m_blockable = true;
        component.m_dodgeable = true;
        
        component.m_hitProps = false;
        component.m_hitOwner = false;
        component.m_hitParent = false;
        component.m_hitFriendly = false;
        
    }
    public static void TriggerGoblinBeam(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(StartGoblinBeam(0.3f, damages));
    }
    private static IEnumerator StartGoblinBeam(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        
        Object.Instantiate(ZNetScene.instance.GetPrefab("sfx_goblinking_beam"), Player.m_localPlayer.transform.position, Quaternion.identity);

        int amount = 0;
        int maxAmount = 10;
        while (amount < maxAmount)
        {
            Transform transform = Player.m_localPlayer.transform;
            Vector3 position = transform.position;
            Vector3 forward = Player.m_localPlayer.GetLookDir() * 5f;
            Vector3 target = position + forward + transform.up * 1.4f;
            GameObject beam = Object.Instantiate(LoadedAssets.GoblinBeam, target, Quaternion.identity);
            if (beam.TryGetComponent(out Projectile projectile))
            {
                projectile.name = "SageBeam";
                projectile.m_ttl = 50f;
                projectile.m_rayRadius = 0.5f;
                projectile.m_aoe = 0.2f;
                projectile.m_owner = Player.m_localPlayer;
                projectile.m_skill = Skills.SkillType.ElementalMagic;
                projectile.m_raiseSkillAmount = 1f;
                projectile.transform.localRotation = Quaternion.LookRotation(forward);

                bool flag = Physics.Raycast(target, forward, out RaycastHit hit, float.PositiveInfinity, m_layerMask);
            
                Vector3 hitTarget = !flag || !hit.collider ? target + transform.forward * 1000f : hit.point;

                HitData hitData = new()
                {
                    m_damage = damages,
                    m_pushForce = 0.4f,
                    m_dodgeable = true,
                    m_blockable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                    m_skillRaiseAmount = 0.2f,
                    m_skillLevel = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.ElementalMagic)
                };
                hitData.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));
                hitData.SetAttacker(Player.m_localPlayer);
                Vector3 velocity = (hitTarget - target).normalized * 25f;
                projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);

                yield return new WaitForSeconds(delay);
                ++amount;
            }
        }
    }
    public static void TriggerMeteor(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedMeteorSpawn(1f, damages));
    }
    private static IEnumerator DelayedMeteorSpawn(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        float radius = Player.m_localPlayer.GetRadius();
        Transform transform = Player.m_localPlayer.transform;
        GameObject[] instance =  LoadedAssets.VFX_SongOfSpirit.Create(Player.m_localPlayer.GetCenterPoint(), transform.rotation, transform, radius * 2f);
        int max = 3;
        int count = 0;
        while (count < max)
        {
            Vector3 pos = transform.position + transform.up * 50f + Player.m_localPlayer.GetLookDir();
            GameObject spell = Object.Instantiate(LoadedAssets.Meteor, pos, Quaternion.identity);
            if (spell.TryGetComponent(out Projectile projectile))
            {
                projectile.name = "MeteorStrike";
                projectile.m_ttl = 60f;
                projectile.m_rayRadius = 1f;
                projectile.m_aoe = 5f;
                projectile.m_hitNoise = 100f;
                projectile.m_owner = Player.m_localPlayer;
                projectile.m_skill = Skills.SkillType.ElementalMagic;
                projectile.m_raiseSkillAmount = 1f;
                projectile.transform.localRotation = Quaternion.LookRotation(Player.m_localPlayer.GetLookDir());

                bool flag = !Physics.Raycast(Player.m_localPlayer.GetEyePoint(), Player.m_localPlayer.GetLookDir(), out RaycastHit hit, 1000f, m_layerMask) || !hit.collider;
                
                Vector3 target =  flag ? Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 1000f : hit.point;
                target += Random.insideUnitSphere * 15f;

                HitData hitData = new()
                {
                    m_damage = damages,
                    m_pushForce = 0.5f,
                    m_dodgeable = true,
                    m_blockable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                    m_skillRaiseAmount = 1f,
                    m_skillLevel = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.ElementalMagic)
                };
                hitData.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));
                hitData.SetAttacker(Player.m_localPlayer);

                Vector3 velocity = (target - pos).normalized * 25f;
                
                projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);
                
                yield return new WaitForSeconds(delay * 0.5f);
            }
            ++count;
        }

        yield return new WaitForSeconds(2f);
        foreach (GameObject effect in instance)
        {
            ZNetScene.instance.Destroy(effect);
        }
    }

    // public static bool TriggerBlink()
    // {
    //     float range = 10f;
    //     if (PlayerManager.m_playerTalents.TryGetValue("BlinkTeleport", out Talent ability)) range *= ability.m_level;
    //     bool flag = Blink(range);
    //     if (flag) Player.m_localPlayer.RaiseSkill(Skills.SkillType.ElementalMagic);
    //     return flag;
    //     // AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedBlink(2f, range));
    // }
    //
    // private static bool Blink(float range)
    // {
    //     Vector3 initialPos = Player.m_localPlayer.transform.position;
    //     Vector3 forward = Player.m_localPlayer.GetLookDir() * range;
    //     bool flag = !Physics.Raycast(Player.m_localPlayer.GetEyePoint(), Player.m_localPlayer.GetLookDir(), out RaycastHit hit, range, m_layerMask) || !hit.collider;
    //     Vector3 target = flag ? Player.m_localPlayer.GetEyePoint() + forward : hit.point;
    //
    //     if (!ZoneSystem.instance.FindFloor(target, out float height))
    //     {
    //         Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_portal_blocked");
    //         return false;
    //     }
    //     target.y = height;
    //     Player.m_localPlayer.transform.position = target;
    //     if (Vector3.Distance(Player.m_localPlayer.transform.position, initialPos) < (range / 1.5f))
    //     {
    //         return false;
    //     }
    //     Player.m_localPlayer.ResetCloth();
    //     LoadedAssets.FX_EikthyrStomp.Create(target, Player.m_localPlayer.transform.rotation);
    //     return true;
    // }
    //
    // private static IEnumerator DelayedBlink(float delay, float range)
    // {
    //     yield return new WaitForSeconds(0.1f);
    //     bool flag = !Physics.Raycast(Player.m_localPlayer.GetEyePoint(), Player.m_localPlayer.GetLookDir(), out RaycastHit hit, range, m_layerMask) || !hit.collider;
    //     
    //     Vector3 target = flag ? Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 10f : hit.point;
    //
    //     float solid = ZoneSystem.instance.GetSolidHeight(target);
    //     target.y = solid;
    //
    //     Quaternion rotation = Player.m_localPlayer.transform.rotation;
    //     if (Player.m_localPlayer.TeleportTo(target, rotation, false))
    //     {
    //         Player.m_localPlayer.RaiseSkill(Skills.SkillType.ElementalMagic, 1f);
    //     }
    //     
    //     yield return new WaitForSeconds(delay);
    //     LoadedAssets.FX_EikthyrStomp.Create(target, rotation);
    //
    // }

    public static void TriggerStoneThrow(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedStoneThrowSpawn(0.3f, damages));
    }
    
    private static IEnumerator DelayedStoneThrowSpawn(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        Transform transform = Player.m_localPlayer.transform;
        Vector3 pos = transform.position + transform.up * 5f + Player.m_localPlayer.GetLookDir();
        GameObject spell = Object.Instantiate(LoadedAssets.TrollStone, pos, Quaternion.identity);
        if (spell.TryGetComponent(out Projectile projectile))
        {
            projectile.name = "StoneThrow";
            projectile.m_ttl = 10f;
            projectile.m_rayRadius = 1f;
            projectile.m_aoe = 5f;
            projectile.m_gravity = 5f;
            projectile.m_hitNoise = 40f;
            projectile.m_owner = Player.m_localPlayer;
            projectile.m_skill = Skills.SkillType.ElementalMagic;
            projectile.m_raiseSkillAmount = 10f;
            projectile.transform.localRotation = Quaternion.LookRotation(Player.m_localPlayer.GetLookDir());

            bool flag = !Physics.Raycast(Player.m_localPlayer.GetEyePoint(), Player.m_localPlayer.GetLookDir(), out RaycastHit hit, 1000f, m_layerMask) || !hit.collider;

            Vector3 target = flag
                ? Player.m_localPlayer.GetEyePoint() + Player.m_localPlayer.GetLookDir() * 1000f
                : hit.point;

            HitData hitData = new()
            {
                m_damage = damages,
                m_pushForce = 10f,
                m_dodgeable = true,
                m_blockable = true,
                m_skill = Skills.SkillType.ElementalMagic,
                m_skillRaiseAmount = 1f,
                m_skillLevel = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.ElementalMagic)
            };
            hitData.SetAttacker(Player.m_localPlayer);
            hitData.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));

            Vector3 velocity = (target - pos).normalized * 25f;

            projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);
        }
    }

    public static void TriggerRootBeam(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(StartRootBeam(0.3f, damages));
    }

    private static IEnumerator StartRootBeam(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(delay);
        
        int amount = 0;
        int maxAmount = 10;
        while (amount < maxAmount)
        {
            Transform transform = Player.m_localPlayer.transform;
            Vector3 position = transform.position;
            Vector3 forward = Player.m_localPlayer.GetLookDir() * 5f;
            Vector3 target = position + forward + transform.up * 1.4f;
            GameObject beam = Object.Instantiate(LoadedAssets.GDKingRoots, target, Quaternion.identity);
            if (beam.TryGetComponent(out Projectile projectile))
            {
                projectile.name = "RootBeam";
                projectile.m_ttl = 50f;
                projectile.m_rayRadius = 0.5f;
                projectile.m_aoe = 0.2f;
                projectile.m_owner = Player.m_localPlayer;
                projectile.m_skill = Skills.SkillType.ElementalMagic;
                projectile.m_raiseSkillAmount = 1f;
                projectile.transform.localRotation = Quaternion.LookRotation(forward);

                RaycastHit hit;
                bool flag = Physics.Raycast(target, forward, out hit, float.PositiveInfinity, m_layerMask);
            
                Vector3 hitTarget = !flag || !hit.collider ? target + transform.forward * 1000f : hit.point;

                HitData hitData = new()
                {
                    m_damage = damages,
                    m_pushForce = 0.4f,
                    m_dodgeable = true,
                    m_blockable = true,
                    m_skill = Skills.SkillType.ElementalMagic,
                    m_skillRaiseAmount = 0.2f
                };
                hitData.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.ElementalMagic), 0.1f, 1f));
                hitData.SetAttacker(Player.m_localPlayer);
                
                Vector3 velocity = (hitTarget - target).normalized * 25f;
                projectile.Setup(Player.m_localPlayer, velocity, -1f, hitData, null, null);

                yield return new WaitForSeconds(delay);
                ++amount;
            }
        }
    }

    public static void TriggerSpawnTrap(HitData.DamageTypes damages)
    {
        AlmanacClassesPlugin._Plugin.StartCoroutine(DelayedTrap(10f, damages));
        Player.m_localPlayer.RaiseSkill(Skills.SkillType.Crossbows);
    }

    private static IEnumerator DelayedTrap(float delay, HitData.DamageTypes damages)
    {
        yield return new WaitForSeconds(1f);
        Vector3 location = Player.m_localPlayer.transform.position;
        LoadedAssets.TrapArmedEffects.Create(location, Quaternion.identity);
        GameObject trap = Object.Instantiate(LoadedAssets.CustomTrap, location, Quaternion.identity);
        if (trap.TryGetComponent(out ZNetView zNetView))
        {
            if (zNetView.IsValid()) zNetView.GetZDO().Persistent = false;
        }
        if (trap.TryGetComponent(out Trap component))
        {
            component.m_aoe.m_damage = damages;
            component.m_aoe.m_damage.Modify(Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(Skills.SkillType.Bows),0.1f, 1f));
            component.m_triggeredByPlayers = false;
            component.m_aoe.m_hitFriendly = false;
            component.m_aoe.m_owner = Player.m_localPlayer;
            component.m_startsArmed = true;
        }

        yield return new WaitForSeconds(delay);
        if (trap.TryGetComponent(out WearNTear wearNTear))
        {
            wearNTear.Destroy();
        }
    }

    public static void TriggerBleeding(Character __instance)
    {
        if (__instance.GetSEMan().HaveStatusEffect("SE_Bleed".GetStableHashCode()))
        {
            StatusEffect bleed = __instance.GetSEMan().GetStatusEffect("SE_Bleed".GetStableHashCode());
            bleed.ResetTime();
            SE_Bleed? seBleed = bleed as SE_Bleed;
            if (seBleed != null)
            {
                ++seBleed.m_stack;
            }
        }
        else
        {
            __instance.GetSEMan().AddStatusEffect("SE_Bleed".GetStableHashCode());
        }
    }
    public static void TriggerHeal(float amount, float range = 10f)
    {
        List<Player> players = new();
        Player.GetPlayersInRange(Player.m_localPlayer.transform.position, range, players);
        float value = ApplySkill(Skills.SkillType.BloodMagic, amount);
        foreach (Player player in players)
        {
            if (player.GetSEMan().HaveStatusEffect("SE_Heal".GetStableHashCode()))
            {
                player.GetSEMan().RemoveStatusEffect("SE_Heal".GetStableHashCode());
            }
            player.GetSEMan().AddStatusEffect("SE_Heal".GetStableHashCode());
            player.Heal(value);
        }
    }

    public static float ApplySkill(Skills.SkillType type, float amount)
    {
        return type is Skills.SkillType.None ? amount : Mathf.Clamp(amount * Mathf.Clamp(Player.m_localPlayer.GetSkillFactor(type), 0.1f, 1f), 1, amount);
    }
}