using System.Collections.Generic;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public abstract class MultiDragCalculator : BaseDragCalculator
    {
        protected readonly List<Transform> current = new();

        protected Transform source;
        protected IDraggable draggable;
        protected Vector3 startPointer;

        public override void BeginDrag(IDraggable draggable)
        {
            TryBeginDrag(draggable);
        }

        protected bool TryBeginDrag(IDraggable draggable)
        {
            current.Clear();

            if (!TryGetTransform(draggable, out source))
            {
                this.draggable = null;
                source = null;
                return false;
            }

            this.draggable = draggable;
            startPointer = Pointer;
            return true;
        }

        public override void UpdateDrag()
        {
            if (source == null || IsBelowThreshold(startPointer))
            {
                return;
            }

            if (!Model.IsDragging)
            {
                TryStartDrag();
                return;
            }

            Model.UpdateDrag(Pointer);
            ApplyMovement();
        }

        protected abstract void ApplyMovement();

        public override void EndDrag()
        {
            if (source == null || !Model.IsDragging)
            {
                return;
            }

            ApplyMovement();
            Model.EndDrag(Pointer);
        }

        private void TryStartDrag()
        {
            current.Clear();

            var selection = InteractionContext.Selection;

            if (!selection.Contains(draggable))
            {
                selection.Set(draggable);
            }

            foreach (var item in selection.GetSelected<IDraggable>())
            {
                if (TryGetTransform(item, out var transform))
                {
                    current.Add(transform);
                }
            }

            if (current.Count == 0)
            {
                return;
            }

            Model.BeginDrag(selection.GetSelected<IDraggable>(), startPointer);
            Model.UpdateDrag(Pointer);
            ApplyMovement();
        }
    }
}
