using AlpacaMyGames;
using TMPro;
using UnityEngine;

public class InteractableTooltipUI : MonoBehaviour
{
    private static InteractableTooltipUI _instance;
    public static InteractableTooltipUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private TextMeshProUGUI _text;
    private Transform _objectToFollow;

    private void Awake()
    {
        _instance = this;
        _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_objectToFollow == null)
            return;

        Vector2 screenPos = Utilities.GetScreenPositionFromWorld2D(_objectToFollow.position) + Vector2.up * 10.0f;
        transform.position = screenPos;
    }

    public void SetTooltip(Transform objectToFollow, string text)
    {
        _objectToFollow = objectToFollow;
        _text.gameObject.SetActive(true);
        _text.SetText(text);
    }

    public void HideTooltip()
    {
        _text.gameObject.SetActive(false);
    }
}
