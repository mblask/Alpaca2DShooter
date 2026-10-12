using System.Collections.Generic;
using UnityEngine;

public class Workshop : MonoBehaviour, IInteractable
{
    private GamePlayCanvas _canvas;
    //private FloatingTextSingle _floatingText;
    private List<SpriteRenderer> _tableSpriteRenderer = new List<SpriteRenderer>();

    public string InteractableName => "Workshop";

    private void Awake()
    {
        _tableSpriteRenderer = new List<SpriteRenderer>(transform.Find("Table").GetComponentsInChildren<SpriteRenderer>());
    }

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateWorkshopOnCollider(collision, true);
        //if (_floatingText == null)
        //{
        //    string message = "Get close and press C\r\n to activate workshop";
        //    _floatingText = FloatingTextSpawner.CreateFloatingTextStatic
        //        (transform.position, message, Color.white, 1.0f, 8.0f, 0.5f);
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ActivateWorkshopOnCollider(collision, false);
    }

    public void ActivateWorkshopOnCollider(Collider2D collider, bool value)
    {
        ICrafting crafting = collider.GetComponent<ICrafting>();
        if (crafting == null)
            return;

        crafting.EnableCrafting(value);
        if (_canvas.CraftingIsActive() && !value)
            _canvas.ShowCraftingUI(false);
    }

    public void Highlight()
    {
        foreach (SpriteRenderer renderer in _tableSpriteRenderer)
            renderer.color = Constants.HIGHLIGHT_COLOR;
    }

    public void RemoveHighlight()
    {
        foreach (SpriteRenderer renderer in _tableSpriteRenderer)
            renderer.color = Constants.DEFAULT_COLOR;
    }

    public void Interact()
    {
        _canvas.ToggleCraftingUI();
    }
}
