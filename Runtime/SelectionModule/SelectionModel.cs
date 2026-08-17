using System;
using System.Collections.Generic;

namespace SimpleManipulationKit
{
    public sealed class SelectionModel
    {
        private readonly HashSet<ISelectable> selected = new();

        public event Action OnUpdateSelected;

        public IEnumerable<T> GetSelected<T>() where T : ISelectable
        {
            foreach (var item in selected)
            {
                if (item is T typed)
                {
                    yield return typed;
                }
            }
        }

        public bool Contains(ISelectable item) => item != null && selected.Contains(item);

        public void Add(ISelectable item)
        {
            if (item == null || !selected.Add(item))
            {
                return;
            }

            OnUpdateSelected?.Invoke();
        }

        public void Add<T>(IEnumerable<T> items) where T : ISelectable
        {
            var changed = false;
            foreach (var item in items)
            {
                if (item != null && selected.Add(item))
                {
                    changed = true;
                }
            }

            if (changed)
            {
                OnUpdateSelected?.Invoke();
            }
        }

        public void Add(IEnumerable<ISelectable> items) => Add<ISelectable>(items);

        public void Remove(ISelectable item)
        {
            if (item == null || !selected.Remove(item))
            {
                return;
            }

            OnUpdateSelected?.Invoke();
        }

        public void Remove<T>(IEnumerable<T> items) where T : ISelectable
        {
            var changed = false;
            foreach (var item in items)
            {
                if (item != null && selected.Remove(item))
                {
                    changed = true;
                }
            }

            if (changed)
            {
                OnUpdateSelected?.Invoke();
            }
        }

        public void Remove(IEnumerable<ISelectable> items) => Remove<ISelectable>(items);

        public void Toggle(ISelectable item)
        {
            if (item == null)
            {
                return;
            }

            if (!selected.Remove(item))
            {
                selected.Add(item);
            }

            OnUpdateSelected?.Invoke();
        }

        public void Set(ISelectable item)
        {
            if (item == null || selected.Count == 1 && selected.Contains(item))
            {
                return;
            }

            selected.Clear();
            selected.Add(item);
            OnUpdateSelected?.Invoke();
        }

        public void Set(IEnumerable<ISelectable> items)
        {
            var next = new HashSet<ISelectable>();
            foreach (var item in items)
            {
                if (item != null)
                {
                    next.Add(item);
                }
            }

            if (selected.SetEquals(next))
            {
                return;
            }

            selected.Clear();
            selected.UnionWith(next);
            OnUpdateSelected?.Invoke();
        }

        public void Clear()
        {
            if (selected.Count == 0)
            {
                return;
            }

            selected.Clear();
            OnUpdateSelected?.Invoke();
        }
    }
}
