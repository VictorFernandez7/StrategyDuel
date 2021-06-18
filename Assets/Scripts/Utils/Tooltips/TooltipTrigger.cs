using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils.UI.Tooltips
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Title("Parameters")]
        [SerializeField] private float _hideTooltipTimer;

        [Title("References")]
        [SerializeField] private Tooltip _tooltip;

        public Tooltip tooltip => _tooltip;

        public bool canDisplayTooltip
        {
            get => _canDisplayTooltip;
            set { _canDisplayTooltip = value; }
        }

        private bool _timerPaused;
        private bool _mouseOverTrigger;
        private bool _mouseOverTooltip;
        private bool _canDisplayTooltip = true;
        private float _timer;

        private void Awake()
        {
            EventManagement();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_canDisplayTooltip)
                ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseOverTrigger = false;
        }

        private void Update()
        {
            if (!_mouseOverTrigger && !_timerPaused)
            {
                _timer += Time.deltaTime;

                if (_timer >= _hideTooltipTimer)
                {
                    HideTooltip();
                    ResetTimer();
                }

                if (_mouseOverTooltip)
                {
                    _timerPaused = true;
                    ResetTimer();
                }
            }
        }

        private void EventManagement()
        {
            if (_tooltip != null)
            {
                _tooltip.onMouseEnterTooltip.AddListener(MouseOverTooltip);
                _tooltip.onMouseExitTooltip.AddListener(MouseNotOverTooltip);
                _tooltip.onMouseExitTooltip.AddListener(HideTooltip);

                // If the tooltip contains nested tooltips
                if (_tooltip.GetComponentInChildren<TooltipTrigger>())
                {
                    _tooltip.onMouseExitTooltip.RemoveListener(MouseNotOverTooltip);
                    _tooltip.onMouseExitTooltip.RemoveListener(HideTooltip);

                    _tooltip.GetComponentInChildren<TooltipTrigger>().tooltip.onMouseEnterTooltip.AddListener(MouseOverTooltip);
                    _tooltip.GetComponentInChildren<TooltipTrigger>().tooltip.onMouseExitTooltip.AddListener(MouseNotOverTooltip);
                }
            }
        }

        private void ShowTooltip()
        {
            ResetTimer();

            if (!_mouseOverTrigger)
            {
                _mouseOverTrigger = true;
                _timerPaused = false;

                if (_tooltip != null)
                    _tooltip.gameObject.SetActive(true);
            }
        }

        private void HideTooltip()
        {
            _timerPaused = true;

            if (_tooltip != null)
            {
                _tooltip.gameObject.SetActive(false);

                if (_tooltip.GetComponentInParent<Tooltip>() != null && !_mouseOverTooltip)
                    _tooltip.GetComponentInParent<Tooltip>().gameObject.SetActive(false);
            }
        }

        private void ResetTimer()
        {
            _timer = 0f;
        }

        private void MouseOverTooltip()
        {
            _mouseOverTooltip = true;
        }

        private void MouseNotOverTooltip()
        {
            ResetTimer();
            _mouseOverTrigger = false;
            _mouseOverTooltip = false;
        }
    }
}