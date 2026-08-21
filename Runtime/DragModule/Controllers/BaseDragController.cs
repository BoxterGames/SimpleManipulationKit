using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public abstract class BaseDragController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference, Attributes] private ISelectionCalculator selectionCalculator = new MultiSelection();
        [SerializeReference, Attributes] private ISpaceConverter spaceConverter = new ScreenSpaceConverter();

        private readonly List<IDraggable> targets = new();
        private readonly Dictionary<IDraggable, Vector3> grabOffsets = new();

        private IDraggable Draggable => view as IDraggable;
        private bool IsDragging => InteractionContext.Drag.IsDragging;

        protected virtual void OnValidate()
        {
            if (view is not null && Draggable is null)
                view = null;

            view ??= GetComponentsInChildren<MonoBehaviour>(true)
                .FirstOrDefault(x => x is IDraggable);
        }

        protected void UpdateDrag()
        {
            if (!IsDragging)
                return;

            UpdateDragTargets(Input.mousePosition);
            InteractionContext.Drag.UpdateDrag(Input.mousePosition);
        }

        protected bool TryBeginDrag()
        {
            if (Draggable is null ||
                Draggable is IDraggableAvailable available && !available.CanDrag())
                return false;

            selectionCalculator.Select(Draggable);

            var selected = InteractionContext.Selection
                .GetSelected<IDraggable>()
                .ToList();

            if (selected.Count == 0)
                return false;

            InteractionContext.Drag.BeginDrag(selected, Input.mousePosition);
            BeginDragTargets(selected, Input.mousePosition);

            return true;
        }

        protected void EndDrag()
        {
            if (!IsDragging)
                return;

            EndDragTargets();
            InteractionContext.Drag.EndDrag(Input.mousePosition);
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