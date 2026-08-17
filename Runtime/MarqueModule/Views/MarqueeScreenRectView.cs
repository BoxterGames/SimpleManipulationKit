using UnityEngine;

namespace SimpleManipulationKit
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MarqueeScreenRectView : MonoBehaviour
    {
        private MarqueeModel Marquee
        {
            get
            {
                return InteractionContext.Marquee;
            }
        }

        private void LateUpdate()
        {
            var rt = (RectTransform)transform;

            if (!ShouldShow())
            {
                rt.sizeDelta = Vector2.zero;
                return;
            }

            var root = rt.parent as RectTransform;
            if (root == null)
            {
                rt.sizeDelta = Vector2.zero;
                return;
            }

            var camera = GetCanvasCamera(root);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, Marquee.StartPosition, camera, out var a);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, Marquee.EndPosition, camera, out var b);

            var min = Vector2.Min(a, b);
            var max = Vector2.Max(a, b);
            var localZ = rt.localPosition.z;
            rt.anchoredPosition = (min + max) * 0.5f;
            rt.sizeDelta = max - min;
            var local = rt.localPosition;
            rt.localPosition = new Vector3(local.x, local.y, localZ);
        }

        private static bool ShouldShow()
        {
            return InteractionContext.Marquee.IsActive
                && Input.GetMouseButton(0)
                && !InteractionContext.Drag.IsDragging;
        }

        private static Camera GetCanvasCamera(RectTransform root)
        {
            var canvas = root.GetComponentInParent<Canvas>();
            return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
        }
    }
}
