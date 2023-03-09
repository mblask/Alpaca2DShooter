using System.Collections;
using System.Collections.Generic;
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

        crafting.EnableCrafting(value);
    }
}
