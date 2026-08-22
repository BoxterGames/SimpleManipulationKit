using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class XoZSpaceConverter : ISpaceConverter
    {
        private Camera targetCamera;

        public Vector3 ScreenToWorldPoint(Transform reference, Vector2 screenPoint)
        {
            targetCamera ??= Camera.main;

            var ray = targetCamera.ScreenPointToRay(screenPoint);
            var plane = new Plane(reference.parent.up, reference.parent.position);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        public Vector3 ScreenToLocalPoint(Transform reference, Vector3 screenPoint)
        {
            var worldPoint = ScreenToWorldPoint(reference, screenPoint);
            return reference.parent.InverseTransformPoint(worldPoint);
        }

        public Vector3 GetSize(Transform reference, Vector3 localA, Vector3 localB)
        {
            var delta = localA - localB;
            return new Vector3(Mathf.Abs(delta.x), reference.localScale.y, Mathf.Abs(delta.z));
        }

        public Vector3 GetCenterPosition(Transform reference, Vector3 localA, Vector3 localB)
        {
            var localPosition = (localA + localB) * 0.5f;
            localPosition.y = reference.localPosition.y;
            return reference.parent.TransformPoint(localPosition);
        }

        public bool IsIntersect(Transform reference, Vector3 screenA, Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);

            var position = reference.parent.InverseTransformPoint(reference.position);
            
            var minX = Mathf.Min(a.x, b.x);
            var maxX = Mathf.Max(a.x, b.x);
            var minZ = Mathf.Min(a.z, b.z);
            var maxZ = Mathf.Max(a.z, b.z);

            return position.x >= minX &&
                   position.x <= maxX &&
                   position.z >= minZ &&
                   position.z <= maxZ;
        }
    }
}
