using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    public interface IDragCalculator
    {
        void BeginDrag(IDraggable source);
        void UpdateDrag();
        void EndDrag();
    }
}
