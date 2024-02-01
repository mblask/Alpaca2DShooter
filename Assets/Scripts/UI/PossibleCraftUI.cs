using UnityEngine;
using UnityEngine.UI;

public class PossibleCraftUI : MonoBehaviour
{
    private CraftingRecipe _possibleRecipe;
    private AlpacaButtonUI _button;
    private Image _image;
    private Item _possibleProductItem;

    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        _button = transform.Find("Image").GetComponent<AlpacaButtonUI>();
        _image = transform.Find("Image").GetComponent<Image>();
        _image.enabled = false;
    }

    private void Start()
    {
        _uiCanvas = GamePlayCanvas.Instance;

        _button.onLeftClick = () => {
            selectThisRecipe();
        };

        _button.onCursorEnter = () =>
        {
            _uiCanvas.SetupItemTooltip(_possibleProductItem);
        };

        _button.onCursorExit = () =>
        {
            _uiCanvas.RemoveItemTooltip();
        };
    }

    private void selectThisRecipe()
    {
        _uiCanvas.PopulateCraftingSlots(_possibleRecipe);
    }

    public void AddPossibleRecipe(CraftingRecipe recipe)
    {
        _possibleRecipe = recipe;
        _possibleProductItem = recipe.ProductItem;
        _image.sprite = recipe.ProductItem.ItemSprite;
        _image.enabled = true;
        _image.preserveAspect = true;
    }
}
