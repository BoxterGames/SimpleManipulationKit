using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    public class SelectionController : MonoBehaviour, IPointerDownHandler
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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Selectable is null)
            {
                return;
            }

            selectionCalculator.Select(Selectable);
        }
    }
}
