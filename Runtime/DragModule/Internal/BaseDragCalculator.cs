using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public abstract class BaseDragCalculator : IDragCalculator
    {
        protected const float DragThreshold = 5f;

        protected DragModel Model => InteractionContext.Drag;
        protected static Vector3 Pointer => Input.mousePosition;

        private static Camera camera;
        protected static Camera DragCamera => camera ??= Camera.main;

        public abstract void BeginDrag(IDraggable source);
        public abstract void UpdateDrag();
        public abstract void EndDrag();

        protected static bool IsBelowThreshold(Vector3 startPointer) =>
            !InteractionContext.Drag.IsDragging &&
            (Pointer - startPointer).magnitude < DragThreshold;

        protected static bool TryGetTransform(IDraggable draggable, out Transform transform)
        {
            if (draggable is MonoBehaviour behaviour)
            {
                transform = behaviour.transform;
                return true;
            }

            transform = null;
            return false;
        }

        protected static Vector3 ProjectPointer(Vector3 screenPoint, float y)
        {
            var ray = DragCamera.ScreenPointToRay(screenPoint);
            var plane = new Plane(Vector3.up, new Vector3(0f, y, 0f));

            return plane.Raycast(ray, out var enter)
                ? ray.GetPoint(enter)
                : Vector3.zero;
        }
    }
}
