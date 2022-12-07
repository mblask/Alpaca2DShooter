using UnityEngine;

public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
    [Range(0.0f, 100.0f)] public float ChanceToDrop;
    public Color Color;
    public SFXClip PickupAudio;

    public virtual void UseItem()
    {
        Debug.Log("Use: " + ItemName);
    }
}
