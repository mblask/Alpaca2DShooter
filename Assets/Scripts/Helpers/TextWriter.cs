using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using TMPro;

public class TextWriter : MonoBehaviour
{
    private static TextWriter _instance;
    private List<TextWriterSingle> _textWriterSingleList;

    private void Awake()
    {
        _instance = this;
        _textWriterSingleList = new List<TextWriterSingle>();
    }

    public static TextWriterSingle AddWriterStatic(TextMeshProUGUI textMesh, string textToWrite, float letterInterval, bool removeWriterBeforeAdd, Action onWritingComplete)
    {
        if (removeWriterBeforeAdd)
        {
            _instance.removeWriter(textMesh);
        }
        return _instance.addWriter(textMesh, textToWrite, letterInterval, onWritingComplete);
    }

    private TextWriterSingle addWriter(TextMeshProUGUI textMesh, string textToWrite, float letterInterval, Action onWritingComplete)
    {
        TextWriterSingle newTextWriterSingle = new TextWriterSingle(textMesh, textToWrite, letterInterval, onWritingComplete);
        _textWriterSingleList.Add(newTextWriterSingle);

        return newTextWriterSingle;
    }

    public static void RemoveWriterStatic(TextMeshProUGUI textMesh)
    {
        _instance.removeWriter(textMesh);
    }

    private void removeWriter(TextMeshProUGUI textMesh)
    {
        for (int i = 0; i < _textWriterSingleList.Count; i++)
        {
            if (_textWriterSingleList[i].GetTextMesh().Equals(textMesh))
            {
                _textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < _textWriterSingleList.Count; i++)
        {
            bool isComplete = _textWriterSingleList[i].Update();
            if (isComplete)
            {
                _textWriterSingleList.RemoveAt(i);
                i--;
            }
        }        
    }

    public class TextWriterSingle
    {
        private string _textToWrite;
        private StringBuilder _stringBuilder = new StringBuilder();
        private TextMeshProUGUI _textMesh;
        private event Action _onWritingComplete;

        private float _textWritingTimer;
        private float _letterInterval;
        private bool _writingComplete = false;
        private int _letterIndex = 0;

        public TextWriterSingle(TextMeshProUGUI textMesh, string textToWrite, float letterInterval, Action onWritingComplete)
        {
            _textMesh = textMesh;
            _textToWrite = textToWrite;
            _letterInterval = letterInterval;
            _onWritingComplete = onWritingComplete;
        }

        //returns true on completion
        public bool Update()
        {
            _textWritingTimer -= Time.deltaTime;

            if (!_writingComplete && _textWritingTimer <= 0.0f)
            {
                _stringBuilder.Append(_textToWrite[_letterIndex]);
                _textMesh.SetText(_stringBuilder.ToString());

                _letterIndex++;
                _textWritingTimer = _letterInterval;

                if (_letterIndex >= _textToWrite.Length)
                {
                    _writingComplete = true;
                    _onWritingComplete?.Invoke();
                    return true;
                }

            }

            return false;
        }

        public TextMeshProUGUI GetTextMesh()
        {
            return _textMesh;
        }

        public bool IsActive()
        {
            return _letterIndex < _textToWrite.Length;
        }

        public void WriteAllAndDestroy()
        {
            _textMesh.text = _textToWrite;
            _letterIndex = _textToWrite.Length;
            _onWritingComplete?.Invoke();
            TextWriter.RemoveWriterStatic(_textMesh);
        }
    }
}
