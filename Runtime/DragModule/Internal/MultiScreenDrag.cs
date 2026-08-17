using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class MultiScreenDrag : MultiDragCalculator
    {
        private readonly Dictionary<Transform, Vector3> startLocalPositions = new();

        public override void BeginDrag(IDraggable draggable)
        {
            startLocalPositions.Clear();
            TryBeginDrag(draggable);
        }

        protected override void ApplyMovement()
        {
            if (startLocalPositions.Count == 0)
            {
                foreach (var transform in current)
                {
                    startLocalPositions[transform] = transform.localPosition;
                }
            }

            var delta = (Vector3)(Pointer - startPointer);

            foreach (var transform in current)
            {
                transform.localPosition = startLocalPositions[transform] + delta;
            }
        }
    }
}
