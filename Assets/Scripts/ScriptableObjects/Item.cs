using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item characteristics")]
    public string ItemName;
    public Sprite ItemSprite;
    public GameObject ItemPrefab;
    [Range(0.0f, 100.0f)] public float ChanceToDrop;
    public bool IsCollectible;
    public Color Color;
    public SFXClip PickupAudio;
    [TextArea]
    public string ItemTooltipText = string.Empty;

    public virtual string GetItemTooltipText()
    {
        return string.Empty;
    }

    public virtual bool UseItem()
    {
        return true;
    }
}
