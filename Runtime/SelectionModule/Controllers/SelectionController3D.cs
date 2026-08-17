using UnityEngine;
using System.Linq;
using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    [RequireComponent(typeof(Collider))]
    public class SelectionController3D : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference] private ISelectionCalculator selectionCalculator = new MultiSelection();

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

        private void OnMouseDown()
        {
            if (Selectable is null)
            {
                return;
            }

            selectionCalculator.Select(Selectable);
        }
    }
}
