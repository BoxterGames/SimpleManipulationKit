using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public abstract class SimpleDragCalculator : BaseDragCalculator
    {
        protected Transform target;
        protected IDraggable draggable;
        protected Vector3 startPointer;

        public override void BeginDrag(IDraggable draggable)
        {
            TryBeginDrag(draggable);
        }

        protected bool TryBeginDrag(IDraggable draggable)
        {
            if (!TryGetTransform(draggable, out target))
            {
                this.draggable = null;
                return false;
            }

            this.draggable = draggable;
            startPointer = Pointer;
            return true;
        }

        public override void UpdateDrag()
        {
            if (target == null || IsBelowThreshold(startPointer))
            {
                return;
            }

            if (!Model.IsDragging)
            {
                InteractionContext.Selection.Set(draggable);
                Model.BeginDrag(draggable, startPointer);
            }

            Model.UpdateDrag(Pointer);
            ApplyMovement();
        }

        protected abstract void ApplyMovement();

        public override void EndDrag()
        {
            if (target == null || !Model.IsDragging)
            {
                return;
            }

            ApplyMovement();
            Model.EndDrag(Pointer);
        }
    }
}
