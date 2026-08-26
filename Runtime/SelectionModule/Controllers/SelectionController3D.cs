using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    public class SelectionController3D : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference, Attributes] private ISelectionCalculator selectionCalculator = new MultiSelection();

        private ISelectable Selectable => view as ISelectable;

        private void OnValidate()
        {
            if (view is not null && Selectable is null)
            {
                view = null;
            }

            view ??= GetComponentsInChildren<MonoBehaviour>(true).FirstOrDefault(x => x is ISelectable);
        }

        private void Awake()
        {
            selectionCalculator ??= new MultiSelection();
        }

        private void OnDisable()
        {
            InteractionContext.Selection.Remove(Selectable);
        }

        private void OnMouseDown()
        {
            if (Selectable is null
                || !Input.GetMouseButtonDown(0)
                || EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            selectionCalculator.Select(Selectable);
        }
    }
}
