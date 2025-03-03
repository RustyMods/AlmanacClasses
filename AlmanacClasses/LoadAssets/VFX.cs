using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AlmanacClasses.LoadAssets;

public static class VFX
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private static GameObject VFX_BardNotes = null!;
    private static GameObject VFX_MagicRunes = null!;
    private static GameObject VFX_BardNotesBurn = null!;
    public static GameObject GoblinBeam = null!;
    public static GameObject Meteor = null!;
    public static GameObject TrollStone = null!;
    public static GameObject GDKingRoots = null!;
    public static GameObject Fireball = null!;
    public static GameObject Lightning = null!;
    
    private static readonly List<TalentEffectList> Effects = new();
    private static readonly List<CustomPrefab> CustomPrefabs = new();
    public static readonly TalentEffectList LeechEffects = new("vfx_leech_hit", "sfx_leech_hit");
    public static readonly TalentEffectList ShiedHitEffect = new("fx_StaffShield_Hit");
    public static readonly TalentEffectList ShieldBreakEffect = new("fx_StaffShield_Break");
    public static readonly TalentEffectList TrapArmed = new("fx_trap_arm");
    public static readonly TalentEffectList BleedEffect = new(() =>
    {
        GameObject bleed = Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_Wet"), AlmanacClassesPlugin._Root.transform, false);
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
        return new(){m_prefab = bleed};
    });
    public static readonly TalentEffectList SongOfSpirit = new(
        () =>
    {
        GameObject VFX_UndeadBurn = Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_UndeadBurn"),
            AlmanacClassesPlugin._Root.transform, false);
        Transform trails = VFX_UndeadBurn.transform.Find("trails");
        Transform flames = VFX_UndeadBurn.transform.Find("flames");
        Object.Destroy(trails.gameObject);
        Object.Destroy(flames.gameObject);
        VFX_UndeadBurn.name = "vfx_SongOfSpirit";
        RegisterToZNetScene(VFX_UndeadBurn);
        return new(){m_prefab = VFX_UndeadBurn, m_attach = true, m_scale = true};
    }, 
        () => new(){m_prefab = ZNetScene.instance.GetPrefab("fx_DvergerMage_Nova_ring")});
    public static readonly TalentEffectList DragonBreath = new("vfx_dragon_coldbreath", "sfx_dragon_coldball_start");
    public static readonly TalentEffectList DragonBreathHit = new("vfx_iceblocker_destroyed");
    public static readonly TalentEffectList FX_ChainLightning_Hit = new("fx_chainlightning_hit");
    public static readonly TalentEffectList SoothEffects = new("vfx_creature_soothed");
    public static readonly TalentEffectList UnSummonEffects = new("vfx_skeleton_death");
    public static readonly TalentEffectList StaffPreSpawnEffects = new("fx_summon_skeleton_spawn");
    public static readonly TalentEffectList FX_SummonSkeleton = new("fx_summon_skeleton");
    public static readonly TalentEffectList FX_DvergerPower = new(() =>
    {
        GameObject fx_dverger_support_start = ZNetScene.instance.GetPrefab("fx_DvergerMage_Support_start");
        GameObject FX_TalentPower = Object.Instantiate(fx_dverger_support_start, AlmanacClassesPlugin._Root.transform, false);
        FX_TalentPower.name = "fx_TalentPower";
        ParticleSystem spark = FX_TalentPower.transform.Find("sparks").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = spark.main;
        mainModule.loop = true;
        Object.Destroy(FX_TalentPower.GetComponent<TimedDestruction>());
        RegisterToZNetScene(FX_TalentPower);
        return new() { m_prefab = FX_TalentPower, m_attach = true, m_inheritParentScale = true, m_inheritParentRotation = true};
    });
    public static readonly TalentEffectList FX_Heal = new(() =>
    {
        GameObject fx_dverger_support_start = ZNetScene.instance.GetPrefab("fx_DvergerMage_Support_start");
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
        return new() { m_prefab = fx_healing, m_attach = true, m_follow = true };
    });

    public static readonly TalentEffectList FX_Experience = new(
        () => new(){m_prefab = ZNetScene.instance.GetPrefab("vfx_Potion_eitr_minor"), m_enabled = true, m_attach = true, m_scale = true},
        () => new(){m_prefab = ZNetScene.instance.GetPrefab("sfx_demister_start"), m_enabled = true, m_attach = true, m_scale = true});

    public static readonly TalentEffectList FX_BattleFury = new(
        () => new(){m_prefab = ZNetScene.instance.GetPrefab("vfx_Potion_stamina_medium"), m_attach = true, m_scale = true}, 
        () => new(){m_prefab = ZNetScene.instance.GetPrefab("sfx_boar_love"), m_attach = true});

    public static readonly TalentEffectList FX_Electric = new(() =>
    {
        GameObject FX_Lightning = ZNetScene.instance.GetPrefab("fx_Lightning");
        GameObject FX_lightningClone = Object.Instantiate(FX_Lightning, AlmanacClassesPlugin._Root.transform, false);
        FX_lightningClone.name = "fx_Lightning_timed";
        RegisterToZNetScene(FX_lightningClone);
        return new() { m_prefab = FX_lightningClone, m_attach = true, m_scale = true, m_multiplyParentVisualScale = true };
    });

    public static readonly TalentEffectList FX_RogueBleed = new(()=>new(){m_prefab = ZNetScene.instance.GetPrefab("fx_CharredStone_Destruction"), m_multiplyParentVisualScale = true});
    public static readonly TalentEffectList SFX_Dverger_Shot = new("sfx_dverger_fireball_rain_shot", "fx_Fader_Spin");
    public static readonly TalentEffectList FX_GP_Activation = new("fx_GP_Activation");
    public static readonly TalentEffectList PingEffectNear = new("vfx_WishbonePing", "sfx_WishbonePing_near");
    public static readonly TalentEffectList PingEffectMed = new("vfx_WishbonePing", "sfx_WishbonePing_med");
    public static readonly TalentEffectList PingEffectFar = new("vfx_WishbonePing", "sfx_WishbonePing_far");
    public static readonly CustomPrefab CustomTrap = new(() =>
    {
        GameObject customTrap = Object.Instantiate(ZNetScene.instance.GetPrefab("piece_trap_troll"), AlmanacClassesPlugin._Root.transform, false);
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

        return customTrap;
    });
    public class TalentEffectList
    {
        private readonly List<Func<EffectList.EffectData>> m_actions = new();
        private readonly List<string> m_prefabNames = new();
        public readonly EffectList m_effectList = new();
        public GameObject[] Create(Vector3 basePos, Quaternion baseRot, Transform? baseParent = null, float scale = 1f,
            int variant = -1) => m_effectList.Create(basePos, baseRot, baseParent, scale, variant);
        public void Setup()
        {
            if (!ZNetScene.instance) return;
            List<GameObject> prefabs = m_prefabNames.Select(name => ZNetScene.instance.GetPrefab(name)).Where(prefab => prefab is not null).ToList();
            List<EffectList.EffectData> data = prefabs.Select(prefab => new EffectList.EffectData() { m_prefab = prefab }).ToList();
            data.AddRange(m_actions.Select(action => action.Invoke()).ToList());
            m_effectList.m_effectPrefabs = data.ToArray();
        }
        public TalentEffectList(params string[] effectNames)
        {
            m_prefabNames = effectNames.ToList();
            Effects.Add(this);
        }
        public TalentEffectList(params Func<EffectList.EffectData>[] customs)
        {
            m_actions = customs.ToList();
            Effects.Add(this);
        }
    }

    public class CustomPrefab
    {
        private readonly Func<GameObject> m_action;
        public GameObject m_prefab = null!;
        public GameObject Instantiate(Vector3 pos, Quaternion rot) => Object.Instantiate(m_prefab, pos, rot);
        public void Setup()
        {
            m_prefab = m_action.Invoke();
            RegisterToZNetScene(m_prefab);
        }
        public CustomPrefab(Func<GameObject> action)
        {
            m_action = action;
            CustomPrefabs.Add(this);
        }
    }

    public static void Init()
    {
        foreach(var effect in Effects) effect.Setup();
        foreach(var custom in CustomPrefabs) custom.Setup();
        
        VFX_BardNotes = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("FX_BardNotes");
        VFX_MagicRunes = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("FX_MagicRunes");
        VFX_BardNotesBurn = AlmanacClassesPlugin._AssetBundle.LoadAsset<GameObject>("vfx_bardNotesBurn");

        RegisterToZNetScene(VFX_BardNotes);
        RegisterToZNetScene(VFX_MagicRunes);
        RegisterToZNetScene(VFX_BardNotesBurn);
        
        GoblinBeam = ZNetScene.instance.GetPrefab("projectile_beam");
        Meteor = ZNetScene.instance.GetPrefab("projectile_meteor_fader");
        TrollStone = ZNetScene.instance.GetPrefab("troll_throw_projectile");
        GDKingRoots = ZNetScene.instance.GetPrefab("gdking_root_projectile");
        Fireball = ZNetScene.instance.GetPrefab("staff_fireball_projectile");
        Lightning = ZNetScene.instance.GetPrefab("staff_lightning_projectile");
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