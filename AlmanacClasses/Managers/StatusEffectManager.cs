using System;
using System.Collections.Generic;
using System.Text;
using AlmanacClasses.Classes;
using AlmanacClasses.Classes.Abilities;
using AlmanacClasses.Data;
using AlmanacClasses.LoadAssets;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace AlmanacClasses.Managers;

public abstract class StatusEffectManager
{
    public class Data
    {
        public Talent talent = null!;
        public string name = null!;
        public string m_name = "";
        public Sprite m_icon = null!;
        public EffectList m_startEffects = new();
        public bool m_isCharacteristic;

        public bool Init(out StatusEffect? statusEffect)
        {
            statusEffect = null;
            if (!ObjectDB.instance) return false;
            ObjectDB _objectDB = ObjectDB.instance;
            StatusEffect? possible = _objectDB.m_StatusEffects.Find(effect => effect.name == name);
            if (possible)
            {
                Player? localPlayer = Player.m_localPlayer;
                if (localPlayer)
                {
                    if (localPlayer.GetSEMan().HaveStatusEffect(possible.name))
                    {
                        localPlayer.GetSEMan().RemoveStatusEffect(possible);
                    }
                }
                _objectDB.m_StatusEffects.Remove(possible);
            }

            TalentEffect effect = ScriptableObject.CreateInstance<TalentEffect>();
            effect.data = this;
            effect.name = name;
            effect.m_name = m_name;
            effect.m_icon = m_isCharacteristic ? null : m_icon;
            effect.m_ttl = talent.m_duration?.Value ?? 0f;
            effect.m_tooltip = talent.m_description;
            effect.m_startEffects = m_startEffects;
            effect.m_attributes = talent.m_attribute;
            
            _objectDB.m_StatusEffects.Add(effect);
            statusEffect = effect;
            return true;
        }

        public class TalentEffect : StatusEffect
        {
            public Data data = null!;
            private const float m_carryWeightRatio = 1f;
            private float m_absorbDamage;
            private float m_damage;
            private float areaTimer;
            private float healTimer;

            private bool ShouldAffect(StatusEffectData.Modifier modifier)
            {
                if (!data.talent.m_modifiers.TryGetValue(modifier, out ConfigEntry<float> config)) return false;
                return Math.Abs(config.Value - DefaultData.defaultModifiers[modifier]) > 0.009f;
            }

            private void DamageArea(float dt)
            {
                areaTimer += dt;
                if (areaTimer < data.talent.m_damageInterval) return;
                areaTimer = 0f;
                List<Character> characters = new();
                Character.GetCharactersInRange(Player.m_localPlayer.transform.position, data.talent.m_radius, characters);
                foreach (Character character in characters)
                {
                    if (character.IsPlayer()) continue;
                    HitData hitData = new HitData()
                    {
                        m_ranged = true,
                        m_hitType = HitData.HitType.PlayerHit,
                        m_skill = Skills.SkillType.ElementalMagic,
                        m_skillRaiseAmount = 0.2f,
                        m_blockable = false,
                        m_dodgeable = false,
                        m_damage = TalentManager.GetDamages(data.talent),
                    };
                    hitData.SetAttacker(m_character);
                    character.ApplyDamage(hitData, true, true);
                }
            }

            public override bool IsDone()
            {
                if (m_damage <= m_absorbDamage) return m_ttl > 0.0 && m_time > m_ttl;
                Transform transform = m_character.transform;
                LoadedAssets.ShieldBreakEffects.Create(m_character.GetCenterPoint(), transform.rotation, transform, m_character.GetRadius() * 2f);
                m_character.RaiseSkill(Skills.SkillType.BloodMagic, 1f);
                return true;
            }

            public override void UpdateStatusEffect(float dt)
            {
                base.UpdateStatusEffect(dt);
                if (TalentManager.GetDamages(data.talent).HaveDamage()) DamageArea(dt);
                if (ShouldAffect(StatusEffectData.Modifier.Heal))
                {
                    UpdateHeal(dt);
                }
            }

            private void UpdateHeal(float dt)
            {
                healTimer += dt;
                if (healTimer < 5f) return;
                healTimer = 0.0f;

                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Heal, out ConfigEntry<float> config)) return;
                
                m_character.Heal(Spells.ApplySkill(Skills.SkillType.BloodMagic, config.Value * data.talent.m_level));
            }

