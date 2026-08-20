using SimpleManipulationKit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    [DefaultExecutionOrder(-50)]
    public sealed class MarqueeController : MonoBehaviour
    {
        private const float MinScreenDistance = 20f;

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private Vector3 fromScreen;
        private bool wasDragging;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            {
                fromScreen = Input.mousePosition;
                Marquee.BeginMarquee(Project(fromScreen));
            }

            if (Input.GetMouseButton(0) && Marquee.IsActive)
            {
                UpdateMarquee();
            }

            if (Input.GetMouseButtonUp(0))
            {
                CompleteMarquee();
            }

            wasDragging |= InteractionContext.Drag.IsDragging;
        }

        private void UpdateMarquee()
        {
            var toScreen = (Vector3)Input.mousePosition;

            if (((Vector2)toScreen - (Vector2)fromScreen).magnitude < MinScreenDistance)
            {
                Marquee.UpdateMarquee(Marquee.StartPosition);
                return;
            }

            Marquee.UpdateMarquee(Project(toScreen));
        }

        private void CompleteMarquee()
        {
            if (wasDragging || !Marquee.IsActive)
            {
                wasDragging = false;
                Marquee.CancelMarquee();
                return;
            }

            Marquee.EndMarquee(Marquee.EndPosition);
        }

        private Vector3 Project(Vector3 screen)
        {
            var view = Marquee.View;
            if (view == null)
            {
                return screen;
            }

            var camera = Marquee.Camera != null ? Marquee.Camera : Camera.main;
            return view.Project(screen, camera, transform);
        }

        private static bool IsPointerOverUI() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
