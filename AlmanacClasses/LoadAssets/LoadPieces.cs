using AlmanacClasses.Managers;
using AlmanacClasses.UI;
using UnityEngine;
using ValheimClasses.Managers;

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
        MaterialReplacer.RegisterGameObjectForMatSwap(altar.Prefab);
        Transform book = altar.Prefab.transform.GetChild(0);
        book.gameObject.AddComponent<TalentBook>();
        altar.Prefab.AddComponent<AltarEffectFade>();
        altar.SpecialProperties.NoConfig = true;
    }
}