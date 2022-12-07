using UnityEngine;

[CreateAssetMenu(fileName = "Artefact Item", menuName = "Scriptable Objects / Non Inventory Item / Artefact", order = 2)]
public class ArtefactItem : NonInventoryItem
{
    public override void UseItem()
    {
        PlayerArtefacts.Instance?.AddArtefact(this);
    }
}
