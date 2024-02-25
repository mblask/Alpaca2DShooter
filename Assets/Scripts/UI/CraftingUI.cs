using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour, IUiObject
{

    private Transform _container;
    private Transform _possibleCraftsTransform;
    private List<IngredientSlotUI> _ingredientSlotUIList;
    private ProductSlotUI _productSlotUI;
    private Transform _possibleCraftedItemsContainer;
    private Button _craftButton;

    private CraftingRecipe _recipeToCraft;

    private GameAssets _gameAssets;
    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _container = transform.Find("Container");
        _possibleCraftsTransform = _container.Find("PossibleCraftsScreen");
        _possibleCraftedItemsContainer = _possibleCraftsTransform.Find("ScrollView").Find("Viewport").Find("Content");
        _ingredientSlotUIList = new List<IngredientSlotUI>(_container.Find("CraftingScreen").GetComponentsInChildren<IngredientSlotUI>());
        _productSlotUI = _container.Find("CraftingScreen").Find("ProductSlot").GetComponent<ProductSlotUI>();
        _craftButton = transform.Find("Container").Find("CraftingScreen").Find("CraftButton").GetComponent<Button>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _canvas = GamePlayCanvas.Instance;
        _craftButton.onClick.AddListener(craftItem);
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

    private void clearCraftingSlots()
    {
        _recipeToCraft = null;
        foreach (IngredientSlotUI ingredientSlot in _ingredientSlotUIList)
            ingredientSlot.RemoveItemFromSlot();

        _productSlotUI.RemoveItemFromSlot();
    }

    public void PopulateCraftingSlots(CraftingRecipe recipe)
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

    public bool IsActive()
    {
        if (_container == null)
            return false;

        return _container.gameObject.activeSelf;
    }

    public void ShowCraftingUI(bool value)
    {
        _container.gameObject.SetActive(value);

        foreach (IngredientSlotUI ingredientSlot in _ingredientSlotUIList)
            ingredientSlot.RemoveItemFromSlot();

        _productSlotUI.RemoveItemFromSlot();

        bool showPossibleCrafts = _container.gameObject.activeSelf && CraftingManager.GetPossibleCraftsStatic().Count > 0;
        showPossibleCraftsScreen(showPossibleCrafts);

        if (value)
            GamePlayCanvas.AddOpenUiStatic(this);

        if (!value)
        {
            GamePlayCanvas.RemoveOpenUiStatic(this);
            _canvas.RemoveItemTooltip();
        }
    }

    public void HideUI()
    {
        ShowCraftingUI(false);
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
