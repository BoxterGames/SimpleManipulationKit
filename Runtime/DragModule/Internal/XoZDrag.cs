using System;
using System.Collections.Generic;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class XoZDrag : IDragCalculator
    {
        private static Vector3 Pointer => Input.mousePosition;
        private static Camera camera;

        private readonly List<IDraggable> targets = new();
        private readonly Dictionary<IDraggable, Vector3> grabOffsets = new();
        private readonly Dictionary<IDraggable, Transform> targetTransforms = new();

        public void BeginDrag(List<IDraggable> draggables)
        {
            targets.Clear();
            grabOffsets.Clear();
            targetTransforms.Clear();

            foreach (var draggable in draggables)
            {
                if (draggable is not MonoBehaviour mono)
                {
                    continue;
                }

                targets.Add(draggable);
                targetTransforms.Add(draggable, mono.transform);
            }

            if (targets.Count == 0)
            {
                return;
            }

            camera ??= Camera.main;

            var hit = Project(targetTransforms[targets[0]]);
            foreach (var draggable in targets)
            {
                var t = targetTransforms[draggable];
                grabOffsets[draggable] = t.localPosition - t.parent.InverseTransformPoint(hit);
                if (draggable is IDraggableStart start)
                {
                    start.OnDragStart(t.localPosition);
                }
            }
        }

        public void UpdateDrag()
        {
            if (targets.Count == 0)
            {
                return;
            }

            var hit = Project(targetTransforms[targets[0]]);
            foreach (var draggable in targets)
            {
                var t = targetTransforms[draggable];
                t.localPosition = t.parent.InverseTransformPoint(hit) + grabOffsets[draggable];
                if (draggable is IDraggableUpdate update)
                {
                    update.OnDragUpdate(t.localPosition);
                }
            }
        }

        public void EndDrag()
        {
            foreach (var draggable in targets)
            {
                if (draggable is IDraggableEnd end)
                {
                    end.OnDragEnd(targetTransforms[draggable].localPosition);
                }
            }

            targets.Clear();
            grabOffsets.Clear();
            targetTransforms.Clear();
        }

        private static Vector3 Project(Transform source)
        {
            var ray = camera.ScreenPointToRay(Pointer);
            var plane = new Plane(source.parent.up, source.position);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }
    }
}
