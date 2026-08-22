using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public sealed class DragCalculator
    {
        private readonly ISelectionCalculator selectionCalculator;
        private readonly ISpaceConverter spaceConverter;

        private readonly List<IDraggable> targets = new();
        private readonly Dictionary<IDraggable, Vector3> grabOffsets = new();

        private bool IsDragging => InteractionContext.Drag.IsDragging;

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
                return;

            UpdateDragTargets(screenPoint);
            InteractionContext.Drag.UpdateDrag(screenPoint);
        }

        public bool TryBeginDrag(IDraggable draggable, Vector3 screenPoint)
        {
            if (draggable is null ||
                draggable is IDraggableAvailable available && !available.CanDrag())
                return false;

            selectionCalculator.Select(draggable);

            var selected = InteractionContext.Selection
                .GetSelected<IDraggable>()
                .ToList();

            if (selected.Count == 0)
                return false;

            InteractionContext.Drag.BeginDrag(selected, screenPoint);
            BeginDragTargets(selected, screenPoint);

            return true;
        }

        public void EndDrag(Vector3 screenPoint)
        {
            if (!IsDragging)
                return;

            EndDragTargets();
            InteractionContext.Drag.EndDrag(screenPoint);
        }

        private void BeginDragTargets(
            IReadOnlyList<IDraggable> draggables,
            Vector3 screenPoint)
        {
            targets.Clear();
            grabOffsets.Clear();

            foreach (var draggable in draggables)
            {
                if (draggable is not MonoBehaviour)
                    continue;

                targets.Add(draggable);
            }

            if (targets.Count == 0)
                return;

            var firstTransform = ((MonoBehaviour)targets[0]).transform;
            var hit = spaceConverter.ScreenToWorldPoint(firstTransform, screenPoint);

            foreach (var draggable in targets)
            {
                var transform = ((MonoBehaviour)draggable).transform;

                grabOffsets[draggable] =
                    transform.localPosition -
                    transform.parent.InverseTransformPoint(hit);

                if (draggable is IDraggableStart start)
                    start.OnDragStart(transform.localPosition);
            }
        }

        private void UpdateDragTargets(Vector3 screenPoint)
        {
            if (targets.Count == 0)
                return;

            var firstTransform = ((MonoBehaviour)targets[0]).transform;
            var hit = spaceConverter.ScreenToWorldPoint(firstTransform, screenPoint);

            foreach (var draggable in targets)
            {
                var transform = ((MonoBehaviour)draggable).transform;

                transform.localPosition =
                    transform.parent.InverseTransformPoint(hit) +
                    grabOffsets[draggable];

                if (draggable is IDraggableUpdate update)
                    update.OnDragUpdate(transform.localPosition);
            }
        }

        private void EndDragTargets()
        {
            foreach (var draggable in targets)
            {
                if (draggable is IDraggableEnd end)
                    end.OnDragEnd(((MonoBehaviour)draggable).transform.localPosition);
            }

            targets.Clear();
            grabOffsets.Clear();
        }
    }
}
