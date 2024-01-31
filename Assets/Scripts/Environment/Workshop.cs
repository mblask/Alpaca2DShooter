using UnityEngine;

public class Workshop : MonoBehaviour
{
    private GamePlayCanvas _canvas;
    private FloatingTextSingle _floatingText;

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateWorkshopOnCollider(collision, true);
        if (_floatingText == null)
        {
            _floatingText = FloatingTextSpawner.CreateFloatingTextStatic
                (transform.position, "Press C\r\n to activate workshop", Color.white, 1.0f, 8.0f, 0.5f);
        }
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
}
