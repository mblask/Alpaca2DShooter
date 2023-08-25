using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    private static CraftingUI _instance;

    private Transform _container;
    private Transform _possibleCraftsTransform;
    private List<IngredientSlotUI> _ingredientSlotUIList;
    private ProductSlotUI _productSlotUI;
    private Transform _possibleCraftedItemsContainer;

    private Button _craftButton;

    private CraftingRecipe _recipeToCraft;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _possibleCraftsTransform = _container.Find("PossibleCraftsScreen");
        _possibleCraftedItemsContainer = _possibleCraftsTransform.Find("ScrollView").Find("Viewport").Find("Content");
        _ingredientSlotUIList = new List<IngredientSlotUI>(_container.Find("CraftingScreen").GetComponentsInChildren<IngredientSlotUI>());
        _productSlotUI = _container.Find("CraftingScreen").Find("ProductSlot").GetComponent<ProductSlotUI>();
        _craftButton = transform.Find("Container").Find("CraftingScreen").Find("CraftButton").GetComponent<Button>();
    }

    private void Start()
    {
        PlayerInventory.Instance.OnToggleCraftingUI += showCraftingUI;
        _gameAssets = GameAssets.Instance;
        _craftButton.onClick.AddListener(craftItem);
    }

    private void OnDisable()
    {
        PlayerInventory.Instance.OnToggleCraftingUI -= showCraftingUI;
    }

    private void craftItem()
    {
        if (_recipeToCraft == null)
            return;

        if (!CraftingManager.CraftItemStatic(_recipeToCraft))
            return;

        clearCraftingSlots();
        showPossibleCraftsScreen(CraftingManager.GetPossibleCraftsStatic().Count > 0);
    }

    public static void PopulateCraftingSlots(CraftingRecipe recipe)
    {
        _instance.populateCraftingSlots(recipe);
    }

    private void clearCraftingSlots()
    {
        _recipeToCraft = null;
        foreach (IngredientSlotUI ingredientSlot in _ingredientSlotUIList)
            ingredientSlot.RemoveItemFromSlot();

        _productSlotUI.RemoveItemFromSlot();
    }

    private void populateCraftingSlots(CraftingRecipe recipe)
    {
        if (recipe == null)
            return;

        _recipeToCraft = recipe;

        for (int i = 0; i < _ingredientSlotUIList.Count; i++)
        {
            if (recipe.CraftingIngredients[i].Item != null)
                _ingredientSlotUIList[i].AddItemToSlot(recipe.CraftingIngredients[i].Item);
        }
    
        if (recipe.ProductItem != null)
            _productSlotUI.AddItemToSlot(recipe.ProductItem);
    }

    private void showCraftingUI()
    {
        bool isActive = !_container.gameObject.activeSelf;

        _container.gameObject.SetActive(isActive);

        foreach (IngredientSlotUI ingredientSlot in _ingredientSlotUIList)
            ingredientSlot.RemoveItemFromSlot();

        _productSlotUI.RemoveItemFromSlot();

        showPossibleCraftsScreen(_container.gameObject.activeSelf && CraftingManager.GetPossibleCraftsStatic().Count > 0);

        if (!isActive)
            ItemTooltip.RemoveTooltipStatic();
    }

    private void showPossibleCraftsScreen(bool value)
    {
        _possibleCraftsTransform.gameObject.SetActive(value);

        if (value)
        {
            foreach (Transform possibleCraftUITransform in _possibleCraftedItemsContainer)
            {
                Destroy(possibleCraftUITransform.gameObject);
            }

            List<CraftingRecipe> possibleRecipes = CraftingManager.GetPossibleCraftsStatic();
            foreach (CraftingRecipe recipe in possibleRecipes)
            {
                PossibleCraftUI possibleCraftUI = Instantiate(_gameAssets.PossibleCraftUI, _possibleCraftedItemsContainer).GetComponent<PossibleCraftUI>();
                possibleCraftUI.AddPossibleRecipe(recipe);
            }
        }
    }
}
