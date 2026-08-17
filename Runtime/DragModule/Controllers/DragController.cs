using System.Linq;
using SimpleManipulationKit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    public class DragController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference] private IDragCalculator dragCalculator = new MultiScreenDrag();

        private bool isDragging;

        private IDraggable Draggable => view as IDraggable;

        private void OnValidate()
        {
            if (view is not null && Draggable is null)
            {
                view = null;
            }

            view ??= GetComponentsInChildren<MonoBehaviour>(true).FirstOrDefault(x => x is IDraggable);
        }

        private void Awake()
        {
            dragCalculator ??= new MultiScreenDrag();
        }

        private void Update()
        {
            if (!isDragging)
            {
                return;
            }

            dragCalculator.UpdateDrag();

            if (Draggable is IDraggableUpdate draggableUpdate)
            {
                draggableUpdate.OnDragUpdate(Input.mousePosition);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Draggable is null || Draggable is IDraggableAvailable available && !available.CanDrag())
            {
                return;
            }

            if (Draggable is IDraggableStart draggableStart)
            {
                draggableStart.OnDragStart();
            }

            isDragging = true;
            dragCalculator.BeginDrag(Draggable);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            if (Draggable is IDraggableEnd draggableEnd)
            {
                draggableEnd.OnDragEnd(eventData.position);
            }

            dragCalculator.EndDrag();
            isDragging = false;
        }
    }
}
