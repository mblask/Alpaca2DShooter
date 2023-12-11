using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Image _image;
    private Button _button;

    private SkillSO _buttonSkill;
    private bool _isPressed = false;
    private Color _pressedColor = new Color(0.7f, 0.7f, 0.7f);
    private Color _notPressedColor = new Color(1.0f, 1.0f, 1.0f);

    private PlayerSelector _playerSelector;

    private void Awake()
    {
        _text = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _playerSelector = PlayerSelector.Instance;
        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        _isPressed = !_isPressed;
        if (_isPressed)
        {
            _image.color = _pressedColor;
            _playerSelector.AddSkill(_buttonSkill);
        }
        else
        {
            _image.color = _notPressedColor;
            _playerSelector.RemoveSkill(_buttonSkill);
        }
    }

    public void SetupButton(SkillSO skill)
    {
        _text.SetText(skill.Name);
        _buttonSkill = skill;
    }
}
