using UnityEngine;
using UnityEngine.UI;

public class NPCHealthCanvas : MonoBehaviour
{
    private Transform _cameraTransform;

    private bool _keepActive = false;
    private float _activeDuration = 2.0f;
    private TimerObject _activeTimer = new TimerObject();

    private Transform _healthPanel;
    private Slider _healthSlider;
    private NPCStats _enemyStats;

    private void Awake()
    {
        _healthPanel = transform.Find("HealthPanel");
        _healthSlider = GetComponentInChildren<Slider>();
        _enemyStats = GetComponentInParent<NPCStats>();
    }

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        UpdateHealthSlider(_enemyStats.EnemyHealth.GetFinalValue());
        DeactivateHealthSlider();
    }

    private void Update()
    {
        activatedProcess();
    }

    public void UpdateHealthSlider(float value)
    {
        if (value < 0.0f)
            return;

        _healthSlider.minValue = 0.0f;
        _healthSlider.maxValue = _enemyStats.EnemyHealth.GetFinalValue();

        _healthSlider.value = value;
    }

    private void activatedProcess()
    {
        if (_keepActive)
            return;

        if (!_healthPanel.gameObject.activeSelf)
            return;

        if (_activeTimer.Update())
            _healthPanel.gameObject.SetActive(false);
    }

    public void SetPosition(Transform transform)
    {
        if (transform == null)
            return;

        gameObject.transform.LookAt(gameObject.transform.position + _cameraTransform.forward);
        gameObject.transform.position = transform.position;
    }

    public void ActivateHealthSlider(bool keepActive = false)
    {
        if (_healthPanel == null)
            return;

        _keepActive = keepActive;

        _healthPanel.gameObject.SetActive(true);
        _activeTimer = new TimerObject(_activeDuration);
    }

    public void DeactivateHealthSlider()
    {
        if (_healthPanel == null)
            return;

        _keepActive = false;
        _healthPanel.gameObject.SetActive(false);
    }
}
