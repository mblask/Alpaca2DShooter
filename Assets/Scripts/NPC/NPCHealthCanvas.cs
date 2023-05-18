using UnityEngine;
using UnityEngine.UI;

public class NPCHealthCanvas : MonoBehaviour
{
    private Transform _cameraTransform;

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

    public void UpdateHealthSlider(float value)
    {
        if (value < 0.0f)
            return;

        _healthSlider.minValue = 0.0f;
        _healthSlider.maxValue = _enemyStats.EnemyHealth.GetFinalValue();

        _healthSlider.value = value;
    }

    public void SetPosition(Transform transform)
    {
        if (transform == null)
            return;

        gameObject.transform.LookAt(gameObject.transform.position + _cameraTransform.forward);
        gameObject.transform.position = transform.position;
    }

    public void ActivateHealthSlider()
    {
        if (_healthPanel == null)
            return;

        _healthPanel.gameObject.SetActive(true);

        Invoke("DeactivateHealthSlider", 2.0f);
    }

    public void DeactivateHealthSlider()
    {
        if (_healthPanel == null)
            return;

        _healthPanel.gameObject.SetActive(false);
    }
}
