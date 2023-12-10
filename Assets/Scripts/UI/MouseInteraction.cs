using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private PlayerWeapons _playerWeapons;

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;
    }

    private void Update()
    {
        onLeftMouseDown();
        onLeftMouseUp();
    }

    private void onLeftMouseDown()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        checkInteractables();
        _playerWeapons.LeftClickDown();
    }

    private void onLeftMouseUp()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        _playerWeapons.LeftClickUp();
        _playerWeapons.EnableShooting(true);
    }

    private void checkInteractables()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D collider in hits)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            if (Vector2.Distance(_playerWeapons.transform.position, transform.position) < Constants.INTERACTION_DISTANCE)
            {
                colliderClose(collider);
                interactable.Interact();
            }
            else
            {
                colliderFar(collider);
            }
        }
    }

    private void colliderClose(Collider2D collider)
    {
        _playerWeapons.EnableShooting(false);

        if (collider.GetComponent<NPC_AI>() != null)
            _playerWeapons.EnableShooting(true);
    }

    private void colliderFar(Collider2D collider)
    {
        _playerWeapons.EnableShooting(false);

        if (collider.GetComponent<Box>() != null)
            _playerWeapons.EnableShooting(true);

        if (collider.GetComponent<NPC_AI>() != null)
            _playerWeapons.EnableShooting(true);

        FloatingTextSpawner.CreateFloatingTextStatic(_playerWeapons.transform.position, "Too far away!", Color.white);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Highlight();
            FloatingTextSpawner.CreateFloatingTextStatic(collision.transform.position, interactable.InteractableName, Color.white, destroyAfter: 1.0f, fontSize: 8, floatSpeed: 0.5f);
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
            interactable.RemoveHighlight();
            FloatingTextSpawner.RemoveLastStatic();
        }

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        if (pickupItem != null)
            ItemTooltip.RemoveTooltipStatic();
    }
}
