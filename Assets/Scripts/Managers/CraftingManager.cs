using System.Collections;
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

    public bool craftItem(CraftingRecipe craftingRecipe)
    {
        if (craftingRecipe == null)
            return false;

        foreach (Item item in craftingRecipe.Ingredients)
            PlayerInventory.DeleteItemFromInventoryStatic(item);

        return PlayerInventory.AddToInventoryStatic(craftingRecipe.ProductItem);
    }

    public static List<CraftingRecipe> GetPossibleCraftsStatic()
    {
        List<Item> items = PlayerInventory.GetItemsStatic();

        return _instance.numberOfPossibleCrafts(items);
    }

    public static List<CraftingRecipe> GetPossibleCraftsStatic(List<Item> items)
    {
        return _instance.numberOfPossibleCrafts(items);
    }

    private List<CraftingRecipe> numberOfPossibleCrafts(List<Item> items)
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

            foreach (Item item in recipe.Ingredients)
            {
                if (item == null || (item != null && items.Contains(item)))
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
