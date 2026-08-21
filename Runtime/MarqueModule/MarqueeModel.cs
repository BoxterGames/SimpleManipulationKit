using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class MarqueeModel
    {
        private readonly HashSet<ISelectable> replaceBatch = new();

        private Vector3 startWorld;
        private Vector3 endWorld;

        public bool IsActive { get; private set; }

        public Vector3 StartScreen => WorldToScreen(startWorld);
        public Vector3 EndScreen => WorldToScreen(endWorld);

        public event Action<Vector3> OnMarqueeStart;
        public event Action<Vector3, Vector3> OnMarqueeUpdate;
        public event Action<Vector3, Vector3> OnMarqueeEnd;

        public void BeginMarquee(Vector3 startScreen)
        {
            startWorld = ScreenToWorld(startScreen);
            endWorld = startWorld;
            IsActive = true;
            OnMarqueeStart?.Invoke(StartScreen);
        }

        public void UpdateMarquee(Vector3 endScreen)
        {
            if (!IsActive)
            {
                return;
            }

            endWorld = ScreenToWorld(endScreen);
            OnMarqueeUpdate?.Invoke(StartScreen, EndScreen);
        }

        public void EndMarquee()
        {
            if (!IsActive)
            {
                return;
            }

            OnMarqueeEnd?.Invoke(StartScreen, EndScreen);

            if (replaceBatch.Count > 0)
            {
                InteractionContext.Selection.Set(replaceBatch);
            }

            replaceBatch.Clear();
            Clear();
        }

        public void Add(ISelectable selectable)
        {
            if (IsActive && selectable != null)
            {
                replaceBatch.Add(selectable);
            }
        }

        public void CancelMarquee()
        {
            replaceBatch.Clear();
            Clear();
        }

        private void Clear()
        {
            startWorld = Vector3.zero;
            endWorld = Vector3.zero;
            IsActive = false;
        }

        private static Vector3 ScreenToWorld(Vector3 screen)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return screen;
            }

            var ray = camera.ScreenPointToRay(screen);
            var plane = new Plane(camera.transform.forward, Vector3.zero);

            if (plane.Raycast(ray, out var enter))
            {
                return ray.GetPoint(enter);
            }

            screen.z = Mathf.Max(camera.nearClipPlane, 0.1f);
            return camera.ScreenToWorldPoint(screen);
        }

        private static Vector3 WorldToScreen(Vector3 world)
        {
            var camera = Camera.main;
            return camera != null ? camera.WorldToScreenPoint(world) : world;
        }
    }
}
