using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleManipulationKit.Internal
{
    [RequireComponent(typeof(Collider))]
    public class ManipulationController3D : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour view;
        [SerializeField] private bool canMarqueeSelect = true;
        [SerializeField] private Camera interactionCamera;
        [SerializeReference, Attributes] private ISelectionCalculator selectionCalculator = new MultiSelection();
        [SerializeReference, Attributes] private ISpaceConverter spaceConverter = new XoZSpaceConverter();

        private DragCalculator dragCalculator;
        private Vector3 globalStartPoint;

        private IDraggable Draggable => view as IDraggable;
        private MarqueeModel Marquee => InteractionContext.Marquee;

        private void Awake()
        {
            dragCalculator = new DragCalculator(selectionCalculator, spaceConverter);
            Marquee.OnMarqueeStart += HandleMarqueeStart;
            Marquee.OnMarqueeEnd += HandleMarqueeEnd;
        }

        private void OnDestroy()
        {
            Marquee.OnMarqueeStart -= HandleMarqueeStart;
            Marquee.OnMarqueeEnd -= HandleMarqueeEnd;
        }

        private void OnDisable()
        {
            InteractionContext.Selection.Remove(Draggable);
        }

        private void OnValidate()
        {
            if (view is not null && Draggable is null)
            {
                view = null;
            }

            view ??= GetComponentsInChildren<MonoBehaviour>(true).FirstOrDefault(x => x is IDraggable);
        }

        private void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)
                || EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            dragCalculator.TryBeginDrag(Draggable, Input.mousePosition);
        }

        private void Update()
        {
            dragCalculator.UpdateDrag(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            dragCalculator.EndDrag(Input.mousePosition);
        }

        private void HandleMarqueeStart(Vector3 startScreen)
        {
            globalStartPoint = spaceConverter.ScreenToWorldPoint(transform, startScreen);
        }

        private void HandleMarqueeEnd(Vector3 startScreen, Vector3 endScreen)
        {
            if (!canMarqueeSelect ||
                Draggable is not MonoBehaviour mono ||
                !mono.isActiveAndEnabled)
            {
                return;
            }

            var camera = interactionCamera != null ? interactionCamera : Camera.main;
            var adjustedStartScreen = camera.WorldToScreenPoint(globalStartPoint);

            if (!spaceConverter.IsIntersect(transform, adjustedStartScreen, endScreen))
            {
                return;
            }

            Marquee.Add(Draggable);
        }
    }
}
