using UnityEngine;

[CreateAssetMenu(fileName = "Artefact Item", menuName = "Scriptable Objects / Non Inventory Item / Artefact", order = 2)]
public class ArtefactItem : NonInventoryItem
{
    public override bool UseItem()
    {
        if (PlayerArtefacts.Instance == null)
            return false;

        return PlayerArtefacts.Instance.AddArtefact(this);
    }
}
