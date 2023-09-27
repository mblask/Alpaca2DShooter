using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private static CraftingManager _instance;
    [SerializeField] private List<CraftingRecipe> _craftingRecipes;

    private void Awake()
    {
        _instance = this;
    }

    public static bool CraftItemStatic(CraftingRecipe craftingRecipe)
    {
        return _instance.craftItem(craftingRecipe);
    }

    private bool craftItem(CraftingRecipe craftingRecipe)
    {
        if (craftingRecipe == null)
            return false;

        foreach (CraftingIngredient ingredient in craftingRecipe.CraftingIngredients)
            if (ingredient.DestroyedOnCrafting)
                PlayerInventory.DeleteItemFromInventoryStatic(ingredient.Item);

        if (craftingRecipe.ProductItem is AmmoItem)
            return (craftingRecipe.ProductItem as AmmoItem).UseItem();

        return PlayerInventory.AddToInventoryStatic(craftingRecipe.ProductItem);
    }

    public static List<CraftingRecipe> GetPossibleCraftsStatic()
    {
        List<Item> items = PlayerInventory.GetItemsStatic();

        return _instance.getPossibleCrafts(items);
    }

    public static List<CraftingRecipe> GetPossibleCraftsStatic(List<Item> items)
    {
        return _instance.getPossibleCrafts(items);
    }

    private List<CraftingRecipe> getPossibleCrafts(List<Item> items)
    {
        List<CraftingRecipe> possibleCrafts = new List<CraftingRecipe>();

        foreach (CraftingRecipe recipe in _craftingRecipes)
        {
            if (recipe.ProductItem == null)
            {
                Debug.LogError("Recipe: " + recipe.name + ", doesn't have a product item");
                continue;
            }

            bool ingredientsValid = false;

            foreach (CraftingIngredient ingredient in recipe.CraftingIngredients)
            {
                if (ingredient.Item == null || (ingredient.Item != null && items.Contains(ingredient.Item)))
                    ingredientsValid = true;
                else
                {
                    ingredientsValid = false;
                    break;
                }
            }

            if (ingredientsValid)
                possibleCrafts.Add(recipe);
        }

        return possibleCrafts;
    }
}
