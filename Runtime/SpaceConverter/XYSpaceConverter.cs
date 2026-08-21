using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class XYSpaceConverter : ISpaceConverter
    {
        private Camera targetCamera;

        public Vector3 ScreenToWorldPoint(Transform reference, Vector2 screenPoint)
        {
            targetCamera ??= Camera.main;

            var ray = targetCamera.ScreenPointToRay(screenPoint);
            var plane = new Plane(reference.parent.forward, reference.parent.position);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        public Vector3 ScreenToLocalPoint(Transform reference, Vector3 screenPoint)
        {
            var worldPoint = ScreenToWorldPoint(reference, screenPoint);
            return reference.parent.InverseTransformPoint(worldPoint);
        }

        public Vector3 GetSize(Transform reference, Vector3 screenA, Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);
            var delta = a - b;
            return new Vector3(Mathf.Abs(delta.x), Mathf.Abs(delta.y), reference.localScale.z);
        }

        public Vector3 GetCenterPosition(Transform reference, Vector3 screenA, Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);
            var localPosition = (a + b) * 0.5f;
            localPosition.z = reference.localPosition.z;
            return reference.parent.TransformPoint(localPosition);
        }

        public bool IsIntersect(Transform reference, Vector3 screenA, Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);

            var position = reference.parent.InverseTransformPoint(reference.position);
            
            var minX = Mathf.Min(a.x, b.x);
            var maxX = Mathf.Max(a.x, b.x);
            var minY = Mathf.Min(a.y, b.y);
            var maxY = Mathf.Max(a.y, b.y);

            return position.x >= minX &&
                   position.x <= maxX &&
                   position.y >= minY &&
                   position.y <= maxY;
        }
    }
}
