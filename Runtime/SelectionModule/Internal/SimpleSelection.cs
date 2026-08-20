using System;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SimpleSelection : ISelectionCalculator
    {
        public void Select(ISelectable selectable)
        {
            InteractionContext.Selection.Set(selectable);
        }
    }
}
