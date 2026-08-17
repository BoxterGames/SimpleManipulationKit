using System;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SelectMarqueeWorld : BaseMarqueeCalculator
    {
        public override void OnMarqueeEnd(ISelectable selectable, Vector3 start, Vector3 end)
        {
            if (!SelectableBounds.IntersectsMarqueeWorld(selectable, start, end, MarqueeCamera))
            {
                return;
            }

            ApplySelection(selectable);
        }
    }
}
