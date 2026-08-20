using System;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    [Serializable]
    public sealed class XoZMarquee : IMarqueeView
    {
        public Vector3 Project(Vector3 screen, Camera camera, Transform target)
        {
            return MarqueePlane.ScreenToPlane(screen, camera);
        }

        public void Apply(Transform target, Vector3 start, Vector3 end)
        {
            MarqueePlane.GetBounds(start, end, out var minX, out var maxX, out var minZ, out var maxZ);

            target.position = new Vector3((minX + maxX) * 0.5f, 0f, (minZ + maxZ) * 0.5f);
            target.localScale = new Vector3(maxX - minX, 1f, maxZ - minZ);
        }

        public bool Contains(Vector3 world, Vector3 start, Vector3 end)
        {
            MarqueePlane.GetBounds(start, end, out var minX, out var maxX, out var minZ, out var maxZ);
            return MarqueePlane.Contains(world, minX, maxX, minZ, maxZ);
        }
    }
}
