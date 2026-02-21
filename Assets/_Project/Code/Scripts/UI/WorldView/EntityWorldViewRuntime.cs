using System;
using System.Collections.Generic;
using Project.Entities;
using Project.Messages;
using Project.ScriptableObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.UI
{
    public sealed class EntityWorldViewRuntime : IDisposable
    {
        private readonly Dictionary<IEntity, WorldViewState> _activeWorldViews = new();
        private readonly ViewServiceConfig _config;

        public EntityWorldViewRuntime(ViewServiceConfig config)
        {
            _config = config;
        }

        public void HandleEatPreyMessage(IEntity entity)
        {
            if (_activeWorldViews.TryGetValue(entity, out var state))
            {
                state.Combo++;
                state.ExpireAt = Time.time + _config.WorldViewLifetimeSeconds;
                state.IsHiding = false;
                state.PendingRemove = false;
                state.View.CancelHide();
                state.View.SetHeader(FormatHeader(state.Combo));
                return;
            }

            var view = Object.Instantiate(_config.WorldViewPrefab, entity.GetWorldViewParent());
            
            var worldViewState = new WorldViewState
            {
                View = view,
                Combo = 1,
                ExpireAt = Time.time + _config.WorldViewLifetimeSeconds,
            };
            
            _activeWorldViews.Add(entity, worldViewState);
            view.PlayShow(FormatHeader(worldViewState.Combo));
        }

        public void Tick()
        {
            if (TryUpdateWorldViews(out var markedKey))
            {
                TryRemoveMarkedWorldView(markedKey);
            }
        }

        public void Dispose()
        {
            _activeWorldViews.Clear();
        }

        private bool TryUpdateWorldViews(out IEntity markedKey)
        {
            var hasMarkedKey = false;
            markedKey = null;

            foreach (var pair in _activeWorldViews)
            {
                var key = pair.Key;
                var state = pair.Value;
                if (TryMarkPendingRemove(state, key, ref hasMarkedKey, ref markedKey))
                {
                    continue;
                }

                TryStartHide(state);
            }

            return hasMarkedKey;
        }

        private bool TryMarkPendingRemove(
            WorldViewState state,
            IEntity key,
            ref bool hasMarkedKey,
            ref IEntity markedKey)
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

        private bool TryStartHide(WorldViewState state)
        {
            if (state.IsHiding || Time.time < state.ExpireAt)
            {
                return false;
            }

            state.IsHiding = true;
            state.View.PlayHide(() => state.PendingRemove = true);
            return true;
        }

        private bool TryRemoveMarkedWorldView(IEntity markedKey)
        {
            if (markedKey == null)
            {
                return false;
            }

            if (_activeWorldViews.Remove(markedKey, out var removedState))
            {
                Object.Destroy(removedState.View);
                return true;
            }

            return false;
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
