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
            Marquee.OnMarqueeEnd += HandleMarqueeEnd;
        }

        private void OnDestroy()
        {
            Marquee.OnMarqueeEnd -= HandleMarqueeEnd;
        }

        private void HandleMarqueeEnd(Vector3 startScreen, Vector3 endScreen)
        {
            if (Selectable is not MonoBehaviour mono || 
                !mono.isActiveAndEnabled ||
                !interactionSpace.IsIntersect(transform, startScreen, endScreen))
            {
                return;
            }
            
            Marquee.Add(Selectable);   
        }
    }
}
