using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class MarqueeModel
    {
        private readonly HashSet<ISelectable> replaceBatch = new();

        public bool IsActive { get; private set; }

        public Vector3 StartScreen { get; private set; }
        public Vector3 EndScreen { get; private set; }

        public event Action<Vector3> OnMarqueeStart;
        public event Action<Vector3, Vector3> OnMarqueeUpdate;
        public event Action<Vector3, Vector3> OnMarqueeEnd;

        public void BeginMarquee(Vector3 startScreen)
        {
            StartScreen = startScreen;
            EndScreen = startScreen;
            IsActive = true;
            OnMarqueeStart?.Invoke(StartScreen);
        }

        public void UpdateMarquee(Vector3 endScreen)
        {
            if (!IsActive)
            {
                return;
            }

            EndScreen = endScreen;
            OnMarqueeUpdate?.Invoke(StartScreen, EndScreen);
        }

        public void EndMarquee()
        {
            if (!IsActive)
            {
                return;
            }

            OnMarqueeEnd?.Invoke(StartScreen, EndScreen);
            InteractionContext.Selection.Set(replaceBatch);
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
            IsActive = false;
        }
    }
}
