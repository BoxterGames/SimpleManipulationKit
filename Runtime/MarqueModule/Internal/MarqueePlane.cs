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

        public static void GetScreenRectBoundsOnPlane(
            Vector3 screenStart,
            Vector3 screenEnd,
            Camera camera,
            out float minX,
            out float maxX,
            out float minZ,
            out float maxZ)
        {
            var minScreen = Vector2.Min(screenStart, screenEnd);
            var maxScreen = Vector2.Max(screenStart, screenEnd);

            var topLeft = ScreenToPlane(new Vector3(minScreen.x, maxScreen.y, 0f), camera);
            var topRight = ScreenToPlane(new Vector3(maxScreen.x, maxScreen.y, 0f), camera);
            var bottomRight = ScreenToPlane(new Vector3(maxScreen.x, minScreen.y, 0f), camera);
            var bottomLeft = ScreenToPlane(new Vector3(minScreen.x, minScreen.y, 0f), camera);

            minX = Mathf.Min(topLeft.x, topRight.x, bottomRight.x, bottomLeft.x);
            maxX = Mathf.Max(topLeft.x, topRight.x, bottomRight.x, bottomLeft.x);
            minZ = Mathf.Min(topLeft.z, topRight.z, bottomRight.z, bottomLeft.z);
            maxZ = Mathf.Max(topLeft.z, topRight.z, bottomRight.z, bottomLeft.z);
        }

        public static bool Contains(Vector3 world, float minX, float maxX, float minZ, float maxZ)
        {
            return world.x >= minX && world.x <= maxX
                && world.z >= minZ && world.z <= maxZ;
        }
    }
}
