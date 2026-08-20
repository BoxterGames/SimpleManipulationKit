using System.Linq;
using SimpleManipulationKit;
using UnityEngine;

namespace SimpleManipulationKit.Internal
{
    public sealed class MarqueSelectController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeReference] private IMarqueeCalculator marqueeCalculator = new SelectMarqueeXoZ();

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
            marqueeCalculator ??= new SelectMarqueeXoZ();
            Marquee.OnMarqueeEnd += HandleMarqueeEnd;
        }

        private void OnDestroy()
        {
            Marquee.OnMarqueeEnd -= HandleMarqueeEnd;
        }

        private void HandleMarqueeEnd(Vector3 start, Vector3 end)
        {
            if (Selectable is null)
            {
                return;
            }

            marqueeCalculator.OnMarqueeEnd(Selectable, start, end);
        }
    }
}
