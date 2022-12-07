using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "Scriptable Objects / Crafting Recipe", order = 5)]
public class CraftingRecipe : ScriptableObject
{
    public List<InventoryItem> Ingredients = new List<InventoryItem>(3);

    [Space]
    public Item ProductItem;
}
