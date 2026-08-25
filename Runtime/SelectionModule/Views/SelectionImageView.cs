using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace SimpleManipulationKit
{
    public class SelectionImageView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color unselectedColor = Color.gray;
        [SerializeField] private MonoBehaviour view;

        private ISelectable Selectable => view as ISelectable;
        private SelectionModel Selection => InteractionContext.Selection;

        private void OnValidate()
        {
            if (view is not null && Selectable is null)
            {
                view = null;
            }

            view ??= GetComponentsInChildren<MonoBehaviour>(true).FirstOrDefault(x => x is ISelectable);
            view ??= GetComponentsInParent<MonoBehaviour>(true).FirstOrDefault(x => x is ISelectable);
        }

        private void OnEnable()
        {
            Selection.OnUpdateSelected += RefreshVisual;
            RefreshVisual();
        }

        private void OnDisable()
        {
            Selection.OnUpdateSelected -= RefreshVisual;
        }

        private void RefreshVisual()
        {
            if (background is null)
            {
                return;
            }

            var isSelected = Selection.Contains(Selectable);
            background.color = isSelected ? selectedColor : unselectedColor;
        }
    }
}
