using UnityEngine;
using UnityEngine.UI;

public class PlayerOrBossLevelUI : MonoBehaviour
{
    private Transform _container;
    private Button _cancelButton;
    private Button _playerButton;
    private Button _bossButton;

    private void Awake()
    {
        _container = transform.Find("Container");
        _cancelButton = _container.Find("CancelButton").GetComponent<Button>();
        //_playerButton = _container.Find("PlayerButton").GetComponent<Button>();
        //_bossButton = _container.Find("BossButton").GetComponent<Button>();

        Debug.Log(_cancelButton == null);
        //Debug.Log(_playerButton == null);
        //Debug.Log(_bossButton == null);
    }

    public void Activate(bool value)
    {
        _container.gameObject.SetActive(value);
    }
}
