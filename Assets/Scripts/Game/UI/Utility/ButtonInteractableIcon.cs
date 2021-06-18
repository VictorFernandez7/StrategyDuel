using UnityEngine.UI;
using UnityEngine;

namespace Utils.UI
{
    public class ButtonInteractableIcon : MonoBehaviour
    {
        private Image _icon;
        private Color _baseColor;
        private Color _lockedColor;
        private Button _parentButton;

        private void Awake()
        {
            _icon = GetComponent<Image>();
            _parentButton = GetComponentInParent<Button>();

            _baseColor = _parentButton.colors.normalColor;
            _lockedColor = _parentButton.colors.disabledColor;

            SwitchIconStatus();
        }

        public void SwitchIconStatus()
        {
            if (_icon != null)
            {
                if (_parentButton.interactable)
                    _icon.color = _baseColor;

                else
                    _icon.color = _lockedColor;
            }
        }
    }
}