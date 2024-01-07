using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
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
    }

    public static void RemoveTooltipStatic()
    {
        _instance?.RemoveTooltip();
    }
    
    public void RemoveTooltip()
    {
        if (_animator.GetBool(IS_ACTIVE_STRING))
            _animator.SetBool(IS_ACTIVE_STRING, false);
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
            case JunkItem:
                readJunkItem();
                break;
            case DataItem:
                readDataItem();
                break;
        }
    }

    private void readConsumableItem(ConsumableItem consumable)
    {
        if (consumable.LifeRestored != Vector2.zero)
        {
            _sb.Append("Restores Life:");
            _sb.AppendLine();
            _sb.Append(consumable.LifeRestored.x.ToString() + " to " + consumable.LifeRestored.y.ToString());
            _sb.AppendLine();
        }

        if (consumable.StaminaRestored != Vector2.zero)
        {
            _sb.Append("Restores Stamina:");
            _sb.AppendLine();
            _sb.Append(consumable.StaminaRestored.x.ToString() + " to " + consumable.StaminaRestored.y.ToString());
        }

        if (consumable.LimbPatcher)
        {
            _sb.Append("Limb Patcher");
            _sb.AppendLine();
        }

        if (consumable.LimbToughnessDuration != Vector2.zero)
        {
            char sign = Mathf.Sign(consumable.LimbToughnessDuration.x) > 0.0f ? '+' : '-';
            _sb.Append("Toughness " + sign + consumable.LimbToughnessDuration.x * 100.0f + "% (" + consumable.LimbToughnessDuration.y + "s)");
        }
    }

    private void readInstantaneousItem(InstantaneousItem instantaneous)
    {
        if (instantaneous.LifeRestored != Vector2.zero)
        {
            _sb.Append("Restores Life:");
            _sb.AppendLine();
            _sb.Append(instantaneous.LifeRestored.x.ToString() + " to " + instantaneous.LifeRestored.y.ToString());
            _sb.AppendLine();
        }

        if (instantaneous.StaminaRestored != Vector2.zero)
        {
            _sb.Append("Restores Stamina:");
            _sb.AppendLine();
            _sb.Append(instantaneous.StaminaRestored.x.ToString() + " to " + instantaneous.StaminaRestored.y.ToString());
        }
    }

    private void readWeaponItem(WeaponItem weaponItem)
    {
        _sb.Append("Damage: ");
        _sb.Append(weaponItem.WeaponDamage.x.ToString());
        _sb.Append(" - ");
        _sb.Append(weaponItem.WeaponDamage.y.ToString());

        if (weaponItem.MagazineBullets > 0)
        {
            _sb.AppendLine();
            _sb.Append("Mag. Capacity: ");
            _sb.Append(weaponItem.MagazineBullets.ToString());
        }

        if (weaponItem.Automatic)
        {
            _sb.AppendLine();
            _sb.Append("Automatic");
        }

        if (weaponItem.StrengthRequired > 0)
        {
            _sb.AppendLine();
            _sb.Append("Strength: ");
            _sb.Append(weaponItem.StrengthRequired.ToString());
        }

        requiredStats(weaponItem);
    }

    private void readJunkItem()
    {
        _sb.AppendLine();
        _sb.AppendLine("Junk item");
        _sb.AppendLine("Used in crafting");
    }

    private void readDataItem()
    {
        _sb.AppendLine();
        _sb.AppendLine("Data item");
        _sb.AppendLine("Can be read or\nrun on terminals");
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
