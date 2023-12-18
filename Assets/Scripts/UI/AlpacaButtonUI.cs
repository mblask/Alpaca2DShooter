using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class AlpacaButtonUI : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image _image;
    private Color _defaultColor;
    [SerializeField] private Color _hoverColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
    [SerializeField] private Color _pressedColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);

    public Action onLeftClick { private get; set; }
    public Action onRightClick { private get; set; }
    public Action onMiddleClick { private get; set; }
    public Action onCursorEnter { private get; set; }
    public Action onCursorExit { private get; set; }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    private void OnEnable()
    {
        _image.color = _defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftClick?.Invoke();
                break;
            case PointerEventData.InputButton.Right:
                onRightClick?.Invoke();
                break;
            case PointerEventData.InputButton.Middle:
                onMiddleClick?.Invoke();
                break;
            default:
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //_defaultColor = _image.color;
        //_image.color = _pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //_image.color = _defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onCursorEnter?.Invoke();
        _defaultColor = _image.color;
        _image.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onCursorExit?.Invoke();
        _image.color = _defaultColor;
    }
}
