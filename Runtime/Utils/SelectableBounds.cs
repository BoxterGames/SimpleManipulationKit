using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    public static class SelectableBounds
    {
        public static bool IntersectsMarqueeXoZ(ISelectable selectable, Vector3 screenStart, Vector3 screenEnd, Camera camera)
        {
            return IntersectsPlane(selectable, screenStart, screenEnd, camera, new XoZMarquee());
        }

        public static bool IntersectsMarqueeXY(ISelectable selectable, Vector3 screenStart, Vector3 screenEnd, Camera camera)
        {
            if (selectable is MonoBehaviour behaviour && behaviour.transform is RectTransform)
            {
                return IntersectsScreen(selectable, screenStart, screenEnd);
            }

            return IntersectsPlane(selectable, screenStart, screenEnd, camera, new XYMarquee());
        }

        private static bool IntersectsPlane(
            ISelectable selectable,
            Vector3 screenStart,
            Vector3 screenEnd,
            Camera camera,
            IMarqueeView plane)
        {
            if (selectable is not MonoBehaviour behaviour)
            {
                return false;
            }

            var transform = behaviour.transform;

            if (transform is RectTransform rectTransform)
            {
                var corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);

                foreach (var corner in corners)
                {
                    if (plane.Contains(corner, screenStart, screenEnd, camera))
                    {
                        return true;
                    }
                }

                return false;
            }

            return plane.Contains(transform.position, screenStart, screenEnd, camera);
        }

        private static bool IntersectsScreen(ISelectable selectable, Vector3 start, Vector3 end)
        {
            if (selectable is not MonoBehaviour behaviour || behaviour.transform is not RectTransform rectTransform)
            {
                return false;
            }

            var marqueeMin = Vector2.Min(start, end);
            var marqueeMax = Vector2.Max(start, end);
            var marqueeRect = Rect.MinMaxRect(marqueeMin.x, marqueeMin.y, marqueeMax.x, marqueeMax.y);

            return GetScreenRect(rectTransform).Overlaps(marqueeRect);
        }

        private static Rect GetScreenRect(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var camera = GetCanvasCamera(rectTransform);

            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);

            foreach (var corner in corners)
            {
                var screen = RectTransformUtility.WorldToScreenPoint(camera, corner);
                min = Vector2.Min(min, screen);
                max = Vector2.Max(max, screen);
            }

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private static Camera GetCanvasCamera(RectTransform rectTransform)
        {
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
        }
    }
}
