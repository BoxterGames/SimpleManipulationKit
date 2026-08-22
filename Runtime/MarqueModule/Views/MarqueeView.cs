using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    [DefaultExecutionOrder(-100)]
    public sealed class MarqueeView : MonoBehaviour
    {
        [SerializeReference, Attributes] private ISpaceConverter interactionSpace = new ScreenSpaceConverter();

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private Vector3 localStartPoint;

        private void Awake()
        {
            Marquee.OnMarqueeStart += HandleMarqueeStart;
            transform.localScale = interactionSpace.GetSize(transform, Vector3.zero, Vector3.zero);
        }

        private void OnDestroy()
        {
            Marquee.OnMarqueeStart -= HandleMarqueeStart;
        }

        private void HandleMarqueeStart(Vector3 startScreen)
        {
            localStartPoint = interactionSpace.ScreenToLocalPoint(transform, startScreen);
        }

        private void LateUpdate()
        {
            if (!InteractionContext.Marquee.IsActive
                || !Input.GetMouseButton(0)
                || InteractionContext.Drag.IsDragging)
            {
                transform.localScale = interactionSpace.GetSize(transform, Vector3.zero, Vector3.zero);
                return;
            }

            var endLocal = interactionSpace.ScreenToLocalPoint(transform, Marquee.EndScreen);

            transform.position = interactionSpace.GetCenterPosition(transform, localStartPoint, endLocal);
            transform.localScale = interactionSpace.GetSize(transform, localStartPoint, endLocal);
        }
    }
}
