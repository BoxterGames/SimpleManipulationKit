using System.Collections.Generic;
using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    public interface IDragCalculator
    {
        void BeginDrag(List<IDraggable> draggables);
        void UpdateDrag();
        void EndDrag();
    }
}
