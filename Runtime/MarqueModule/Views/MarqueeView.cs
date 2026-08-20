using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    [DefaultExecutionOrder(-100)]
    public sealed class MarqueeView : MonoBehaviour
    {
        [SerializeField] private Camera interactionCamera;
        [SerializeReference] private IMarqueeView view = new XoZMarquee();

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private Camera Camera =>
            interactionCamera != null ? interactionCamera : UnityEngine.Camera.main;

        private void OnValidate()
        {
            view ??= new XoZMarquee();
            PrepareUnitScale();
        }

        private void Awake()
        {
            view ??= new XoZMarquee();
            PrepareUnitScale();
            Bind();
        }

        private void OnEnable()
        {
            Bind();
        }

        private void LateUpdate()
        {
            Bind();

            if (!ShouldShow() || view is null)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            view.Apply(transform, Marquee.StartPosition, Marquee.EndPosition);
        }

        private void Bind()
        {
            Marquee.View = view;
            Marquee.Camera = Camera;
        }

        private void PrepareUnitScale()
        {
            if (transform is RectTransform rectTransform)
            {
                rectTransform.sizeDelta = Vector2.one;
            }
        }

        private static bool ShouldShow()
        {
            return InteractionContext.Marquee.IsActive
                && Input.GetMouseButton(0)
                && !InteractionContext.Drag.IsDragging;
        }
    }
}
