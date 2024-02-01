using UnityEngine;
using UnityEngine.UI;

public class DoorCanvas : MonoBehaviour
{
    private Transform _lockpickingPanel;
    private Slider _lockpickingSlider;

    private void Awake()
    {
        _lockpickingPanel = transform.Find("LockpickingPanel");
        _lockpickingSlider = _lockpickingPanel.Find("LockpickingSlider").GetComponent<Slider>();

        transform.rotation = Quaternion.LookRotation(Vector3.forward);

        Activate(false);
    }

    public void Activate(bool value)
    {
        _lockpickingPanel.gameObject.SetActive(value);
    }

    public bool IsActive()
    {
        return _lockpickingPanel.gameObject.activeSelf;
    }

    public void UpdateSlider(float percentageValue)
    {
        if (!IsActive())
            Activate(true);

        _lockpickingSlider.value = percentageValue;

        if (_lockpickingSlider.value < 1.0f)
            return;

        Activate(false);
        FloatingTextSpawner.CreateFloatingTextStatic
            (PlayerStats.Instance.transform.position, "Lock open!", Color.white, 2.0f, 7, 0.8f, true, FloatDirection.Up);
    }
}
