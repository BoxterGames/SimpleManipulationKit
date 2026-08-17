using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public sealed class TableSpace : IInteractionSpace
    {
        private readonly Transform table;

        public TableSpace(Transform table) => this.table = table;

        public Vector3 ScreenToLocal(Vector3 screen, Camera camera)
        {
            var ray = camera.ScreenPointToRay(screen);
            var plane = table != null
                ? new Plane(table.forward, table.position)
                : new Plane(Vector3.up, Vector3.zero);

            return plane.Raycast(ray, out var enter)
                ? WorldToLocal(ray.GetPoint(enter))
                : Vector3.zero;
        }

        public Vector3 WorldToLocal(Vector3 world) =>
            table != null
                ? table.InverseTransformPoint(world)
                : new Vector3(world.x, world.z, 0f);

        public Vector3 LocalToWorld(Vector3 local) =>
            table != null
                ? table.TransformPoint(new Vector3(local.x, local.y, 0f))
                : new Vector3(local.x, 0f, local.y);

        public static void GetBoundsXY(Vector3 start, Vector3 end, out float minX, out float maxX, out float minY, out float maxY)
        {
            minX = Mathf.Min(start.x, end.x);
            maxX = Mathf.Max(start.x, end.x);
            minY = Mathf.Min(start.y, end.y);
            maxY = Mathf.Max(start.y, end.y);
        }

        public static bool ContainsXY(Vector3 tableLocal, float minX, float maxX, float minY, float maxY) =>
            tableLocal.x >= minX && tableLocal.x <= maxX
            && tableLocal.y >= minY && tableLocal.y <= maxY;
    }
}
