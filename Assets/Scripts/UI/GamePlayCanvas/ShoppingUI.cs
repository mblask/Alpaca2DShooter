using UnityEngine;

public class ShoppingUI : MonoBehaviour
{
    private Transform _shopUi;
    private Transform _buyItemUi;

    private void Awake()
    {
        _shopUi = transform.Find("ShopUI");
        _buyItemUi = transform.Find("BuyItemUI");
    }

    public void ShowShopUI(bool value)
    {
        _shopUi.gameObject.SetActive(value);
    }

    public void ShowBuyItemUI(bool value)
    {
        _buyItemUi.gameObject.SetActive(value);
    }
}
