using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class MultiSelection : ISelectionCalculator
    {
        private bool IsShiftPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        private bool IsControlPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        private SelectionModel Model => InteractionContext.Selection;
        
        public void Select(ISelectable selectable)
        {
            if (IsShiftPressed)
            {
                Model.Add(selectable);
                return;
            }

            if (IsControlPressed)
            {
                Model.Toggle(selectable);
                return;
            }

            if (selectable is IDraggable && Model.Contains(selectable))
            {
                return;
            }

            Model.Set(selectable);
        }
    }
}
