using System;
using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class MultiSelection : BaseSelectionCalculator
    {
        public override void Select(ISelectable selectable)
        {
            if (IsShiftPressed)
            {
                Selection.Add(selectable);
                return;
            }

            if (IsControlPressed)
            {
                Selection.Toggle(selectable);
                return;
            }

            if (selectable is IDraggable && Selection.Contains(selectable))
            {
                return;
            }

            Selection.Set(selectable);
        }
    }
}