            public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
            {
                if (data.m_isCharacteristic)
                {
                    if (hitData.m_skill is Skills.SkillType.Axes or Skills.SkillType.Clubs or Skills.SkillType.Knives
                        or Skills.SkillType.Polearms or Skills.SkillType.Spears or Skills.SkillType.Swords or Skills.SkillType.Unarmed)
                    {
                        hitData.ApplyModifier(PlayerManager.GetDamageRatio(Characteristic.Strength));
                    }
                    if (hitData.m_skill is Skills.SkillType.Bows or Skills.SkillType.Crossbows)
                    {
                        hitData.ApplyModifier(PlayerManager.GetDamageRatio(Characteristic.Dexterity));
                    }

                    if (hitData.m_skill is Skills.SkillType.ElementalMagic or Skills.SkillType.BloodMagic)
                    {
                        hitData.ApplyModifier(PlayerManager.GetDamageRatio(Characteristic.Intelligence));
                    }

                    if (PlayerManager.m_playerTalents.TryGetValue("CoreLumberjack", out Talent ability))
                    {
                        hitData.m_damage.m_chop *= AlmanacClassesPlugin._LumberjackIncrease.Value * ability.m_level;
                    }
                }

                if (data.talent.m_key == "RogueDamage")
                {
                    if (data.talent.m_chance != null)
                    {
                        float percentage = (data.talent.m_chance?.Value ?? 20f) * data.talent.m_level;
                        int random = UnityEngine.Random.Range(0, 101);
                        if (random <= percentage)
                        {
                            hitData.ApplyModifier(hitData.m_backstabBonus);
                            LoadedAssets.ShieldBreakEffects.Create(hitData.m_point, Quaternion.identity);
                        }
                    }
                }

                if (!ShouldAffect(StatusEffectData.Modifier.Attack)) return;

                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Attack,
                        out ConfigEntry<float> config)) return;
                
                hitData.ApplyModifier(config.Value * data.talent.m_level);
            }

            public override void ModifyHealthRegen(ref float regenMultiplier)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.HealthRegen)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.HealthRegen,
                        out ConfigEntry<float> config)) return;
                
                regenMultiplier *= config.Value * data.talent.m_level;
            }

            public override void ModifyStaminaRegen(ref float staminaRegen)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.StaminaRegen)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.StaminaRegen,
                        out ConfigEntry<float> config)) return;
                
                staminaRegen *= config.Value * data.talent.m_level;
            }

            public override void ModifyRaiseSkill(Skills.SkillType skill, ref float value)
            {
                if (data.m_isCharacteristic)
                {
                    int wisdom = CharacteristicManager.GetCharacteristic(Characteristic.Wisdom);
                    if (wisdom > 0)
                    {
                        value *= Mathf.Clamp(wisdom / 10f, 1f, Single.MaxValue);
                    }
                }
                else
                {
                    if (!ShouldAffect(StatusEffectData.Modifier.RaiseSkills)) return;

                    if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.RaiseSkills,
                            out ConfigEntry<float> config)) return;
                    
                    value *= config.Value * data.talent.m_level;
                }
            }

            public override void ModifySpeed(float baseSpeed, ref float speed)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.Speed)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Speed,
                        out ConfigEntry<float> config)) return;
                
                speed *= config.Value * data.talent.m_level;
            }

            public override void ModifyNoise(float baseNoise, ref float noise)
            {
                if (data.m_isCharacteristic)
                {
                    int dexterity = CharacteristicManager.GetCharacteristic(Characteristic.Dexterity);
                    if (dexterity > 0)
                    {
                        noise *= Mathf.Clamp(dexterity / 5f, 0.1f, 1f);
                    }
                }
                else
                {
                    if (!ShouldAffect(StatusEffectData.Modifier.Noise)) return;
                    if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Noise,
                            out ConfigEntry<float> config)) return;
                    
                    noise *= config.Value * data.talent.m_level;
                }
            }

            public override void ModifyStealth(float baseStealth, ref float stealth)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.Stealth)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Stealth,
                        out ConfigEntry<float> config)) return;
                
                stealth *= config.Value * data.talent.m_level;
            }

            public override void ModifyMaxCarryWeight(float baseLimit, ref float limit)
            {
                if (data.talent.m_statusEffect.m_isCharacteristic)
                {
                    limit += CharacteristicManager.GetCharacteristic(Characteristic.Strength) / m_carryWeightRatio;
                }
                else
                {
                    if (!ShouldAffect(StatusEffectData.Modifier.MaxCarryWeight)) return;
                    if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.MaxCarryWeight, out ConfigEntry<float> config)) return;
                    
                    limit += config.Value * data.talent.m_level;
                }
            }

            public override void ModifyRunStaminaDrain(float baseDrain, ref float drain)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.RunStaminaDrain)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.RunStaminaDrain, out ConfigEntry<float> config)) return;
                
                drain *= config.Value;
            }

            public override void ModifyJumpStaminaUsage(float baseStaminaUse, ref float staminaUse)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.JumpStaminaDrain)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.JumpStaminaDrain,
                        out ConfigEntry<float> config)) return;
                
                staminaUse *= config.Value;
            }

            public override void OnDamaged(HitData hit, Character attacker)
            {
                if (!attacker) return;
                if (!m_character) return;
                if (ShouldAffect(StatusEffectData.Modifier.Reflect))
                {
                    if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.Reflect, out ConfigEntry<float> reflectConfig)) return;
                    
                    float modifier = reflectConfig.Value * data.talent.m_level;
                    HitData hitData = new()
                    {
                        m_damage = hit.m_damage,
                        m_skill = Skills.SkillType.Knives,
                        m_hitType = hit.m_hitType,
                        m_attacker = m_character.GetZDOID(),
                        m_blockable = false,
                        m_dodgeable = false,
                        m_skillRaiseAmount = 1f,
                    };
                    hitData.m_damage.Modify(modifier);
                    attacker.Damage(hitData);
                }
                
                if (ShouldAffect(StatusEffectData.Modifier.DamageAbsorb))
                {
                    m_damage += hit.GetTotalDamage();
                    hit.ApplyModifier(0.0f);
                    m_character.RaiseSkill(Skills.SkillType.BloodMagic);
                    LoadedAssets.ShieldHitEffects.Create(hit.m_point, Quaternion.LookRotation(-hit.m_dir), m_character.transform);
                    return;
                }
                
                if (!ShouldAffect(StatusEffectData.Modifier.DamageReduction)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.DamageReduction, out ConfigEntry<float> damageReductionConfig)) return;
                
                float value = Mathf.Clamp01(2f - damageReductionConfig.Value * data.talent.m_level);
                hit.ApplyModifier(value);
            }

            public override void ModifyDamageMods(ref HitData.DamageModifiers modifiers)
            {
                List<HitData.DamageModPair> resistances = new();
                foreach (KeyValuePair<HitData.DamageType, ConfigEntry<HitData.DamageModifier>> resist in data.talent.m_resistances)
                {
                    var resistance = new HitData.DamageModPair
                    {
                        m_type = resist.Key,
                        m_modifier = resist.Value.Value
                    };
                    resistances.Add(resistance);
                }
                modifiers.Apply(resistances);
            }

            public override void ModifyFallDamage(float baseDamage, ref float damage)
            {
                if (m_character.GetSEMan().HaveStatusEffect("SlowFall".GetStableHashCode())) return;
                if (ShouldAffect(StatusEffectData.Modifier.FallDamage))
                {
                    if (data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.FallDamage, out ConfigEntry<float> config))
                    {
                        damage *= config.Value;
                    }
                }
                if (damage >= 0.0) return;
                damage = 0.0f;
            }

            public override void ModifyEitrRegen(ref float eitrRegen)
            {
                if (!ShouldAffect(StatusEffectData.Modifier.EitrRegen)) return;
                if (!data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.EitrRegen,
                        out ConfigEntry<float> config)) return;
                
                eitrRegen *= config.Value * data.talent.m_level;
            }

            public override void Setup(Character character)
            {
                // base.Setup(character);
                m_character = character;
                if (data.talent.m_triggerStartEffects != null)
                {
                    if (data.talent.m_triggerStartEffects.Value is AlmanacClassesPlugin.Toggle.On)
                    {
                        TriggerStartEffects();
                    }
                }
                else
                {
                    TriggerStartEffects();
                }
                
                m_ttl = data.talent.m_duration?.Value ?? 0f;
                if (data.talent.m_modifiers.TryGetValue(StatusEffectData.Modifier.DamageAbsorb, out ConfigEntry<float> config))
                {
                    m_absorbDamage = Spells.ApplySkill(Skills.SkillType.BloodMagic, config.Value * data.talent.m_level);
                }
                Player? player = character as Player;
                if (player == null) return;
                if (AnimationManager.IsEmote(data.talent.m_animation))
                {
                    if (player.InEmote()) player.StopEmote();
                    player.StartEmote(data.talent.m_animation);
                }
            }
            
            public override string GetTooltipString()
            {
                StringBuilder stringBuilder = new();
                if (!data.m_isCharacteristic) return "";
                int experience = PlayerManager.m_tempPlayerData.m_experience;
                int level = PlayerManager.GetPlayerLevel(experience);
                int nextLevelExp = PlayerManager.GetRequiredExperience(level + 1);
                List<string> info = new()
                {
                    $"$text_level: <color=orange>{level}</color>",
                    $"$text_experience: <color=orange>{experience}</color> / <color=orange>{nextLevelExp}</color>",
                    $"$almanac_vitality: <color=orange>{PlayerManager.GetTotalAddedHealth()}</color>",
                    $"$se_stamina: <color=orange>{PlayerManager.GetTotalAddedStamina()}</color>",
                    $"$se_eitr: <color=orange>{PlayerManager.GetTotalAddedEitr()}</color>",
                    $"$text_melee_dmg: <color=orange>{(int)((PlayerManager.GetDamageRatio(Characteristic.Strength) - 1) * 100)}</color>%",
                    $"$text_ranged_dmg: <color=orange>{(int)((PlayerManager.GetDamageRatio(Characteristic.Dexterity) - 1) * 100)}</color>%",
                    $"$text_magic_dmg: <color=orange>{(int)((PlayerManager.GetDamageRatio(Characteristic.Intelligence) - 1) * 100)}</color>%"
                };
                foreach (string item in info)
                {
                    stringBuilder.Append(item);
                    stringBuilder.Append("\n");
                }

                return Localization.instance.Localize(stringBuilder.ToString());
            }
        }
    }

    public class CustomFinder : StatusEffect
    {
        public EffectList m_pingEffectNear = new();
        public EffectList m_pingEffectMed = new();
        public EffectList m_pingEffectFar = new();
        public float m_closeFrequency = 1f;
        public float m_distantFrequency = 5f;
        public float m_updateBeaconTimer;
        public float m_pingTimer;
        public Beacon? m_beacon;
        public bool m_findCharacters;
        public float m_findRadius = 10f;

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            if (m_findCharacters)
            {
                m_updateBeaconTimer += dt;
                if (!(m_updateBeaconTimer > 1.0)) return;
                m_updateBeaconTimer = 0.0f;
                List<Character> characters = new();
                string targetName = GetTargetByBiome();
                if (targetName.IsNullOrWhiteSpace()) return;
                Character.GetCharactersInRange(m_character.transform.position, m_findRadius, characters);
                foreach (Character target in characters)
                {
                    if (target.name.Replace("(Clone)", string.Empty) != targetName) continue;
                    if (target.GetSEMan().HaveStatusEffect("SE_Hunter".GetStableHashCode()))
                    {
                        target.GetSEMan().GetStatusEffect("SE_Hunter".GetStableHashCode()).ResetTime();
                    }
                    else
                    {
                        target.GetSEMan().AddStatusEffect("SE_Hunter".GetStableHashCode());
                    }
                }
            }
            else
            {
                m_updateBeaconTimer += dt;
                if (m_updateBeaconTimer > 1.0)
                {
                    UpdateBeacon();
                }
                UpdateBeaconEffect(dt);
            }
        }

        private void UpdateBeacon()
        {
            Beacon closestBeaconInRange = Beacon.FindClosestBeaconInRange(m_character.transform.position);
            if (closestBeaconInRange == m_beacon) return;
            m_beacon = closestBeaconInRange;
            if (!m_beacon) return;
            m_pingTimer = 0.0f;
        }

        private void UpdateBeaconEffect(float dt)
        {
            if (m_beacon == null) return;
            float num1 = Utils.DistanceXZ(m_character.transform.position, m_beacon.transform.position);
            float t = Mathf.Clamp01(num1 / m_beacon.m_range);
            float num2 = Mathf.Lerp(m_closeFrequency, m_distantFrequency, t);
            m_pingTimer += dt;
            if (m_pingTimer <= num2) return;
            m_pingTimer = 0.0f;
            Transform transform = m_character.transform;
            if (t < 0.20000000298023224)
            {
                m_pingEffectNear.Create(transform.position, transform.rotation, transform);
            }
            else if (t < 0.6000000238418579)
            {
                m_pingEffectMed.Create(transform.position, transform.rotation, transform);
            }
            else
            {
                m_pingEffectFar.Create(transform.position, transform.rotation, transform);
            }
        }

        public override string GetTooltipString() => "";

        private string GetTargetByBiome()
        {
            Heightmap.Biome biome = WorldGenerator.instance.GetBiome(m_character.transform.position);
            switch (biome)
            {
                case Heightmap.Biome.Meadows:
                    return AlmanacClassesPlugin._HunterMeadows.Value;
                case Heightmap.Biome.BlackForest:
                    return AlmanacClassesPlugin._HunterBlackForest.Value;
                case Heightmap.Biome.Swamp:
                    return AlmanacClassesPlugin._HunterSwamp.Value;
                case Heightmap.Biome.Mountain:
                    return AlmanacClassesPlugin._HunterMountain.Value;
                case Heightmap.Biome.Plains:
                    return AlmanacClassesPlugin._HunterPlains.Value;
                case Heightmap.Biome.Mistlands:
                    return AlmanacClassesPlugin._HunterMistLands.Value;
                case Heightmap.Biome.AshLands:
                    return AlmanacClassesPlugin._HunterAshLands.Value;
                case Heightmap.Biome.DeepNorth:
                    return AlmanacClassesPlugin._HunterDeepNorth.Value;
                case Heightmap.Biome.Ocean:
                    return AlmanacClassesPlugin._HunterOcean.Value;
                default:
                    return "Deer";
            }
        }
    }
}