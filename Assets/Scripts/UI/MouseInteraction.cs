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
        onLeftMouseDown();
        onLeftMousUp();
    }

    private void onLeftMouseDown()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        checkInteractables();
        _playerWeapons.LeftClickDown();
    }

    private void onLeftMousUp()
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

            if (Vector2.Distance(_playerWeapons.transform.position, transform.position) < _playerDistance)
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
            interactable.Highlight();

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        if (pickupItem != null)
            ItemTooltip.SetupTooltipStatic(pickupItem.GetItem());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
            interactable.RemoveHighlight();

        PickupItem pickupItem = collision.GetComponent<PickupItem>();
        if (pickupItem != null)
            ItemTooltip.RemoveTooltipStatic();
    }
}
