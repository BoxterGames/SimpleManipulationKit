using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class XYMarquee : IMarqueeView
    {
        public void Apply(Transform target, Vector3 screenStart, Vector3 screenEnd, Camera camera)
        {
            if (target.parent is RectTransform root)
            {
                ApplyInRect(target, root, screenStart, screenEnd, camera);
                return;
            }

            MarqueePlane.GetScreenRectBoundsOnXYPlane(
                screenStart,
                screenEnd,
                camera,
                out var minX,
                out var maxX,
                out var minY,
                out var maxY);

            target.position = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            target.localScale = new Vector3(maxX - minX, maxY - minY, 1f);
        }

        public bool Contains(Vector3 world, Vector3 screenStart, Vector3 screenEnd, Camera camera)
        {
            MarqueePlane.GetScreenRectBoundsOnXYPlane(
                screenStart,
                screenEnd,
                camera,
                out var minX,
                out var maxX,
                out var minY,
                out var maxY);

            return MarqueePlane.ContainsXY(world, minX, maxX, minY, maxY);
        }

        private static void ApplyInRect(
            Transform target,
            RectTransform root,
            Vector3 screenStart,
            Vector3 screenEnd,
            Camera camera)
        {
            var canvas = root.GetComponentInParent<Canvas>();
            var cam = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : camera != null ? camera : canvas?.worldCamera ?? Camera.main;

            var minScreen = Vector2.Min(screenStart, screenEnd);
            var maxScreen = Vector2.Max(screenStart, screenEnd);

            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            var initialized = false;

            TryProject(new Vector2(minScreen.x, maxScreen.y));
            TryProject(new Vector2(maxScreen.x, maxScreen.y));
            TryProject(new Vector2(maxScreen.x, minScreen.y));
            TryProject(new Vector2(minScreen.x, minScreen.y));

            if (!initialized)
            {
                target.localScale = Vector3.zero;
                return;
            }

            var center = (min + max) * 0.5f;
            var size = max - min;
            target.localPosition = new Vector3(center.x, center.y, target.localPosition.z);
            target.localScale = new Vector3(size.x, size.y, 1f);

            void TryProject(Vector2 screen)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screen, cam, out var local))
                {
                    return;
                }

                if (!initialized)
                {
                    min = max = local;
                    initialized = true;
                    return;
                }

                min = Vector2.Min(min, local);
                max = Vector2.Max(max, local);
            }
        }
    }
}
