using System;
using System.Collections.Generic;
using SimpleManipulationKit.Internal;
using UnityEngine;

namespace SimpleManipulationKit
{
    public sealed class MarqueeModel
    {
        private readonly HashSet<ISelectable> replaceBatch = new();

        public bool IsActive { get; private set; }
        public Vector3 StartPosition { get; private set; }
        public Vector3 EndPosition { get; private set; }
        public IMarqueeView View { get; set; } = new XoZMarquee();
        public Camera Camera { get; set; }

        public event Action<Vector3> OnMarqueeStart;
        public event Action<Vector3, Vector3> OnMarqueeUpdate;
        public event Action<Vector3, Vector3> OnMarqueeEnd;

        public void BeginMarquee(Vector3 start)
        {
            StartPosition = start;
            EndPosition = start;
            IsActive = true;
            OnMarqueeStart?.Invoke(start);
        }

        public void UpdateMarquee(Vector3 end)
        {
            if (!IsActive)
            {
                return;
            }

            EndPosition = end;
            OnMarqueeUpdate?.Invoke(StartPosition, end);
        }

        public void EndMarquee(Vector3 end)
        {
            if (!IsActive)
            {
                return;
            }

            EndPosition = end;
            replaceBatch.Clear();
            OnMarqueeEnd?.Invoke(StartPosition, end);

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
            StartPosition = Vector3.zero;
            EndPosition = Vector3.zero;
            IsActive = false;
        }
    }
}
