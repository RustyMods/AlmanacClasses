using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AlmanacClasses.LoadAssets;

public static class LoadedAssets
{
    private static GameObject VFX_BardNotes = null!;
    private static GameObject VFX_MagicRunes = null!;
    private static GameObject VFX_BardNotesBurn = null!;

    public static GameObject lightning_AOE = null!;
    public static GameObject GoblinBeam = null!;
    public static GameObject Meteor = null!;
    public static GameObject TrollStone = null!;
    public static GameObject GDKingRoots = null!;

    public static GameObject SkeletonFriendly = null!;
    public static GameObject CustomTrap = null!;
    
    public static EffectList StaffPreSpawnEffects = null!;
    public static EffectList FX_SummonSkeleton = null!;
    public static EffectList SoothEffects = null!;
    public static EffectList UnSummonEffects = null!;
    public static EffectList TrapArmedEffects = null!;

    public static EffectList FX_Electric = null!;
    
    public static EffectList ShieldHitEffects = null!;
    public static EffectList ShieldBreakEffects = null!;
    public static EffectList BleedEffects = null!;
    public static EffectList VFX_SongOfSpirit = null!;
    
    public static EffectList FX_DvergerPower = null!;
    public static EffectList DragonBreath = null!;

    public static EffectList DragonBreathHit = null!;
    
    public static SE_Finder? SE_Finder;
    public static StatusEffect GP_Moder = null!;

    public static EffectList FX_Experience = null!;
    public static EffectList FX_BattleFury = null!;

    public static EffectList FX_Heal = null!;
    public static EffectList FX_RogueBleed = null!;

