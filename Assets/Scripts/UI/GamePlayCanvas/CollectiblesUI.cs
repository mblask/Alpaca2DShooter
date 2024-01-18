using System.Collections.Generic;
using UnityEngine;

public class CollectiblesUI : MonoBehaviour
{
    private Transform _container;
    private Transform _itemContainer;

    private GameAssets _gameAssets;
    private PlayerInventory _playerInventory;

    private void Awake()
    {
        _container = transform.Find("Container");
        _itemContainer = _container.Find("ItemContainer");
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _playerInventory = PlayerInventory.Instance;
    }

    public void ShowUI()
    {
        bool active = !_container.gameObject.activeSelf;
        _container.gameObject.SetActive(active);

        if (active)
            populateItemContainer();
    }

    private void populateItemContainer()
    {
        foreach (Transform item in _itemContainer)
            Destroy(item.gameObject);

        List<Item> collectibles = _playerInventory.GetCollectibles();
        foreach (Item item in collectibles)
        {
            CollectibleItemSlotUI slot = 
                Instantiate(_gameAssets.CollectibleItemSlotUI, _itemContainer).GetComponent<CollectibleItemSlotUI>();
            slot.AddItemToSlot(item);
        }
    }
}
