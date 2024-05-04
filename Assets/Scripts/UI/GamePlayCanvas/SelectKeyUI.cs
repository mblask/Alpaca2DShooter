using TMPro;
using UnityEngine;

public class SelectKeyUI : MonoBehaviour, IUiObject
{
    private static SelectKeyUI _instance;
    public static SelectKeyUI Instance => _instance;

    private string _selectKeyString = "Select a key";
    private string _keyUsedString = "Key already used";

    private Transform _containerTransform;
    private TextMeshProUGUI _textMesh;

    private TimerObject _timerObject;
    private float _uiDuration = 1.0f;

    public bool IsActive => _containerTransform.gameObject.activeSelf;
 
    private void Awake()
    {
        _instance = this;
        _containerTransform = transform.Find("Container");
        _textMesh = _containerTransform.Find("Text").GetComponent<TextMeshProUGUI>();
        _textMesh.SetText(_selectKeyString);

        _timerObject = new TimerObject(_uiDuration);
    }

    private void Update()
    {
        if (!_timerObject.Update())
            return;
        
        _textMesh.SetText(_selectKeyString);
    }

    public void ShowKeyUsedUI()
    {
        _textMesh.SetText(_keyUsedString);
        _containerTransform.gameObject.SetActive(true);
        _timerObject.Reset();
    }

    public void ShowSelectKeyUI()
    {
        _textMesh.SetText(_selectKeyString);
        _containerTransform.gameObject.SetActive(true);
        GamePlayCanvas.AddOpenUiStatic(this);
    }

    public void HideUI()
    {
        _containerTransform.gameObject.SetActive(false);
        GamePlayCanvas.RemoveOpenUiStatic(this);
    }
}
