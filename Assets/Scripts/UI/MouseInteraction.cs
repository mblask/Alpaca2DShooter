using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private float _playerDistance = 3.0f;
    private PlayerWeapons _playerWeapons;

    private IPointerOver _pointerOver;

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;
        _pointerOver = new PointerOver();
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

            if (Vector2.Distance(_playerWeapons.transform.position, transform.position) < _playerDistance)
                interactable.Interact();
            else
                FloatingTextSpawner.CreateFloatingTextStatic(_playerWeapons.transform.position, "Too far away!", Color.white);
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
