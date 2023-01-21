using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workshop : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICrafting crafting = collision.GetComponent<ICrafting>();
        if (crafting == null)
            return;

        crafting.EnableCrafting(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ICrafting crafting = collision.GetComponent<ICrafting>();
        if (crafting == null)
            return;

        crafting.EnableCrafting(false);
    }
}
