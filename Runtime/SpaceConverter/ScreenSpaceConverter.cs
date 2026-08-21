using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class ScreenSpaceConverter : ISpaceConverter
    {
        public Vector3 ScreenToWorldPoint(Transform reference, Vector2 screenPoint)
        {
            var localPoint = ScreenToLocalPoint(reference, screenPoint);
            return reference.parent.TransformPoint(localPoint);
        }

        public Vector3 ScreenToLocalPoint(Transform reference, Vector3 screenPoint)
        {
            var parent = (RectTransform)reference.parent;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                screenPoint,
                null,
                out var localPoint);

            return localPoint;
        }

        public Vector3 GetSize(
            Transform reference,
            Vector3 screenA,
            Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);

            var delta = a - b;

            return new Vector3(
                Mathf.Abs(delta.x),
                Mathf.Abs(delta.y),
                reference.localScale.z);
        }

        public Vector3 GetCenterPosition(
            Transform reference,
            Vector3 screenA,
            Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);

            var position = (a + b) * 0.5f;
            position.z = reference.localPosition.z;

            return position;
        }

        public bool IsIntersect(
            Transform reference,
            Vector3 screenA,
            Vector3 screenB)
        {
            var a = ScreenToLocalPoint(reference, screenA);
            var b = ScreenToLocalPoint(reference, screenB);

            var position = reference.localPosition;

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