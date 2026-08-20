using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    public static class SelectableBounds
    {
        public static bool IntersectsMarqueeXoZ(ISelectable selectable, Vector3 start, Vector3 end)
        {
            return Intersects(selectable, start, end);
        }

        public static bool IntersectsMarqueeXY(ISelectable selectable, Vector3 start, Vector3 end)
        {
            return Intersects(selectable, start, end);
        }

        private static bool Intersects(ISelectable selectable, Vector3 start, Vector3 end)
        {
            if (selectable is not MonoBehaviour behaviour)
            {
                return false;
            }

            var plane = InteractionContext.Marquee.View;
            if (plane == null)
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
                    if (plane.Contains(corner, start, end))
                    {
                        return true;
                    }
                }

                return false;
            }

            return plane.Contains(transform.position, start, end);
        }
    }
}
