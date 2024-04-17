using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputButtonSelectorContainer : MonoBehaviour
{
    private List<InputButtonSelector> _inputButtonSelectors = new List<InputButtonSelector>();

    private void Start()
    {
        _inputButtonSelectors = transform.GetComponentsInChildren<InputButtonSelector>().ToList();
        foreach (InputButtonSelector selector in _inputButtonSelectors)
            selector.SetContainer(this);
    }

    public void DisableAllSelectors()
    {
        foreach (InputButtonSelector selector in _inputButtonSelectors)
            selector.KeySelectionOpen = false;
    }
}
