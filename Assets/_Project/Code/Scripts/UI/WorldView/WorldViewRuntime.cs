using System;
using System.Collections.Generic;
using Project.ScriptableObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.UI
{
    public sealed class WorldViewRuntime : IDisposable
    {
        private readonly Dictionary<Transform, WorldViewState> _activeWorldViews = new();
        private readonly ViewServiceConfig _config;

        public WorldViewRuntime(ViewServiceConfig config)
        {
            _config = config;
        }

        public void Handle(Transform anchor)
        {
            if (anchor == null)
            {
                return;
            }

            if (TryRefreshExisting(anchor))
            {
                return;
            }

            var view = Object.Instantiate(_config.WorldViewPrefab, anchor);

            var worldViewState = new WorldViewState
            {
                View = view,
                Combo = 1,
                ExpireAt = Time.time + _config.WorldViewLifetime,
            };

            _activeWorldViews.Add(anchor, worldViewState);
            view.PlayShow(FormatHeader(worldViewState.Combo));
        }

        public void Tick()
        {
            if (TryUpdateWorldViews(out var markedKey))
            {
                RemoveMarkedWorldView(markedKey);
            }
        }

        public void Dispose()
        {
            foreach (var pair in _activeWorldViews)
            {
                if (pair.Value?.View != null)
                {
                    Object.Destroy(pair.Value.View.gameObject);
                }
            }

            _activeWorldViews.Clear();
        }

        private bool TryRefreshExisting(Transform anchor)
        {
            if (!_activeWorldViews.TryGetValue(anchor, out var state))
            {
                return false;
            }

            RefreshState(state);
            UpdateView(state);
            return true;
        }

        private void RefreshState(WorldViewState state)
        {
            state.Combo++;
            state.ExpireAt = Time.time + _config.WorldViewLifetime;
            state.IsHiding = false;
            state.PendingRemove = false;
        }

        private void UpdateView(WorldViewState state)
        {
            state.View.CancelHide();
            state.View.SetHeader(FormatHeader(state.Combo));
        }

        private bool TryUpdateWorldViews(out Transform markedKey)
        {
            var hasMarkedKey = false;
            markedKey = null;

            foreach (var pair in _activeWorldViews)
            {
                var key = pair.Key;
                var state = pair.Value;

                if (TryMarkInactiveAnchor(key, ref hasMarkedKey, ref markedKey))
                {
                    continue;
                }

                if (TryMarkPendingRemove(state, key, ref hasMarkedKey, ref markedKey))
                {
                    continue;
                }

                StartHide(state);
            }

            return hasMarkedKey;
        }

        private bool TryMarkInactiveAnchor(
            Transform key,
            ref bool hasMarkedKey,
            ref Transform markedKey)
        {
            if (key != null && key.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (hasMarkedKey)
            {
                return true;
            }

            hasMarkedKey = true;
            markedKey = key;
            return true;
        }

        private bool TryMarkPendingRemove(
            WorldViewState state,
            Transform key,
            ref bool hasMarkedKey,
            ref Transform markedKey)
        {
            if (!state.PendingRemove)
            {
                return false;
            }

            if (hasMarkedKey)
            {
                return true;
            }

            hasMarkedKey = true;
            markedKey = key;
            return true;
        }

        private void StartHide(WorldViewState state)
        {
            if (state.IsHiding || Time.time < state.ExpireAt)
            {
                return;
            }

            state.IsHiding = true;
            state.View.PlayHide(() => state.PendingRemove = true);
        }

        private void RemoveMarkedWorldView(Transform markedKey)
        {
            if (markedKey == null)
            {
                return;
            }

            if (!_activeWorldViews.Remove(markedKey, out var removedState))
            {
                return;
            }

            if (removedState?.View != null)
            {
                Object.Destroy(removedState.View.gameObject);
            }
        }

        private string FormatHeader(int combo)
        {
            return combo <= 1
                ? _config.WorldViewHeader
                : string.Format(
                    _config.WorldViewComboFormat,
                    _config.WorldViewHeader,
                    combo);
        }
    }
}