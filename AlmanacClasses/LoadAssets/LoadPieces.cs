using AlmanacClasses.Managers;
using AlmanacClasses.UI;
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
        MaterialReplacer.RegisterGameObjectForShaderSwap(altar.Prefab.transform.Find("model/startplatform_mat").gameObject, MaterialReplacer.ShaderType.PieceShader);
        Transform book = altar.Prefab.transform.GetChild(0);
        book.gameObject.AddComponent<TalentBook>();
        altar.Prefab.AddComponent<AltarEffectFade>();
    }
}