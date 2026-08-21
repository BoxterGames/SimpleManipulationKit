using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    [DefaultExecutionOrder(-50)]
    public sealed class MarqueeController : MonoBehaviour
    {
        private MarqueeModel Marquee => InteractionContext.Marquee;
        private bool wasDragging;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            {
                Marquee.BeginMarquee(Input.mousePosition);
            }

            if (Input.GetMouseButton(0) && Marquee.IsActive)
            {
                Marquee.UpdateMarquee(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                CompleteMarquee();
            }

            wasDragging |= InteractionContext.Drag.IsDragging;
        }

        private void CompleteMarquee()
        {
            if (wasDragging || !Marquee.IsActive)
            {
                wasDragging = false;
                Marquee.CancelMarquee();
                return;
            }

            Marquee.EndMarquee();
        }

        private static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
