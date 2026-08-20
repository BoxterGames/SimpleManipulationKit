using System;
using System.Collections.Generic;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class ScreenDrag : IDragCalculator
    {
        private static Vector2 Pointer => Input.mousePosition;

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
                if (draggable is not MonoBehaviour mono || mono.transform.parent is not RectTransform)
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

            var hit = Project(targetTransforms[targets[0]]);
            foreach (var draggable in targets)
            {
                var t = targetTransforms[draggable];
                grabOffsets[draggable] = t.localPosition - hit;
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
                t.localPosition = hit + grabOffsets[draggable];
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
            var parent = (RectTransform)source.parent;
            var local = parent.GetLocalPosition(GetCanvasCamera(parent), Pointer);
            return new Vector3(local.x, local.y, source.localPosition.z);
        }

        private static Camera GetCanvasCamera(RectTransform rect)
        {
            var canvas = rect.GetComponentInParent<Canvas>();
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return null;
            }

            return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }
    }
}
