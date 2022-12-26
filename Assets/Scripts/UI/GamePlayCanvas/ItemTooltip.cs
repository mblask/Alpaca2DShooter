using System.Text;
using System.Collections;
using System.Collections.Generic;
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

    [Range(25.0f, 100.0f)]public float ImageRotationSpeed = 50.0f;

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
        float angle = _itemImage.transform.rotation.eulerAngles.z + ImageRotationSpeed * Time.deltaTime;
        Vector3 eulerAngles = new Vector3(0.0f, 0.0f, angle);
        _itemImage.transform.eulerAngles = eulerAngles;
    }

    public static void SetupTooltipStatic(Item item)
    {
        _instance.setupTooltip(item);
    }

    private void setupTooltip(Item item)
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
        _instance.removeTooltip();
    }
    
    private void removeTooltip()
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

        ArtefactItem artefact = item as ArtefactItem;

        if (artefact != null)
        {
            _sb.Append("Artefact");

            return;
        }

        ConsumableItem consumable = item as ConsumableItem;

        if (consumable != null)
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

            return;
        }

        InstantaneousItem instantaneous = item as InstantaneousItem;

        if (instantaneous != null)
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

            return;
        }

        WeaponItem weaponItem = item as WeaponItem;

        if (weaponItem != null)
        {
            _sb.Append("Damage: ");
            _sb.Append(weaponItem.WeaponDamage.x.ToString());
            _sb.Append(" - ");
            _sb.Append(weaponItem.WeaponDamage.y.ToString());

            _sb.AppendLine();
            _sb.Append("Durability: ");
            _sb.Append(weaponItem.MaxDurability.ToString());

            _sb.AppendLine();
            _sb.Append("Mag. Capacity: ");
            _sb.Append(weaponItem.MagazineBullets.ToString());

            if (weaponItem.Automatic)
            {
                _sb.AppendLine();
                _sb.Append("Automatic");
            }

            _sb.AppendLine();
            _sb.Append("Strength: ");
            _sb.Append(weaponItem.StrengthRequired.ToString());

            requiredStats(weaponItem);

            return;
        }
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
