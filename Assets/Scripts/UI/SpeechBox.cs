using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SpeechBox : MonoBehaviour
{
    private static SpeechBox _instance;
    public static SpeechBox Instance
    {
        get
        {
            return _instance;
        }
    }

    private List<string> _messageList;
    private string _currentMessage;
    private TextWriter.TextWriterSingle _textWriterSingle;

    private Transform _containerTransform;
    private RectTransform _contentTransform;
    private TextMeshProUGUI _textMesh;

    private float _chatBoxHeight;

    private bool _writingComplete = true;

    private float _scrollingMultiplier = 10.0f;
    private float _letterWritingInterval = 0.05f;

    private int _currentMessageIndex = 0;

    private void Awake()
    {
        _instance = this;
        _containerTransform = transform.Find("Container");
        _contentTransform = _containerTransform.Find("TextScrollView").Find("Viewport").Find("Content").GetComponent<RectTransform>();
        _textMesh = _contentTransform.Find("TextObject").GetComponent<TextMeshProUGUI>();

        _chatBoxHeight = transform.GetComponent<RectTransform>().rect.height;
    }

    private void onBoxActivated()
    {
        if (_textWriterSingle != null && _textWriterSingle.IsActive())
        {
            _textWriterSingle.WriteAllAndDestroy();
        }
        else
        {
            _textWriterSingle = TextWriter.AddWriterStatic(_textMesh, _currentMessage, _letterWritingInterval, true, onWritingComplete);
            _writingComplete = false;
        }
    }

    public static void WriteCurrentMessageCompletelyStatic()
    {
        _instance.writeCurrentMessageCompletely();
    }

    private void writeCurrentMessageCompletely()
    {
        if (_textWriterSingle != null && _textWriterSingle.IsActive())
        {
            _textWriterSingle.WriteAllAndDestroy();
        }
    }
    
    private void Update()
    {
        textScrolling();
    }

    private void textScrolling()
    {
        if (_textMesh == null)
            return;

        if (!_writingComplete && _contentTransform.rect.height > _chatBoxHeight)
        {
            Vector3 contentPosition = _contentTransform.transform.position;
            contentPosition.y += Time.deltaTime * _scrollingMultiplier / _letterWritingInterval;
            _contentTransform.transform.position = contentPosition;
        }
    }

    private void onWritingComplete()
    {
        _writingComplete = true;
        _currentMessageIndex++;
    }

    public static bool IsWriting()
    {
        return _instance.isWriting();
    }

    private bool isWriting()
    {
        return !_writingComplete;
    }

    public static void DeactivateSpeechBoxSingleStatic()
    {
        _instance.deactivateSpeechBoxSingle();
    }

    private void deactivateSpeechBoxSingle()
    {
        _writingComplete = true;
        _containerTransform.gameObject.SetActive(false);
    }

    public static void TriggerSpeechBoxOnInteractionStatic(string message)
    {
        _instance.triggerSpeechBoxOnInteraction(message);
    }

    private void triggerSpeechBoxOnInteraction(string message)
    {
        if (!isActive())
        {
            activateSpeechBoxSingle(message);
            return;
        }

        if (isWriting())
            writeCurrentMessageCompletely();
        else
            deactivateSpeechBoxSingle();
    }

    public static bool ActivateSpeechBoxSingleStatic(string message)
    {
        return _instance.activateSpeechBoxSingle(message);
    }

    private bool activateSpeechBoxSingle(string message)
    {
        if (!_containerTransform.gameObject.activeSelf)
            _containerTransform.gameObject.SetActive(true);

        if (_currentMessage != message)
            _currentMessage = message;

        onBoxActivated();

        return _writingComplete;
    }

    public static bool IsActive()
    {
        return _instance.isActive();
    }

    private bool isActive()
    {
        return _containerTransform.gameObject.activeSelf;
    }
}
