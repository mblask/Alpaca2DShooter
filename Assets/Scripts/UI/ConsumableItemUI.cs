using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableItemUI : MonoBehaviour
{
    private Image _image;
    [SerializeField] private Item _item;

    private Color _partiallyTransparent = new Color(1.0f, 1.0f, 1.0f, 0.2f);
    private Color _opaque = Color.white;

    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
    }

    private void Start()
    {
        setupUI();
    }

    private void setupUI()
    {
        _image.sprite = _item.ItemSprite;
        _image.color = _partiallyTransparent;
        _image.enabled = true;
    }

    public void EnableUI()
    {
        _image.color = _opaque;
    }

    public void DisableUI()
    {
        _image.color = _partiallyTransparent;
    }

    public Item GetItem()
    {
        return _item;
    }
}
