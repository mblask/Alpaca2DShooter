using System;

[Serializable]
public class CraftingIngredient
{
    public InventoryItem Item;
    public bool DestroyedOnCrafting = true;
}
