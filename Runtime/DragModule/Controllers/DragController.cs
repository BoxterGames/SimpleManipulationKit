using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    public class DragController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference, Attributes] private ISelectionCalculator selectionCalculator = new MultiSelection();
        [SerializeReference, Attributes] private ISpaceConverter spaceConverter = new ScreenSpaceConverter();

        private DragCalculator dragCalculator;

        private IDraggable Draggable => view as IDraggable;

        private void Awake()
        {
            dragCalculator = new DragCalculator(selectionCalculator, spaceConverter);
        }

        private void OnValidate()
        {
            if (view is not null && Draggable is null)
            {
                view = null;
            }

            view ??= GetComponentsInChildren<MonoBehaviour>(true).FirstOrDefault(x => x is IDraggable);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragCalculator.TryBeginDrag(Draggable, Input.mousePosition);
        }

        private void Update()
        {
            dragCalculator.UpdateDrag(Input.mousePosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragCalculator.EndDrag(Input.mousePosition);
        }
    }
}
