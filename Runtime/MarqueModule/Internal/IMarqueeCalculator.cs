using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public interface IMarqueeCalculator
    {
        void OnMarqueeEnd(ISelectable selectable, Vector3 start, Vector3 end);
    }
}
