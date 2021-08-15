using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item Item;

    private string _speechBubbleText = "";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(ConstsEnums.PlayerTag))
            return;

        if (Item as WeaponItem)
        {
            PlayerController.Instance.AddWeapon(Item as WeaponItem);
        }

        if (Item as ConsumableItem)
        {
            Debug.Log("The item is a consumable item, which restores " + (Item as ConsumableItem).LifeRestored + " life.");
        }

        if (Item as ArtefactItem)
        {
            //Audio triggered
            _speechBubbleText = "Picked up " + Item.ItemName + "!";
            GamePlayCanvas.Instance.FillSpeechBubbleText(_speechBubbleText);
            GamePlayCanvas.Instance.ActivateSpeechBubble(true);
            PlayerController.Instance.AddArtefact(Item as ArtefactItem);
            _speechBubbleText = "";

            Debug.Log("The item is an artefact item, named " + Item.ItemName);
        }

        Destroy(gameObject);
    }
}
