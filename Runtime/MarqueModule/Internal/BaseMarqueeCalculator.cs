using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public abstract class BaseMarqueeCalculator : IMarqueeCalculator
    {
        protected SelectionModel Selection => InteractionContext.Selection;
        protected MarqueeModel Marquee => InteractionContext.Marquee;

        private static Camera camera;

        protected static Camera MarqueeCamera => camera ??= Camera.main;

        public abstract void OnMarqueeEnd(ISelectable selectable, Vector3 start, Vector3 end);

        protected static Vector3 ScreenToPlane(Vector3 screen)
        {
            return MarqueePlane.ScreenToPlane(screen, MarqueeCamera);
        }

        protected static bool IsShiftPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        protected static bool IsControlPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        protected void ApplySelection(ISelectable selectable)
        {
            if (selectable is MonoBehaviour behaviour && !behaviour.isActiveAndEnabled)
            {
                return;
            }

            if (IsControlPressed)
            {
                Selection.Toggle(selectable);
                return;
            }

            if (IsShiftPressed)
            {
                Selection.Add(selectable);
                return;
            }

            Marquee.Add(selectable);
        }
    }
}
