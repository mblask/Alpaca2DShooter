using UnityEngine;
using UnityEngine.UI;

public class GamePlayCanvas : MonoBehaviour
{
    private static GamePlayCanvas _instance = null;
    public static GamePlayCanvas Instance
    {
        get
        {
            return _instance;
        }
    }

    public Text AmmoText;
    public Text EnemyKillsNum;
    public Text ArtefactsCollectedText;
    public Text TimeRemainingText;
    public GameObject SpeechBubble;
    public Transform PlayerTransform;

    private Text _speechBubbleText;
    private int _enemiesKilled = 0;

    [Header("Read-only")]
    [SerializeField] private bool _speechBubbleActivated = false;
    [SerializeField] private float _bubbleLifetime = 2.0f;
    [SerializeField] private float _timer = 0.0f;

    private void Awake()
    {
        #region Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        #endregion
    }

    private void Start()
    {
        UpdateArtefactsText();
        ActivateSpeechBubble(false);
    }

    private void LateUpdate()
    {
        CheckIfBubbleActive();
        PositionSpeechBubble();
    }

    public void UpdateTime(float time)
    {
        if (time < 0.0f)
            return;

        TimeRemainingText.text = time.ToString("F1");
    }

    public void ActivateGameOverScreen()
    {
        Debug.Log("Game Over!");
    }

    public void UpdateAmmoText(int value, int total)
    {
        if (value == 0)
        {
            Debug.Log("GamePlayCanvas: No ammo. Reload!");
        }

        AmmoText.text = value.ToString() + " / " + total.ToString();
    }

    public void IncrementEnemyKillsText()
    {
        _enemiesKilled++;

        EnemyKillsNum.text = _enemiesKilled.ToString();
    }

    public void UpdateEnemyKillsText(int value)
    {
        if (value == 0)
            return;

        EnemyKillsNum.text = value.ToString();
    }

    public void FillSpeechBubbleText(string bubbleText)
    {
        if (bubbleText == "")
            return;

        _speechBubbleText= SpeechBubble.GetComponentInChildren<Text>();

        _speechBubbleText.text = bubbleText;

        ActivateSpeechBubble(true);
    }

    private void PositionSpeechBubble()
    {
        if (PlayerTransform == null)
        {
            SpeechBubble.transform.position = Vector3.zero;
            return;
        }

        Vector3 position = Camera.main.WorldToScreenPoint(PlayerTransform.position);
        SpeechBubble.transform.position = position;
    }

    public void ActivateSpeechBubble(bool value)
    {
        SpeechBubble.SetActive(value);
        _speechBubbleActivated = value;
        _timer = _bubbleLifetime;
    }

    private void CheckIfBubbleActive()
    {
        if (_speechBubbleActivated)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0.0f)
            {
                ActivateSpeechBubble(false);
            }
        }
    }
    
    public void UpdateArtefactsText()
    {
        ArtefactsCollectedText.text = PlayerController.Instance.ArtefactsCollected.Count + " / " + GameManager.Instance.ArtefactsRequiredToPassLevel.ToString();
    }

    public void UpdateArtefactsText(int value)
    {
        if (value == 0)
            return;

        ArtefactsCollectedText.text = value.ToString() + " / " + GameManager.Instance.ArtefactsRequiredToPassLevel.ToString();
    }
}
