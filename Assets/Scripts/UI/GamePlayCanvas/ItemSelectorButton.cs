using TMPro;
using UnityEngine;

public class ItemSelectorButton : MonoBehaviour
{
    private DataItem _item;
    private AlpacaButtonUI _button;
    private TextMeshProUGUI _buttonText;

    private TerminalUI _terminalUI;

    private void Awake()
    {
        _button = GetComponent<AlpacaButtonUI>();
        _buttonText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _terminalUI = TerminalUI.Instance;
    }

    private void Start()
    {
        _button.onLeftClick = () => { _terminalUI.AddDataItem(_item); };
    }

    public void SetItem(DataItem item)
    {
        _item = item;
        _buttonText.SetText(item.ItemName);
    }
}
