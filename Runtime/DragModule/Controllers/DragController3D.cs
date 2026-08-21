using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [RequireComponent(typeof(Collider))]
    public class DragController3D : BaseDragController
    {
        private void OnMouseDown()
        {
            TryBeginDrag();
        }

        private void Update()
        {
            UpdateDrag();
        }

        private void OnMouseUp()
        {
            EndDrag();
        }
    }
}
