using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private PlayerWeapons _playerWeapons;
    private PointerOver _pointerOver;

    private GamePlayCanvas _canvas;

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
        _playerWeapons = PlayerWeapons.Instance;
        _pointerOver = PointerOver.GetInstance();
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

        if (_playerWeapons == null)
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

        if (_playerWeapons == null)
            return;

        _playerWeapons.LeftClickUp();
        _playerWeapons.EnableShooting(true);
    }

    private void checkInteractables()
    {
        if (_playerWeapons == null)
            return;

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
        if (_playerWeapons == null)
            return;

        _playerWeapons.EnableShooting(false);

        if (collider.GetComponent<NpcAi>() != null)
            _playerWeapons.EnableShooting(true);
    }

    private void colliderFar(Collider2D collider)
    {
        if (_playerWeapons == null)
            return;

        _playerWeapons.EnableShooting(false);

        if (collider.GetComponent<Box>() != null)
        {
            _playerWeapons.EnableShooting(true);
            return;
        }

        if (collider.GetComponent<Door>() != null)
        {
            _playerWeapons.EnableShooting(true);
            return;
        }

        if (collider.GetComponent<NpcAi>() != null)
        {
            _playerWeapons.EnableShooting(true);
            return;
        }

        FloatingTextSpawner.CreateFloatingTextStatic
            (_playerWeapons.transform.position, "Too far away!", Color.white);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Highlight();
            _canvas.SetInteractableTooltip(collision.transform, interactable.InteractableName);
        }

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        if (pickupItem != null)
            _canvas.SetupItemTooltip(pickupItem.GetItem());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.RemoveHighlight();
            _canvas.HideInteractableTooltip();
        }

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        if (pickupItem != null)
            _canvas.RemoveItemTooltip();
    }
}
