using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextConsoleUI : MonoBehaviour
{
    private static TextConsoleUI _instance;
    public static TextConsoleUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private Button _nextButton;
    private Button _previousButton;
    private Animator _animator;

    private bool _isActive = false;
    private bool _connectedToTerminal = false;
    private TextMeshProUGUI _textMesh;
    private string _completeTextToWrite;
    private string[] _textParagraphs;
    private int _currentParagraph = 0;

    private IPointerOver _pointerOver;

    private void Awake()
    {
        _instance = this;
        _pointerOver = PointerOver.GetInstance();

        _animator = GetComponent<Animator>();
        _textMesh = transform.Find("ScrollView").Find("Viewport").Find("Content").Find("Text").GetComponent<TextMeshProUGUI>();
        _nextButton = transform.Find("NextButton").GetComponent<Button>();
        _previousButton = transform.Find("PreviousButton").GetComponent<Button>();

        _nextButton.onClick.AddListener(nextParagraph);
        _previousButton.onClick.AddListener(previousParagraph);
    }

    private void Update()
    {
        switchParagraphs();
        closeUiOnInput();
    }
    
    private void switchParagraphs()
    {
        if (!Input.GetKeyUp(KeyCode.Space))
            return;

        if (_textParagraphs == null || _textParagraphs.Length == 0)
            return;

        incrementParagraph(+1);
        string nextParagraph = _textParagraphs[_currentParagraph];
        _textMesh.SetText(nextParagraph);
    }

    private void closeUiOnInput()
    {
        bool clickAndClose = Input.GetMouseButtonUp(0) && !_pointerOver.OverUI();
        bool escapeClose = Input.GetKeyUp(KeyCode.Escape);

        if (!clickAndClose && !escapeClose)
            return;

        CloseUI(escapeClose);
    }

    public void CloseUI(bool instantClose)
    {
        _completeTextToWrite = string.Empty;
        _textParagraphs = new string[] { };
        _isActive = false;

        if (instantClose)
            _animator.SetTrigger("Default");
        else
            _animator.SetBool("IsActive", _isActive);
    }

    private void incrementParagraph(int increment)
    {
        _currentParagraph += increment;
        if (_currentParagraph < 0)
            _currentParagraph = _textParagraphs.Length - 1;

        if (_currentParagraph >= _textParagraphs.Length)
            _currentParagraph = 0;
    }

    private void nextParagraph()
    {
        incrementParagraph(+1);
        _textMesh.SetText(_textParagraphs[_currentParagraph]);
    }

    private void previousParagraph()
    {
        incrementParagraph(-1);
        _textMesh.SetText(_textParagraphs[_currentParagraph]);
    }

    public void TextToWrite(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        _completeTextToWrite = text;
        _textParagraphs = new string[] { };
        _textParagraphs = _completeTextToWrite.Split("\n");
        _isActive = true;
        _textMesh.SetText(_textParagraphs[0]);
        _animator.SetBool("IsActive", _isActive);
        _animator.SetTrigger("TriggerAnimation");
    }
}
