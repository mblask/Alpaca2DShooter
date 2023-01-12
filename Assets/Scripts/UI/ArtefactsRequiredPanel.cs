using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtefactsRequiredPanel : MonoBehaviour
{
    private Transform _containerTransform;
    private Image[] _artefactImages;
    private RectTransform _rectTransform;

    private Vector2 _originalPosition;
    private Vector2 _displayPosition = new Vector2(0.0f, -80.0f);

    private bool _isActive = false;

    private PlayerArtefacts _playerArtefacts;

    private void Awake()
    {
        _containerTransform = transform.Find("Container");
        _artefactImages = _containerTransform.Find("ArtefactImagesContainer").GetComponentsInChildren<Image>();
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;

    }

    private void Start()
    {
        _playerArtefacts = PlayerArtefacts.Instance;
        HidePanel();
    }

    public void SetupPanel()
    {

    }

    public void ShowPanel()
    {
        _isActive = true;
        _rectTransform.anchoredPosition = _displayPosition;
    }

    public void HidePanel()
    {
        _isActive = false;
        _rectTransform.anchoredPosition = _originalPosition;
    }
}
