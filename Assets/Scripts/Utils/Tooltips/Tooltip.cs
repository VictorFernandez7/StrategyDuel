using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine;

namespace Utils.UI.Tooltips
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Title("Events")]
        [SerializeField] private UnityEvent _onMouseEnterTooltip;
        [SerializeField] private UnityEvent _onMouseExitTooltip;

        public UnityEvent onMouseEnterTooltip => _onMouseEnterTooltip;
        public UnityEvent onMouseExitTooltip => _onMouseExitTooltip;

        public void OnPointerEnter(PointerEventData eventData)
        {
            onMouseEnterTooltip.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onMouseExitTooltip.Invoke();
        }
    }
}