using System.Linq;
using SimpleManipulationKit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    public class DragController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference] private ISelectionCalculator selectionCalculator = new MultiSelection();
        [SerializeReference] private IDragCalculator dragCalculator = new ScreenDrag();

        private IDraggable Draggable => view as IDraggable;
        private bool IsDragging => InteractionContext.Drag.IsDragging;

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
            dragCalculator ??= new ScreenDrag();
        }

        private void Update()
        {
            if (!IsDragging)
            {
                return;
            }

            dragCalculator.UpdateDrag();
            InteractionContext.Drag.UpdateDrag(Input.mousePosition);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Draggable is null || Draggable is IDraggableAvailable available && !available.CanDrag())
            {
                return;
            }

            selectionCalculator.Select(Draggable);
            var selected = InteractionContext.Selection.GetSelected<IDraggable>().ToList();
            if (selected.Count == 0)
            {
                return;
            }

            InteractionContext.Drag.BeginDrag(selected, Input.mousePosition);
            dragCalculator.BeginDrag(selected);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            dragCalculator.EndDrag();
            InteractionContext.Drag.EndDrag(Input.mousePosition);
        }
    }
}
