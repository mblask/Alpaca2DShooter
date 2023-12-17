using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private PlayerWeapons _playerWeapons;
    private PointerOver _pointerOver;

    private InteractableTooltipUI _interactionTooltip;

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;
        _interactionTooltip = InteractableTooltipUI.Instance;
        _pointerOver = new PointerOver();
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

        if (_pointerOver.OverUI())
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
            _interactionTooltip.SetTooltip(collision.transform, interactable.InteractableName);
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
            _interactionTooltip.HideTooltip();
        }

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        if (pickupItem != null)
            ItemTooltip.RemoveTooltipStatic();
    }
}
