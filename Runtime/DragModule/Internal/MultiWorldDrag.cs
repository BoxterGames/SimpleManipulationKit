using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class MultiWorldDrag : MultiDragCalculator
    {
        private Vector3 grabOffset;
        private float sourceY;

        public override void BeginDrag(IDraggable draggable)
        {
            if (!TryBeginDrag(draggable))
            {
                return;
            }

            sourceY = source.position.y;
            grabOffset = source.position - ProjectPointer(startPointer, sourceY);
        }

        protected override void ApplyMovement()
        {
            var newSourcePosition = ProjectPointer(Pointer, sourceY) + grabOffset;
            newSourcePosition.y = sourceY;

            var delta = newSourcePosition - source.position;
            delta.y = 0f;

            foreach (var transform in current)
            {
                transform.position += delta;
            }
        }
    }
}
