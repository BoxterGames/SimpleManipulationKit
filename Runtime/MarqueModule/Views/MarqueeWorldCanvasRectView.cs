using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class MarqueeWorldCanvasRectView : MonoBehaviour
    {
        [SerializeField] private RectTransform canvasRoot;
        [SerializeField] private Camera interactionCamera;

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private void OnValidate()
        {
            canvasRoot ??= transform.parent as RectTransform;
            canvasRoot ??= GetComponentInParent<Canvas>()?.transform as RectTransform;
        }

        private void LateUpdate()
        {
            if (!ShouldShow())
            {
                transform.localScale = Vector3.zero;
                return;
            }

            var root = canvasRoot;
            if (root == null)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            var camera = interactionCamera != null
                ? interactionCamera
                : root.GetComponentInParent<Canvas>()?.worldCamera ?? Camera.main;

            GetScreenRectLocalBounds(
                Marquee.StartPosition,
                Marquee.EndPosition,
                root,
                camera,
                out var min,
                out var max);

            var center = (min + max) * 0.5f;
            var size = max - min;

            transform.localPosition = new Vector3(center.x, center.y, 0f);
            transform.localScale = new Vector3(size.x, size.y, 1f);
        }

        private static void GetScreenRectLocalBounds(
            Vector3 screenStart,
            Vector3 screenEnd,
            RectTransform root,
            Camera camera,
            out Vector2 min,
            out Vector2 max)
        {
            var minScreen = Vector2.Min(screenStart, screenEnd);
            var maxScreen = Vector2.Max(screenStart, screenEnd);

            var boundsMin = new Vector2(float.MaxValue, float.MaxValue);
            var boundsMax = new Vector2(float.MinValue, float.MinValue);
            var initialized = false;

            TryProject(new Vector2(minScreen.x, maxScreen.y));
            TryProject(new Vector2(maxScreen.x, maxScreen.y));
            TryProject(new Vector2(maxScreen.x, minScreen.y));
            TryProject(new Vector2(minScreen.x, minScreen.y));

            if (!initialized)
            {
                boundsMin = boundsMax = Vector2.zero;
            }

            min = boundsMin;
            max = boundsMax;

            void TryProject(Vector2 screen)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screen, camera, out var local))
                {
                    return;
                }

                if (!initialized)
                {
                    boundsMin = local;
                    boundsMax = local;
                    initialized = true;
                    return;
                }

                boundsMin = Vector2.Min(boundsMin, local);
                boundsMax = Vector2.Max(boundsMax, local);
            }
        }

        private static bool ShouldShow()
        {
            return InteractionContext.Marquee.IsActive
                && Input.GetMouseButton(0)
                && !InteractionContext.Drag.IsDragging;
        }
    }
}
