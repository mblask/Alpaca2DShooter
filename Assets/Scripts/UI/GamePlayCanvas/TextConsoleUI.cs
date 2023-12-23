using TMPro;
using UnityEngine;

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

    private TextMeshProUGUI _textMesh;
    private string _textToWrite;

    private void Awake()
    {
        _instance = this;
        _textMesh = transform.Find("ScrollView").Find("Viewport").Find("Content").Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void TextToWrite(string text)
    {
        _textToWrite = text;
        _textMesh.SetText(text);
    }
}
