using UnityEngine;

public class ShoppingCloseButton : MonoBehaviour
{
    private GamePlayCanvas _canvas;
    private AlpacaButtonUI _closeButton;

    private void Awake()
    {
        _closeButton = transform.GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;

        _closeButton.onLeftClick = () => { _canvas.ShowShoppingUI(false); };
    }


}
