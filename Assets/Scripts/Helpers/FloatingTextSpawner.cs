using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingTextSpawnerStatic
{
    public static FloatingTextSingle Create(Vector3 spawnPosition, string textToWrite, Color fontColor, float destroyAfter = 1.0f, float fontSize = 4.0f, float floatSpeed = 1.0f, FloatDirection floatDirection = FloatDirection.UpRight)
    {
        GameObject floatingTextObject = new GameObject("FloatingTextObject", typeof(FloatingTextSingle));
        FloatingTextSingle floatingText = floatingTextObject.GetComponent<FloatingTextSingle>();
        floatingText.Setup(spawnPosition, textToWrite, fontColor, fontSize, destroyAfter, floatSpeed, floatDirection);

        return floatingText;
    }
}

public class FloatingTextSpawner : MonoBehaviour
{
    private static FloatingTextSpawner _instance;
    [SerializeField] private List<FloatingTextSingle> _floatingTextObjectsList;

    private void Awake()
    {
        _instance = this;
        _floatingTextObjectsList = new List<FloatingTextSingle>();
    }

    private void Update()
    {
        for (int i = 0; i < _floatingTextObjectsList.Count; i++)
        {
            if (_floatingTextObjectsList[i].UpdateText())
            {
                Destroy(_floatingTextObjectsList[i].gameObject);
                _floatingTextObjectsList.RemoveAt(i);
                i--;
            }
        }
    }

    public static FloatingTextSingle CreateFloatingTextStatic(Vector3 spawnPosition, string textToWrite, Color fontColor, float destroyAfter = 0.7f, float fontSize = 4.0f, float floatSpeed = 1.0f, bool storeInSpawner = true, FloatDirection floatDirection = FloatDirection.UpRight)
    {
        if (_instance == null)
            Debug.LogError("There is no Floating Text object in the scene!");

        return _instance.createFloatingTextMesh(spawnPosition, textToWrite, fontColor, destroyAfter, fontSize, floatSpeed, storeInSpawner, floatDirection);
    }

    private FloatingTextSingle createFloatingTextMesh(Vector3 spawnLocation, string textToWrite, Color fontColor, float destroyAfter, float fontSize, float floatSpeed, bool storeInSpawner, FloatDirection floatDirection)
    {
        GameObject floatingTextObject = new GameObject("FloatingTextObject", typeof(FloatingTextSingle));
        FloatingTextSingle floatingText = floatingTextObject.GetComponent<FloatingTextSingle>();
        floatingText.Setup(spawnLocation, textToWrite, fontColor, fontSize, destroyAfter, floatSpeed, floatDirection);
        if (storeInSpawner)
            _floatingTextObjectsList.Add(floatingText);

        return floatingText;
    }
}

public class FloatingTextSingle : MonoBehaviour
{
    private TextMeshPro _textMesh;
    private Vector3 _floatDirection;
    private float _floatSpeed;

    private float _destroyAfterTimer;

    private void Awake()
    {
        _textMesh = gameObject.AddComponent<TextMeshPro>();
    }

    public void Setup(Vector3 spawnPosition, string textToWrite, Color color, float fontSize, float destroyAfter, float floatSpeed, FloatDirection floatDirection)
    {
        transform.position = spawnPosition;
        _textMesh.color = color;
        _textMesh.fontSize = fontSize;
        _textMesh.fontStyle = FontStyles.Bold;
        _textMesh.alignment = TextAlignmentOptions.Center;
        _textMesh.text = textToWrite;

        _destroyAfterTimer = destroyAfter;

        _floatSpeed = floatSpeed;
        setFloatDirection(floatDirection);

        _textMesh.sortingLayerID = SortingLayer.NameToID("Foreground");
        _textMesh.sortingOrder = 50;
    }

    public bool UpdateText()
    {
        transform.position += _floatDirection * _floatSpeed * Time.deltaTime;

        Vector3 scaleDecreaseFactor = 0.1f * new Vector3(1.0f, 1.0f, 0.0f) / _destroyAfterTimer;
        Vector3 scale = transform.localScale - scaleDecreaseFactor * Time.deltaTime;
        transform.localScale = scale;

        //return true when ready to be destroyed
        _destroyAfterTimer -= Time.deltaTime;
        if (_destroyAfterTimer <= 0.0f)
            return true;

        return false;
    }

    public void SetText(string text)
    {
        _textMesh.text = text;
    }

    private void setFloatDirection(FloatDirection direction)
    {
        switch (direction)
        {
            case FloatDirection.Up:
                _floatDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f));
                break;
            case FloatDirection.Down:
                _floatDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, -0.5f));
                break;
            case FloatDirection.Left:
                _floatDirection = new Vector3(Random.Range(-1.0f, -0.5f), Random.Range(-1.0f, 1.0f));
                break;
            case FloatDirection.Right:
                _floatDirection = new Vector3(Random.Range(0.5f, 1.0f), Random.Range(-1.0f, 1.0f));
                break;
            case FloatDirection.UpRight:
                _floatDirection = new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
                break;
            case FloatDirection.UpLeft:
                _floatDirection = new Vector3(Random.Range(-1.0f, -0.5f), Random.Range(0.5f, 1.0f));
                break;
            case FloatDirection.DownRight:
                _floatDirection = new Vector3(Random.Range(0.5f, 1.0f), Random.Range(-1.0f, -0.5f));
                break;
            case FloatDirection.DownLeft:
                _floatDirection = new Vector3(Random.Range(-1.0f, -0.5f), Random.Range(-1.0f, -0.5f));
                break;
            case FloatDirection.Any:
                _floatDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                break;
        }

        _floatDirection.Normalize();
    }
}