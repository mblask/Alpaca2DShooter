using UnityEngine;
using UnityEngine.UI;

public class TerminalCanvas : MonoBehaviour
{
    private Transform _hackingCompletionPanel;
    private Slider _hackingCompletionSlider;

    private void Awake()
    {
        _hackingCompletionPanel = transform.Find("HackingCompletionPanel");
        _hackingCompletionSlider = _hackingCompletionPanel.Find("HackingSlider").GetComponent<Slider>();

        transform.rotation = Quaternion.LookRotation(Vector3.forward);

        Activate(false);
    }

    public void Activate(bool value)
    {
        _hackingCompletionPanel.gameObject.SetActive(value);
    }

    public bool IsActive()
    {
        return _hackingCompletionPanel.gameObject.activeSelf;
    }

    public void UpdateSlider(float percentageValue)
    {
        if (!IsActive())
            Activate(true);

        _hackingCompletionSlider.value = percentageValue;

        if (_hackingCompletionSlider.value >= 1.0f)
        {
            Activate(false);
            FloatingTextSpawner.CreateFloatingTextStatic
                (transform.position, "Cracked!\nTraps reveresed.", Color.white, 2.0f, 4, 0.8f);
        }
    }
}
