using UnityEngine;

namespace SimpleManipulationKit
{
    public static class RectTransformExtensions
    {
        public static Vector2 GetLocalPosition(this RectTransform rectTransform, Camera camera, Vector2 screenPos)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPos,
                camera,
                out var localPoint)
                ? localPoint
                : Vector2.zero;
        }

        public static Vector2 GetLocalPosition(this RectTransform rectTransform, Camera camera, float screenX, float screenY)
        {
            return rectTransform.GetLocalPosition(camera, new Vector2(screenX, screenY));
        }
    }
}
