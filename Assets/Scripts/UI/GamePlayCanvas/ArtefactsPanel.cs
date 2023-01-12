using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtefactsPanel : MonoBehaviour
{
    private Text _artefactsCollectedText;

    private int _maxNumOfArtefacts;

    private void Awake()
    {
        _artefactsCollectedText = transform.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        PlayerArtefacts.Instance.OnArtefactCollected += UpdateArtefactsText;
    }

    private void OnDisable()
    {
        if (PlayerArtefacts.Instance != null)
            PlayerArtefacts.Instance.OnArtefactCollected -= UpdateArtefactsText;
    }

    public void SetupInitialText(int maxNumber)
    {
        _maxNumOfArtefacts = maxNumber;
        _artefactsCollectedText.text = 0.ToString() + " / " + maxNumber.ToString();
    }

    public void UpdateArtefactsText(int value)
    {
        _artefactsCollectedText.text = value.ToString() + " / " + _maxNumOfArtefacts.ToString();
    }
}
