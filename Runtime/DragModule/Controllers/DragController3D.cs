using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    [RequireComponent(typeof(Collider))]
    public class DragController3D : MonoBehaviour
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

        private void OnDisable()
        {
            dragCalculator.CancelDrag(Input.mousePosition);
        }

        private void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)
                || EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            dragCalculator.TryBeginDrag(Draggable, Input.mousePosition);
        }

        private void Update()
        {
            if (!dragCalculator.IsDragging)
            {
                return;
            }

            dragCalculator.UpdateDrag(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            dragCalculator.EndDrag(Input.mousePosition);
        }
    }
}
