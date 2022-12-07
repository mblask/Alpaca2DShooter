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
        Portal.OnTriggerLockedPortal += SetupPanel;
        HidePanel();
    }

    private void OnDisable()
    {
        Portal.OnTriggerLockedPortal -= SetupPanel;
    }

    public void SetupPanel(Portal portal)
    {
        if (!_isActive)
        {
            foreach (Image image in _artefactImages)
            {
                image.gameObject.SetActive(false);
            }

            List<ArtefactItem> artefactsList = portal.GetRequiredArtefacts();

            for (int i = 0; i < artefactsList.Count; i++)
            {
                if (artefactsList[i] == null)
                    continue;

                if (_playerArtefacts.GetArtefacts().Contains(artefactsList[i]))
                    continue;

                _artefactImages[i].gameObject.SetActive(true);
                _artefactImages[i].sprite = artefactsList[i].ItemSprite;
            }

            ShowPanel();

            Invoke("HidePanel", 3.0f);
        }
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
