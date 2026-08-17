using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class SimpleScreenDrag : SimpleDragCalculator
    {
        private Vector3 startLocalPosition;

        public override void BeginDrag(IDraggable draggable)
        {
            if (!TryBeginDrag(draggable))
            {
                return;
            }

            startLocalPosition = target.localPosition;
        }

        protected override void ApplyMovement()
        {
            target.localPosition = startLocalPosition + (Vector3)(Pointer - startPointer);
        }
    }
}
