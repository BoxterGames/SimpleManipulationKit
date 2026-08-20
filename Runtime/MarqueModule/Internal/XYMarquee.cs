using System;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class XYMarquee : IMarqueeView
    {
        private RectTransform root;

        public Vector3 Project(Vector3 screen, Camera camera, Transform target)
        {
            if (target != null && target.parent is RectTransform rectRoot)
            {
                root = rectRoot;
                return ToLocal(rectRoot, screen, camera);
            }

            root = null;
            return MarqueePlane.ScreenToXYPlane(screen, camera);
        }

        public void Apply(Transform target, Vector3 start, Vector3 end)
        {
            if (target.parent is RectTransform rectRoot)
            {
                root = rectRoot;
                ApplyInRect(target, start, end);
                return;
            }

            root = null;
            MarqueePlane.GetBoundsXY(start, end, out var minX, out var maxX, out var minY, out var maxY);
            target.position = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            target.localScale = new Vector3(maxX - minX, maxY - minY, 1f);
        }

        public bool Contains(Vector3 world, Vector3 start, Vector3 end)
        {
            var point = root != null ? root.InverseTransformPoint(world) : world;
            MarqueePlane.GetBoundsXY(start, end, out var minX, out var maxX, out var minY, out var maxY);
            return MarqueePlane.ContainsXY(point, minX, maxX, minY, maxY);
        }

        private static void ApplyInRect(Transform target, Vector3 start, Vector3 end)
        {
            MarqueePlane.GetBoundsXY(start, end, out var minX, out var maxX, out var minY, out var maxY);
            var center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            target.localPosition = new Vector3(center.x, center.y, target.localPosition.z);
            target.localScale = new Vector3(maxX - minX, maxY - minY, 1f);
        }

        private static Vector3 ToLocal(RectTransform rectRoot, Vector3 screen, Camera camera)
        {
            var canvas = rectRoot.GetComponentInParent<Canvas>();
            var cam = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : camera != null ? camera : canvas?.worldCamera ?? Camera.main;

            var local = rectRoot.GetLocalPosition(cam, screen);
            return new Vector3(local.x, local.y, 0f);
        }
    }
}
