using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    public class DragController : BaseDragController, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            TryBeginDrag();
        }

        private void Update()
        {
            UpdateDrag();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndDrag();
        }
    }
}