    public static EffectList SFX_Dverger_Shot = null!;
    
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public static void InitVFX()
    {
        ZNetScene instance = ZNetScene.instance;

        SE_Finder = ObjectDB.instance.GetStatusEffect("Wishbone".GetStableHashCode()) as SE_Finder;
        GP_Moder = ObjectDB.instance.GetStatusEffect("GP_Moder".GetStableHashCode());

        SE_Shield? SE_Shield = ObjectDB.instance.GetStatusEffect("Staff_shield".GetStableHashCode()) as SE_Shield;
        if (SE_Shield != null)
        {
            ShieldHitEffects = new EffectList()
            {
                m_effectPrefabs = SE_Shield.m_hitEffects.m_effectPrefabs
            };
            ShieldBreakEffects = new EffectList()
            {
                m_effectPrefabs = SE_Shield.m_breakEffects.m_effectPrefabs
            };
        }

        TrapArmedEffects = new()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = instance.GetPrefab("fx_trap_arm"),
                    m_enabled = true,
                }
            }
        };

        GameObject bleed = Object.Instantiate(instance.GetPrefab("vfx_Wet"), AlmanacClassesPlugin._Root.transform, false);
        bleed.name = "fx_bleed";
        if (bleed.transform.GetChild(0).TryGetComponent(out ParticleSystem particleSystem))
        {
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient()
            {
                mode = ParticleSystemGradientMode.Color,
                color = Color.red,
                colorMin = Color.red,
                colorMax = Color.red
            };
        }

        foreach (var renderer in bleed.GetComponentsInChildren<Renderer>())
        {
            List<Material> materials = new();
            foreach (var material in renderer.materials)
            {
                Material mat = new Material(material)
                {
                    color = Color.red
                };
                if (mat.HasProperty(EmissionColor)) mat.SetColor(EmissionColor, Color.red);
                materials.Add(mat);
            }

            renderer.materials = materials.ToArray();
            renderer.sharedMaterials = materials.ToArray();
        }
        RegisterToZNetScene(bleed);

        BleedEffects = new()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = bleed,
                    m_enabled = true,
                    m_attach = true,
                    m_scale = true,
                    m_inheritParentScale = true
                }
            }
        };

        GameObject VFX_UndeadBurn = Object.Instantiate(instance.GetPrefab("vfx_UndeadBurn"), AlmanacClassesPlugin._Root.transform, false);
        Transform trails = VFX_UndeadBurn.transform.Find("trails");
        Transform flames = VFX_UndeadBurn.transform.Find("flames");
        Object.Destroy(trails.gameObject);
        Object.Destroy(flames.gameObject);
        VFX_UndeadBurn.name = "vfx_SongOfSpirit";
        RegisterToZNetScene(VFX_UndeadBurn);

        VFX_SongOfSpirit = new EffectList
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = VFX_UndeadBurn,
                    m_enabled = true,
                    m_attach = true,
                    m_scale = true,
                },
                new EffectList.EffectData()
                {
                    m_prefab = instance.GetPrefab("fx_DvergerMage_Nova_ring"),
                    m_enabled = true,
                }
            }
        };
        
        VFX_BardNotes = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("FX_BardNotes");
        VFX_MagicRunes = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("FX_MagicRunes");
        VFX_BardNotesBurn = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("vfx_bardNotesBurn");

        RegisterToZNetScene(VFX_BardNotes);
        RegisterToZNetScene(VFX_MagicRunes);
        RegisterToZNetScene(VFX_BardNotesBurn);
        
        DragonBreath = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = instance.GetPrefab("vfx_dragon_coldbreath"),
                    m_enabled = true,
                    m_attach = true
                },
                new EffectList.EffectData()
                {
                    m_prefab = instance.GetPrefab("sfx_dragon_coldball_start"),
                    m_enabled = true
                }
            }
        };

        DragonBreathHit = new()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = instance.GetPrefab("vfx_iceblocker_destroyed"),
                    m_enabled = true,
                    m_attach = true,
                }
            }
        };
        
        GameObject customLightning = Object.Instantiate(ZNetScene.instance.GetPrefab("lightningAOE"), AlmanacClassesPlugin._Root.transform, false);
        customLightning.name = "lightning_strike";
        if (customLightning.transform.Find("AOE_ROD").TryGetComponent(out Aoe aoe))
        {
            aoe.m_useTriggers = true;
            aoe.m_triggerEnterOnly = true;
            aoe.m_blockable = true;
            aoe.m_dodgeable = true;
            aoe.m_hitProps = false;
            aoe.m_hitOwner = false;
            aoe.m_hitParent = false;
            aoe.m_hitFriendly = false;
        }
        Object.Destroy(customLightning.transform.Find("AOE_AREA").gameObject);
        RegisterToZNetScene(customLightning);

        lightning_AOE = customLightning;
        GoblinBeam = instance.GetPrefab("projectile_beam");
        Meteor = instance.GetPrefab("projectile_meteor");
        TrollStone = instance.GetPrefab("troll_throw_projectile");
        GDKingRoots = instance.GetPrefab("gdking_root_projectile");
        GameObject customTrap = Object.Instantiate(instance.GetPrefab("piece_trap_troll"), AlmanacClassesPlugin._Root.transform, false);
        customTrap.name = "RangerTrap";

        if (customTrap.TryGetComponent(out Piece piece))
        {
            piece.m_resources = null;
            piece.m_destroyedLootPrefab = null;
        }

        if (customTrap.TryGetComponent(out ZNetView zNetView))
        {
            zNetView.m_persistent = false;
        }
        
        if (customTrap.TryGetComponent(out Trap trapComponent))
        {
            trapComponent.m_startsArmed = true;
            trapComponent.m_triggeredByPlayers = false;
        }
        RegisterToZNetScene(customTrap);
        
        CustomTrap = customTrap;
        
        SkeletonFriendly = instance.GetPrefab("Skeleton_Friendly");
        if (SkeletonFriendly.TryGetComponent(out Tameable tameable))
        {
            SoothEffects = tameable.m_sootheEffect;
            UnSummonEffects = tameable.m_unSummonEffect;
        }

        if (instance.GetPrefab("StaffSkeleton").TryGetComponent(out ItemDrop staffSkeletonItemDrop))
        {
            GameObject projectile = staffSkeletonItemDrop.m_itemData.m_shared.m_attack.m_attackProjectile;
            if (projectile.TryGetComponent(out SpawnAbility spawnAbility))
            {
                StaffPreSpawnEffects = spawnAbility.m_preSpawnEffects;
            }
        }

        FX_SummonSkeleton = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = instance.GetPrefab("fx_summon_skeleton"),
                    m_enabled = true,
                }
            }
        };

        GameObject fx_dverger_support_start = instance.GetPrefab("fx_DvergerMage_Support_start");
        GameObject FX_TalentPower = Object.Instantiate(fx_dverger_support_start, AlmanacClassesPlugin._Root.transform, false);
        FX_TalentPower.name = "fx_TalentPower";
        ParticleSystem spark = FX_TalentPower.transform.Find("sparks").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = spark.main;
        mainModule.loop = true;
        Object.Destroy(FX_TalentPower.GetComponent<TimedDestruction>());
        RegisterToZNetScene(FX_TalentPower);

        if (!FX_TalentPower) return;
        FX_DvergerPower = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = FX_TalentPower,
                    m_enabled = true,
                    m_attach = true,
                    m_inheritParentScale = true,
                    m_inheritParentRotation = true
                }
            }
        };

        GameObject fx_healing = Object.Instantiate(fx_dverger_support_start, AlmanacClassesPlugin._Root.transform, false);
        fx_healing.name = "fx_shaman_heal";
        
        foreach (Renderer? renderer in fx_healing.GetComponentsInChildren<Renderer>())
        {
            List<Material> newMats = new();
            foreach (var material in renderer.materials)
            {
                Material mat = new Material(material)
                {
                    color = Color.green
                };
                if (mat.HasProperty(EmissionColor)) mat.SetColor(EmissionColor, Color.green);
                newMats.Add(mat);
            }

            renderer.materials = newMats.ToArray();
            renderer.sharedMaterials = newMats.ToArray();
        }
        RegisterToZNetScene(fx_healing);
        FX_Heal = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = fx_healing,
                    m_attach = true,
                    m_follow = true
                }
            }
        };

        GameObject vfx_eitr = instance.GetPrefab("vfx_Potion_eitr_minor");
        GameObject sfx_demister = instance.GetPrefab("sfx_demister_start");
        FX_Experience = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = vfx_eitr,
                    m_enabled = true,
                    m_attach = true,
                    m_scale = true
                },
                new EffectList.EffectData()
                {
                    m_prefab = sfx_demister,
                    m_enabled = true,
                    m_attach = true
                }
            }
        };

        GameObject vfx_potion_stamina = instance.GetPrefab("vfx_Potion_stamina_medium");
        GameObject sfx_boar_love = instance.GetPrefab("sfx_boar_love");
        FX_BattleFury = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = vfx_potion_stamina,
                    m_enabled = true,
                    m_attach = true,
                    m_scale = true
                },
                new EffectList.EffectData()
                {
                    m_prefab = sfx_boar_love,
                    m_enabled = true,
                    m_attach = true
                }
            }
        };

        GameObject FX_Lightning = instance.GetPrefab("fx_Lightning");
        FX_Electric = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = FX_Lightning,
                    m_enabled = true,
                    m_attach = true,
                    m_scale = true,
                    m_multiplyParentVisualScale = true
                }
            }
        };

        GameObject fx_CharredStone_Destruction = instance.GetPrefab("fx_CharredStone_Destruction");
        FX_RogueBleed = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = fx_CharredStone_Destruction,
                    m_enabled = true,
                    m_multiplyParentVisualScale = true
                }
            }
        };

        GameObject sfx_dverger_fireball_rain_shot = instance.GetPrefab("sfx_dverger_fireball_rain_shot");
        GameObject fx_fader_spin = instance.GetPrefab("fx_Fader_Spin");
        SFX_Dverger_Shot = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = sfx_dverger_fireball_rain_shot,
                    m_enabled = true,
                },
                new EffectList.EffectData()
                {
                    m_prefab = fx_fader_spin,
                    m_enabled = true,
                }
            }
        };
    }

    private static void RegisterToZNetScene(GameObject prefab)
    {
        if (!ZNetScene.instance.m_prefabs.Contains(prefab))
        {
            ZNetScene.instance.m_prefabs.Add(prefab);
        }

        if (!ZNetScene.instance.m_namedPrefabs.ContainsKey(prefab.name.GetStableHashCode()))
        {
            ZNetScene.instance.m_namedPrefabs[prefab.name.GetStableHashCode()] = prefab;
        }
    }
    
    public static EffectList AddBardFX(Color color, string name, bool AddMagicRunes = false)
    {
        EffectList output = new();
        if (!ZNetScene.instance) return output;
        List<EffectList.EffectData> effects = new();
        
        if (!VFX_BardNotes) return output;
        GameObject VFX = Object.Instantiate(VFX_BardNotes, AlmanacClassesPlugin._Root.transform, false);
        if (!VFX) return output;
        VFX.name = name;
        RegisterToZNetScene(VFX);

        foreach (Renderer renderer in VFX.GetComponentsInChildren<Renderer>())
        {
            List<Material> newMats = new();
            foreach (Material mat in renderer.materials)
            {
                Material material = new Material(mat);
                material.color = color;
                newMats.Add(material);
            }

            renderer.materials = newMats.ToArray();
            renderer.sharedMaterials = newMats.ToArray();
        }
        
        effects.Add(new EffectList.EffectData()
        {
            m_prefab = VFX,
            m_attach = true,
            m_enabled = true,
            m_inheritParentScale = true,
            m_inheritParentRotation = true,
        });

        if (AddMagicRunes)
        {
            GameObject Runes = Object.Instantiate(VFX_MagicRunes, AlmanacClassesPlugin._Root.transform, false);
            Runes.name = name + "_Runes";
            RegisterToZNetScene(Runes);

            ParticleSystem[] Runes_PS = Runes.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in Runes_PS)
            {
                ParticleSystem.MainModule mainModule = ps.main;
                mainModule.startColor = color;
                mainModule.loop = false;
            }
            
            effects.Add(new EffectList.EffectData
            {
                m_prefab = Runes,
                m_enabled = true,
            });
        }

        output = new EffectList()
        {
            m_effectPrefabs = effects.ToArray()
        };

        return output;
    }
}