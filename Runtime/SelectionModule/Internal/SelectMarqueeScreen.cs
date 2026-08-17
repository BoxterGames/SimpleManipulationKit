using System;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SelectMarqueeScreen : BaseMarqueeCalculator
    {
        public override void OnMarqueeEnd(ISelectable selectable, Vector3 start, Vector3 end)
        {
            if (!SelectableBounds.IntersectsMarqueeScreen(selectable, start, end))
            {
                return;
            }

            ApplySelection(selectable);
        }
    }
}
