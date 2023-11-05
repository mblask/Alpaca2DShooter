using UnityEngine;

public class Workshop : MonoBehaviour, IWorkshop
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateWorkshopOnCollider(collision, true);
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

        if (value)
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Press C\r\n to activate workshop", Color.white, 1.0f, 5.0f, 0.5f);

        crafting.EnableCrafting(value);
    }
}
