using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private static CraftingManager _instance;
    [SerializeField] private List<CraftingRecipe> _craftingRecipes;

    private AchievementManager _achievementManager;
    private PlayerInventory _playerInventory;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _achievementManager = AchievementManager.Instance;
        _playerInventory = PlayerInventory.Instance;
        _playerWeapons = PlayerWeapons.Instance;
    }

    public static bool CraftItemStatic(CraftingRecipe craftingRecipe)
    {
        if (_instance.craftItem(craftingRecipe))
        {
            _instance._achievementManager.CheckOnItemCrafted();
            return true;
        }

        return false;
    }

    private bool craftItem(CraftingRecipe craftingRecipe)
    {
        if (craftingRecipe == null)
            return false;

        foreach (CraftingIngredient ingredient in craftingRecipe.CraftingIngredients)
            if (ingredient.DestroyedOnCrafting)
                _playerInventory.DeleteItemFromInventory(ingredient.Item);

        if (craftingRecipe.ProductItem is AmmoItem)
            return (craftingRecipe.ProductItem as AmmoItem).UseItem();

        if (craftingRecipe.ProductItem is InstantaneousItem)
            return (craftingRecipe.ProductItem as InstantaneousItem).UseItem();

        if (craftingRecipe.ProductItem is ThrowableItem)
        {
            int craftingQuantity = 3;
            ThrowableWeapon throwable = 
                new ThrowableWeapon(craftingRecipe.ProductItem as ThrowableItem, craftingQuantity);
            return _playerWeapons.AddThrowable(throwable);
        }

        return _playerInventory.AddToInventory(craftingRecipe.ProductItem);
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
