using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "Scriptable Objects / Crafting Recipe", order = 5)]
public class CraftingRecipe : ScriptableObject
{
    /// <summary>
    /// Recipe should contain only singular ingredients
    /// <br>E.g. Knife + Tools + Scrap Metal, but not Tools + Scrap Metal + Scrap Metal</br>
    /// </summary>
    public List<CraftingIngredient> CraftingIngredients = new List<CraftingIngredient>(3);

    [Space]
    public Item ProductItem;
}
