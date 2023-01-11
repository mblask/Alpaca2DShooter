using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private float _playerDistance = 3.0f;
    private PlayerBase _playerBase;

    private void Start()
    {
        _playerBase = PlayerBase.Instance;
    }

    private void Update()
    {
        onLeftMouseClick();
    }

    private void onLeftMouseClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        if (hits.Length == 0)
            return;


        foreach (Collider2D collider in hits)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            if (Vector2.Distance(_playerBase.transform.position, transform.position) < _playerDistance)
            {
                interactable.Interact();
                Debug.Log("Mouse Interaction");
            }
            else
                FloatingTextSpawner.CreateFloatingTextStatic(_playerBase.transform.position, "Too far away!", Color.white);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            NPC_AI npcAI = collision.transform.GetComponent<NPC_AI>();
            if (npcAI == null || (npcAI != null && npcAI.GetNPCType().Equals(NPCType.Ally)))
                PlayerWeapons.EnableShootingStatic(false);

            interactable.Highlight();
        }

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        
        if (pickupItem != null)
            ItemTooltip.SetupTooltipStatic(pickupItem.GetItem());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            PlayerWeapons.EnableShootingStatic(true);
            interactable.RemoveHighlight();
        }

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        
        if (pickupItem != null)
            ItemTooltip.RemoveTooltipStatic();
    }
}
