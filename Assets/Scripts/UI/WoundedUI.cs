using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WoundedUI : MonoBehaviour
{
    private Transform _container;
    private Transform _woundsContainer;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _container = transform.Find("Container");
        _woundsContainer = _container.Find("WoundsContainer");
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;

        PlayerHitManager.Instance.OnPlayerWounded += activateUI;
        PlayerHitManager.Instance.OnPlayerWoundsHealed += activateUI;
    }

    private void OnDisable()
    {

        PlayerHitManager.Instance.OnPlayerWounded -= activateUI;
        PlayerHitManager.Instance.OnPlayerWoundsHealed -= activateUI;
    }

    private void activateUI()
    {
        if (addWounds())
        {
            showUIContainer(true);
            return;
        }

        showUIContainer(false);
    }

    private void showUIContainer(bool value)
    {
        _container.gameObject.SetActive(value);
    }

    private bool addWounds()
    {
        foreach (Transform woundTextTransform in _woundsContainer)
        {
            Destroy(woundTextTransform.gameObject);
        }

        List<WoundType> woundsList = PlayerHitManager.GetWoundsListStatic();

        if (woundsList.Count == 0)
            return false;

        foreach (WoundType wound in woundsList)
        {
            Transform woundTextTransform = Instantiate(_gameAssets.WoundsTextObject, _woundsContainer);
            TextMeshProUGUI woundText = woundTextTransform.Find("Text").GetComponent<TextMeshProUGUI>();
            woundText.SetText(getWoundName(wound));
            woundText.color = Color.red;
        }

        return true;
    }

    private string getWoundName(WoundType woundType)
    {
        return woundType.ToString();
    }
}
