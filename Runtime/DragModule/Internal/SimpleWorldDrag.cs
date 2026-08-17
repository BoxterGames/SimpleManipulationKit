using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SimpleWorldDrag : SimpleDragCalculator
    {
        private Vector3 grabOffset;
        private float targetY;

        public override void BeginDrag(IDraggable draggable)
        {
            if (!TryBeginDrag(draggable))
            {
                return;
            }

            targetY = target.position.y;
            grabOffset = target.position - ProjectPointer(startPointer, targetY);
        }

        protected override void ApplyMovement()
        {
            var position = ProjectPointer(Pointer, targetY) + grabOffset;
            position.y = targetY;
            target.position = position;
        }
    }
}
