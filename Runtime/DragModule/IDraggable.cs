using UnityEngine;

namespace SimpleManipulationKit
{
    public interface IDraggable : ISelectable { }

    public interface IDraggableAvailable : IDraggable
    {
        bool CanDrag();
    }

    public interface IDraggableStart : IDraggable
    {
        void OnDragStart(Vector3 position);
    }

    public interface IDraggableUpdate : IDraggable
    {
        void OnDragUpdate(Vector3 position);
    }

    public interface IDraggableEnd : IDraggable
    {
        void OnDragEnd(Vector3 position);
    }
}
