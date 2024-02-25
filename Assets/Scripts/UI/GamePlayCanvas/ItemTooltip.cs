using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour, IUiObject
{
    private static ItemTooltip _instance;

    public static ItemTooltip Instance
    {
        get
        {
            return _instance;
        }
    }

    private float _imageRotationSpeed = 50.0f;

    private Transform _container;
    private Animator _animator;
    private Image _background;
    private Color _defaultBackgroundColor;

    private const string GO_TO_DEFAULT_STRING = "GoToDefault";
    private const string IS_ACTIVE_STRING = "IsActive";

    private TextMeshProUGUI _itemName;
    private TextMeshProUGUI _itemStats;
    private Image _itemImage;

    private StringBuilder _sb;

    private Item _lastItem;

    private void Awake()
    {
        _instance = this;

        _animator = GetComponent<Animator>();
        _container = transform.Find("Container");
        _background = _container.Find("Background").GetComponent<Image>();
        _defaultBackgroundColor = _background.color;

        _itemName = _container.Find("ItemName").GetComponent<TextMeshProUGUI>();
        _itemStats = _container.Find("ItemStats").GetComponent<TextMeshProUGUI>();
        _itemImage = _container.Find("ItemImage").GetComponent<Image>();
        _itemImage.preserveAspect = true;
    }

    private void LateUpdate()
    {
        rotateTooltipImage();
    }

    private void rotateTooltipImage()
    {
        float angle = _itemImage.transform.rotation.eulerAngles.z + _imageRotationSpeed * Time.deltaTime;
        Vector3 eulerAngles = new Vector3(0.0f, 0.0f, angle);
        _itemImage.transform.eulerAngles = eulerAngles;
    }

    public static void SetupTooltipStatic(Item item)
    {
        _instance?.SetupTooltip(item);
    }

    public void SetupTooltip(Item item)
    {
        _animator.SetTrigger(GO_TO_DEFAULT_STRING);

        _lastItem = item;
        _itemName.SetText(_lastItem.ItemName);
        _background.color = _defaultBackgroundColor;
        updateStats(_lastItem);
        _itemStats.SetText(_sb.ToString());
        _itemImage.sprite = _lastItem.ItemSprite;
        _itemImage.color = _lastItem.Color;

        _animator.SetBool(IS_ACTIVE_STRING, true);
        GamePlayCanvas.AddOpenUiStatic(this);
    }
    
    public void RemoveTooltip()
    {
        GamePlayCanvas.RemoveOpenUiStatic(this);
        if (_animator.GetBool(IS_ACTIVE_STRING))
            _animator.SetBool(IS_ACTIVE_STRING, false);
    }

    public void HideUI()
    {
        RemoveTooltip();
    }

    public Item GetTooltipItem()
    {
        return _lastItem;
    }

    private void updateStats(Item item)
    {
        _sb = new StringBuilder();

        switch (item)
        {
            case ConsumableItem consumable:
                readConsumableItem(consumable);
                break;
            case InstantaneousItem instantaneous:
                readInstantaneousItem(instantaneous);
                break;
            case WeaponItem weaponItem:
                readWeaponItem(weaponItem);
                break;
            case ThrowableItem throwableItem:
                readThrowableItem(throwableItem);
                break;
            case JunkItem junkItem:
                readJunkItem(junkItem);
                break;
            case DataItem dataItem:
                readDataItem(dataItem);
                break;
            case InventoryItem inventoryItem:
                readInventoryItem(inventoryItem);
                break;
        }
    }

    private void readInventoryItem(InventoryItem inventoryItem)
    {
        _sb.Append(inventoryItem.GetItemTooltipText());
    }

    private void readConsumableItem(ConsumableItem consumable)
    {
        _sb.Append(consumable.GetItemTooltipText());
    }

    private void readInstantaneousItem(InstantaneousItem instantaneous)
    {
        _sb.Append(instantaneous.GetItemTooltipText());
    }

    private void readWeaponItem(WeaponItem weaponItem)
    {
        _sb.Append(weaponItem.GetItemTooltipText());
        requiredStats(weaponItem);
    }

    private void readThrowableItem(ThrowableItem throwableItem)
    {
        _sb.Append(throwableItem.GetItemTooltipText());
    }

    private void readJunkItem(JunkItem junkItem)
    {
        _sb.Append(junkItem.GetItemTooltipText());
    }

    private void readDataItem(DataItem dataItem)
    {
        _sb.Append(dataItem.GetItemTooltipText());
    }

    private void requiredStats(WeaponItem weapon)
    {
        if (weapon == null)
            return;

        if (PlayerStats.GetStatByTypeStatic(StatType.Strength).GetFinalValue() >= weapon.StrengthRequired)
            return;
        
        float alfa = 0.6f;
        _background.color = new Color(0.5f, 0.0f, 0.0f, alfa);
    }
}
