using UnityEngine;

[CreateAssetMenu(fileName = "Data Item", menuName = "Scriptable Objects / Inventory Item / DataItem", order = 4)]
public class DataItem : InventoryItem
{
    [Header("Data Item charactersitics")]
    public bool ComputerUsable;
    public AudioClip AudioClip;
    [TextArea]
    public string Text;
}
