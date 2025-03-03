using AlmanacClasses.UI;
using Managers;
using PieceManager;
using UnityEngine;

namespace AlmanacClasses.LoadAssets;

public static class LoadPieces
{
    public static void LoadClassAltar()
    {
        BuildPiece altar = new("classesbundle", "AlmanacClassAltar");
        altar.Name.English("Class Altar");
        altar.Description.English("");
        altar.Crafting.Set(CraftingTable.None);
        altar.RequiredItems.Add("Stone", 20, true);
        altar.RequiredItems.Add("Wood", 20, true);
        MaterialReplacer.RegisterGameObjectForMatSwap(altar.Prefab.transform.Find("model/replace").gameObject);
        Transform book = altar.Prefab.transform.GetChild(0);
        book.gameObject.AddComponent<TalentBook>();
        altar.Prefab.AddComponent<AltarEffectFade>();
        altar.PlaceEffects = new() { "vfx_Place_workbench", "sfx_build_hammer_stone" };
        altar.DestroyedEffects = new() { "vfx_RockDestroyed", "sfx_rock_destroyed" };
        altar.HitEffects = new() { "vfx_RockHit" };
        altar.SwitchEffects = new() { "vfx_Place_throne02" };
        
        MaterialReplacer.MaterialData StartPlatformMat = new MaterialReplacer.MaterialData(AlmanacClassesPlugin._AssetBundle, "_REPLACE_startplatform", MaterialReplacer.ShaderType.RockShader);
        StartPlatformMat.m_texProperties["_EmissiveTex"] = AlmanacClassesPlugin._AssetBundle.LoadAsset<Texture>("startstone_emissive_bw");
        StartPlatformMat.m_floatProperties["_Glossiness"] = 0.216f;
        StartPlatformMat.m_texProperties["_MossTex"] = AlmanacClassesPlugin._AssetBundle.LoadAsset<Texture>("tex_stone_moss");
        StartPlatformMat.m_floatProperties["_MossAlpha"] = 0f;
        StartPlatformMat.m_floatProperties["_MossBlend"] = 10f;
        StartPlatformMat.m_floatProperties["_MossGloss"] = 0f;
        StartPlatformMat.m_floatProperties["_MossNormal"] = 0.263f;
        StartPlatformMat.m_floatProperties["_MossTransition"] = 0.48f;
        StartPlatformMat.m_floatProperties["_AddSnow"] = 1f;
        StartPlatformMat.m_floatProperties["_AddRain"] = 1f;
        StartPlatformMat.PrefabToModify = altar.Prefab.transform.Find("model/startplatform_mat").gameObject;
    }
}