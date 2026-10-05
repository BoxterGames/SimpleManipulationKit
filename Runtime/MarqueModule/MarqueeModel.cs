using System;
using System.Collections.Generic;
using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class MarqueeModel
    {
        private readonly HashSet<ISelectable> replaceBatch = new();
        private readonly MultiSelection selectionCalculator = new();

        public bool IsActive { get; private set; }

        public Vector3 StartScreen { get; private set; }
        public Vector3 EndScreen { get; private set; }

        public event Action<Vector3> OnMarqueeStart;
        public event Action<Vector3, Vector3> OnMarqueeUpdate;
        public event Action<Vector3, Vector3> OnMarqueeEnd;

        internal void BeginMarquee(Vector3 startScreen)
        {
            StartScreen = startScreen;
            EndScreen = startScreen;
            IsActive = true;
            OnMarqueeStart?.Invoke(StartScreen);
        }

        internal void UpdateMarquee(Vector3 endScreen)
        {
            if (!IsActive)
            {
                return;
            }

            EndScreen = endScreen;
            OnMarqueeUpdate?.Invoke(StartScreen, EndScreen);
        }

        internal void EndMarquee()
        {
            if (!IsActive)
            {
                return;
            }

            OnMarqueeEnd?.Invoke(StartScreen, EndScreen);
            selectionCalculator.Select(replaceBatch);
            replaceBatch.Clear();
            Clear();
        }

        internal void Add(ISelectable selectable)
        {
            if (IsActive && selectable != null)
            {
                replaceBatch.Add(selectable);
            }
        }

        internal void CancelMarquee()
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
