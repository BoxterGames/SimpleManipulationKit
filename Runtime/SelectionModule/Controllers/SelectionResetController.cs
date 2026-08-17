using UnityEngine;
using UnityEngine.EventSystems;
using SimpleManipulationKit;

namespace SimpleManipulationKit.Internal
{
    public sealed class SelectionResetController : MonoBehaviour
    {
        private SelectionModel Selection => InteractionContext.Selection;

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Selection.Clear();
                return;
            }

            if (!Input.GetMouseButtonDown(0)
                || HasModifier()
                || IsPointerOverUI()
                || HitSelectable())
            {
                return;
            }

            Selection.Clear();
        }

        private static bool HasModifier()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || 
                   Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }

        private static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private static bool HitSelectable()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return false;
            }

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var hit)
                && hit.collider.GetComponentInParent<ISelectable>() != null;
        }
    }
}
