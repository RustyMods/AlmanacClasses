using System.Collections.Generic;

namespace AlmanacClasses.LoadAssets;

public static class FoodModifications
{
    public static void InitFoodModifications()
    {
        if (!ObjectDB.instance) return;
        List<ItemDrop> consumables = ObjectDB.instance.GetAllItems(ItemDrop.ItemData.ItemType.Consumable, "");
        foreach (ItemDrop item in consumables)
        {
            if (item.m_itemData.m_shared.m_foodEitr <= 0)
            {
                item.m_itemData.m_shared.m_foodEitr = 5;
            }
        }
    }
}