using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    [DefaultExecutionOrder(-100)]
    public sealed class MarqueeView : MonoBehaviour
    {
        [SerializeReference, Attributes] private ISpaceConverter interactionSpace = new ScreenSpaceConverter();

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private void Awake()
        {
            transform.localScale = interactionSpace.GetSize(transform, Vector3.zero, Vector3.zero);
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
            
            transform.position = interactionSpace.GetCenterPosition(transform, Marquee.StartScreen, Marquee.EndScreen);
            transform.localScale = interactionSpace.GetSize(transform, Marquee.StartScreen, Marquee.EndScreen);
        }
    }
}
