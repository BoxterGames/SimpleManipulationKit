using System;
using System.Collections.Generic;
using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class DragModel
    {
        public bool IsDragging => targets.Count  > 0;

        private List<IDraggable> targets = new();

        public event Action<Vector3> OnDragStart;
        public event Action<Vector3, Vector3> OnDragUpdate;
        public event Action<Vector3, Vector3> OnDragEnd;
        
        public Vector3 StartPosition { get; private set; }
        public Vector3 EndPosition { get; private set; }

        public void BeginDrag(IDraggable target, Vector3 pointerPosition)
        {
            targets.Clear();
            targets.Add(target);
            StartPosition = pointerPosition;
            EndPosition = pointerPosition;
            OnDragStart?.Invoke(pointerPosition);
        }

        public void BeginDrag(IEnumerable<IDraggable> listTarget, Vector3 pointerPosition)
        {
            targets.Clear();
            targets.AddRange(listTarget);
            StartPosition = pointerPosition;
            EndPosition = pointerPosition;
            OnDragStart?.Invoke(pointerPosition);
        }

        public void UpdateDrag(Vector3 pointerPosition)
        {
            EndPosition = pointerPosition;
            OnDragUpdate?.Invoke(StartPosition, pointerPosition);
        }

        public void EndDrag(Vector3 pointerPosition)
        {
            EndPosition = pointerPosition;
            OnDragEnd?.Invoke(StartPosition, pointerPosition);
            targets.Clear();
        }
    }
}
