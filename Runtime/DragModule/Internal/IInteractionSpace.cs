using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public interface IInteractionSpace
    {
        Vector3 ScreenToLocal(Vector3 screen, Camera camera);
        Vector3 LocalToWorld(Vector3 local);
    }
}
