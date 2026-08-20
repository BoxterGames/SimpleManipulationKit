using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public static class MarqueePlane
    {
        private static readonly Plane Plane = new(Vector3.up, Vector3.zero);

        public static Vector3 ScreenToPlane(Vector3 screen, Camera camera)
        {
            var ray = camera.ScreenPointToRay(screen);
            return Plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
        }

        public static void GetBounds(Vector3 a, Vector3 b, out float minX, out float maxX, out float minZ, out float maxZ)
        {
            minX = Mathf.Min(a.x, b.x);
            maxX = Mathf.Max(a.x, b.x);
            minZ = Mathf.Min(a.z, b.z);
            maxZ = Mathf.Max(a.z, b.z);
        }

        public static void GetBoundsXY(Vector3 a, Vector3 b, out float minX, out float maxX, out float minY, out float maxY)
        {
            minX = Mathf.Min(a.x, b.x);
            maxX = Mathf.Max(a.x, b.x);
            minY = Mathf.Min(a.y, b.y);
            maxY = Mathf.Max(a.y, b.y);
        }

        public static bool Contains(Vector3 world, float minX, float maxX, float minZ, float maxZ)
        {
            return world.x >= minX && world.x <= maxX
                && world.z >= minZ && world.z <= maxZ;
        }

        public static Vector3 ScreenToXYPlane(Vector3 screen, Camera camera)
        {
            var ray = camera.ScreenPointToRay(screen);
            var plane = new Plane(Vector3.forward, Vector3.zero);
            return plane.Raycast(ray, out var enter) ? ray.GetPoint(enter) : Vector3.zero;
        }

        public static bool ContainsXY(Vector3 world, float minX, float maxX, float minY, float maxY)
        {
            return world.x >= minX && world.x <= maxX
                && world.y >= minY && world.y <= maxY;
        }
    }
}
