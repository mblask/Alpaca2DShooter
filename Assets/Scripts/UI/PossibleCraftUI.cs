using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossibleCraftUI : MonoBehaviour
{
    private CraftingRecipe _possibleRecipe;
    private AlpacaButtonUI _button;
    private Image _image;
    private Item _possibleProductItem;

    private void Awake()
    {
        _button = transform.Find("Image").GetComponent<AlpacaButtonUI>();
        _image = transform.Find("Image").GetComponent<Image>();
        _image.enabled = false;

        _button.onLeftClick = () => {
            selectThisRecipe();
        };

        _button.onCursorEnter = () =>
        {
            ItemTooltip.SetupTooltipStatic(_possibleProductItem);
        };

        _button.onCursorExit = () =>
        {
            ItemTooltip.RemoveTooltipStatic();
        };
    }

    private void selectThisRecipe()
    {
        CraftingUI.PopulateCraftingSlots(_possibleRecipe);
    }

    public void AddPossibleRecipe(CraftingRecipe recipe)
    {
        _possibleRecipe = recipe;
        _possibleProductItem = recipe.ProductItem;
        _image.sprite = recipe.ProductItem.ItemSprite;
        _image.enabled = true;
    }
}
