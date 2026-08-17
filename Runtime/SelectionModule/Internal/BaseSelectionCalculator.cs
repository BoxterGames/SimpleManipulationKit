using System;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public abstract class BaseSelectionCalculator : ISelectionCalculator
    {
        protected SelectionModel Selection => InteractionContext.Selection;

        public abstract void Select(ISelectable selectable);

        protected static bool IsShiftPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        protected static bool IsControlPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }
}
