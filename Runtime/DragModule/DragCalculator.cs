using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    internal sealed class DragCalculator
    {
        private readonly ISelectionCalculator selectionCalculator;
        private readonly ISpaceConverter spaceConverter;

        private readonly List<IDraggable> targets = new();
        private readonly Dictionary<IDraggable, Vector3> grabOffsets = new();

        public bool IsDragging => targets.Count > 0;

        public DragCalculator(
            ISelectionCalculator selectionCalculator,
            ISpaceConverter spaceConverter)
        {
            this.selectionCalculator = selectionCalculator;
            this.spaceConverter = spaceConverter;
        }

        public void UpdateDrag(Vector3 screenPoint)
        {
            if (!IsDragging)
            {
                return;
            }

            if (!Input.GetMouseButton(0))
            {
                EndDrag(screenPoint);
                return;
            }

            UpdateDragTargets(screenPoint);
            InteractionContext.Drag.UpdateDrag(screenPoint);
        }

        public bool TryBeginDrag(IDraggable draggable, Vector3 screenPoint)
        {
            if (draggable is null || !IsDraggable(draggable))
            {
                return false;
            }

            selectionCalculator.Select(draggable);

            var selected = InteractionContext.Selection
                .GetSelected<IDraggable>()
                .Where(IsDraggable)
                .ToList();

            if (selected.Count == 0)
            {
                return false;
            }

            BeginDragTargets(selected, screenPoint);

            if (!IsDragging)
            {
                return false;
            }

            InteractionContext.Drag.BeginDrag(selected, screenPoint);

            return true;
        }

        public void EndDrag(Vector3 screenPoint)
        {
            if (!IsDragging)
            {
                return;
            }

            EndDragTargets();
            InteractionContext.Drag.EndDrag(screenPoint);
        }

        public void CancelDrag(Vector3 screenPoint)
        {
            if (IsDragging)
            {
                EndDrag(screenPoint);
                return;
            }

            if (InteractionContext.Drag.IsDragging)
            {
                InteractionContext.Drag.Cancel();
            }
        }

        private void BeginDragTargets(IReadOnlyList<IDraggable> draggables, Vector3 screenPoint)
        {
            targets.Clear();
            grabOffsets.Clear();

            foreach (var draggable in draggables)
            {
                if (draggable is not MonoBehaviour || !IsDraggable(draggable))
                {
                    continue;
                }

                targets.Add(draggable);
            }

            if (targets.Count == 0)
            {
                return;
            }

            var firstTransform = ((MonoBehaviour)targets[0]).transform;
            var hit = spaceConverter.ScreenToWorldPoint(firstTransform, screenPoint);

            foreach (var draggable in targets)
            {
                var transform = ((MonoBehaviour)draggable).transform;
                var space = transform.parent ?? transform;

                grabOffsets[draggable] =
                    transform.localPosition -
                    space.InverseTransformPoint(hit);

                if (draggable is IDraggableStart start)
                {
                    start.OnDragStart(transform.localPosition);
                }
            }
        }

        private void UpdateDragTargets(Vector3 screenPoint)
        {
            if (targets.Count == 0)
            {
                return;
            }

            var firstTransform = ((MonoBehaviour)targets[0]).transform;
            var hit = spaceConverter.ScreenToWorldPoint(firstTransform, screenPoint);

            foreach (var draggable in targets)
            {
                if (!IsDraggable(draggable))
                {
                    continue;
                }

                var transform = ((MonoBehaviour)draggable).transform;
                var space = transform.parent ?? transform;

                transform.localPosition =
                    space.InverseTransformPoint(hit) +
                    grabOffsets[draggable];

                if (draggable is IDraggableUpdate update)
                {
                    update.OnDragUpdate(transform.localPosition);
                }
            }
        }

        private void EndDragTargets()
        {
            foreach (var draggable in targets)
            {
                if (draggable is IDraggableEnd end)
                {
                    end.OnDragEnd(((MonoBehaviour)draggable).transform.localPosition);
                }
            }

            targets.Clear();
            grabOffsets.Clear();
        }

        private static bool IsDraggable(IDraggable draggable)
        {
            return draggable is not IDraggableAvailable available || available.CanDrag();
        }
    }
}
