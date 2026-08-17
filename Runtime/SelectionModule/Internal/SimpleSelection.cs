using System;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SimpleSelection : BaseSelectionCalculator
    {
        public override void Select(ISelectable selectable)
        {
            Selection.Set(selectable);
        }
    }
}
