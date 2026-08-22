using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public interface ISpaceConverter
    {
        Vector3 ScreenToWorldPoint(Transform reference, Vector2 screenPoint);
        Vector3 ScreenToLocalPoint(Transform reference, Vector3 screenPoint);
        Vector3 GetSize(Transform reference, Vector3 localA, Vector3 localB);
        Vector3 GetCenterPosition(Transform reference, Vector3 localA, Vector3 localB);
        bool IsIntersect(Transform reference, Vector3 screenA, Vector3 screenB);
    }
}
