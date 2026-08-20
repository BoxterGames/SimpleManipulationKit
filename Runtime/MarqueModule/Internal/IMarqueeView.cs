using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public interface IMarqueeView
    {
        void Apply(Transform target, Vector3 screenStart, Vector3 screenEnd, Camera camera);
        bool Contains(Vector3 world, Vector3 screenStart, Vector3 screenEnd, Camera camera);
    }
}
