using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesUI : MonoBehaviour, IUiObject
{
    private Transform _container;
    private Transform _itemContainer;

    private GameAssets _gameAssets;
    private PlayerInventory _playerInventory;
    private GamePlayCanvas _canvas;
    private IPointerOver _pointerOver;

    private void Awake()
    {
        _container = transform.Find("Container");
        _itemContainer = _container.Find("ItemContainer");
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _playerInventory = PlayerInventory.Instance;
        _canvas = GamePlayCanvas.Instance;
        _pointerOver = PointerOver.GetInstance();
    }

    public void ShowUI()
    {
        bool active = !_container.gameObject.activeSelf;
        _container.gameObject.SetActive(active);

        if (active)
        {
            populateItemContainer();
            GamePlayCanvas.AddOpenUiStatic(this);
        }

        
        if (!active)
        {
            _canvas.RemoveItemTooltip();
            GamePlayCanvas.RemoveOpenUiStatic(this);
        }
    }

    public void HideUI()
    {
        _container.gameObject.SetActive(false);
        _canvas.RemoveItemTooltip();
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
