using System.Linq;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [RequireComponent(typeof(Collider))]
    public class DragController3D : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference] private IDragCalculator dragCalculator = new MultiWorldDrag();

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
            dragCalculator ??= new MultiWorldDrag();
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

            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        private void OnMouseDown()
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

        private void OnMouseUp()
        {
            EndDrag();
        }

        private void EndDrag()
        {
            if (!isDragging)
            {
                return;
            }

            if (Draggable is IDraggableEnd draggableEnd)
            {
                draggableEnd.OnDragEnd(Input.mousePosition);
            }

            dragCalculator.EndDrag();
            isDragging = false;
        }
    }
}
