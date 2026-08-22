using System.Linq;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public sealed class MarqueSelectController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeField] private Camera interactionCamera;
        [SerializeReference, Attributes] private ISpaceConverter interactionSpace = new XYSpaceConverter();

        private ISelectable Selectable => view as ISelectable;

        private MarqueeModel Marquee => InteractionContext.Marquee;

        private Vector3 globalStartPoint;
        
        private void OnValidate()
        {
            if (view is not null && Selectable is null)
            {
                view = null;
            }

            view ??= GetComponentsInChildren<MonoBehaviour>(true).FirstOrDefault(x => x is ISelectable);
        }

        private void Awake()
        {
            Marquee.OnMarqueeStart += HandleMarqueeStart;
            Marquee.OnMarqueeEnd += HandleMarqueeEnd;
        }

        private void OnDestroy()
        {
            Marquee.OnMarqueeStart -= HandleMarqueeStart;
            Marquee.OnMarqueeEnd -= HandleMarqueeEnd;
        }

        private void HandleMarqueeStart(Vector3 startScreen)
        {
            globalStartPoint = interactionSpace.ScreenToWorldPoint(transform, startScreen);
        }

        private void HandleMarqueeEnd(Vector3 startScreen, Vector3 endScreen)
        {
            if (Selectable is not MonoBehaviour mono || 
                !mono.isActiveAndEnabled)
            {
                return;
            }

            var camera = interactionCamera != null ? interactionCamera : Camera.main;
            var adjustedStartScreen = camera.WorldToScreenPoint(globalStartPoint);

            if (!interactionSpace.IsIntersect(transform, adjustedStartScreen, endScreen))
            {
                return;
            }
            
            Marquee.Add(Selectable);   
        }
    }
}
