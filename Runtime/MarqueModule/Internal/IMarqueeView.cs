using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public interface IMarqueeView
    {
        Vector3 Project(Vector3 screen, Camera camera, Transform target);
        void Apply(Transform target, Vector3 start, Vector3 end);
        bool Contains(Vector3 world, Vector3 start, Vector3 end);
    }
}
