using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class MarqueeWorldRectView : MonoBehaviour
    {
        [SerializeField] private Camera interactionCamera;

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private Camera Camera =>
            interactionCamera != null ? interactionCamera : UnityEngine.Camera.main;

        private void LateUpdate()
        {
            if (!ShouldShow())
            {
                transform.localScale = Vector3.zero;
                return;
            }

            MarqueePlane.GetScreenRectBoundsOnPlane(
                Marquee.StartPosition,
                Marquee.EndPosition,
                Camera,
                out var minX,
                out var maxX,
                out var minZ,
                out var maxZ);

            transform.position = new Vector3((minX + maxX) * 0.5f, 0f, (minZ + maxZ) * 0.5f);
            transform.localScale = new Vector3(maxX - minX, 1f, maxZ - minZ);
        }

        private static bool ShouldShow()
        {
            return InteractionContext.Marquee.IsActive
                && Input.GetMouseButton(0)
                && !InteractionContext.Drag.IsDragging;
        }
    }
}
