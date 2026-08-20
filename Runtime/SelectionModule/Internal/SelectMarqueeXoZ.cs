using System;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SelectMarqueeXoZ : BaseMarqueeCalculator
    {
        public override void OnMarqueeEnd(ISelectable selectable, Vector3 start, Vector3 end)
        {
            if (!SelectableBounds.IntersectsMarqueeXoZ(selectable, start, end, MarqueeCamera))
            {
                return;
            }

            ApplySelection(selectable);
        }
    }
}
