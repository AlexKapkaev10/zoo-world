using System;
using System.Collections.Generic;
using MessagePipe;
using Project.Entities;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.UI.MVP;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Project.UI
{
    public class ViewService : IViewService, ITickable
    {
        private readonly Dictionary<IEntity, WorldViewState> _activeWorldViews = new();
        private readonly ViewServiceConfig _config;
        private readonly IInfoPresenter _infoPresenter;
        private readonly IDisposable _eatPreySubscription;

        [Inject]
        public ViewService(IGameScopeFactory factory, ViewServiceConfig config)
        {
            _infoPresenter = factory.Get<IInfoPresenter>();
            _eatPreySubscription = factory
                .Get<ISubscriber<EatPreyMessage>>()
                .Subscribe(OnEatPreyMessage);
            
            _config = config;
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            if (_activeWorldViews.TryGetValue(message.Killer, out var state))
            {
                state.Combo++;
                state.ExpireAt = Time.time + _config.WorldViewLifetimeSeconds;
                state.IsHiding = false;
                state.PendingRemove = false;
                state.View.CancelHide();
                state.View.SetHeader(FormatHeader(state.Combo));
                return;
            }

            var view = Object.Instantiate(_config.WorldViewPrefab, message.Killer.GetWorldViewParent());
            var worldViewState = new WorldViewState
            {
                View = view,
                Combo = 1,
                ExpireAt = Time.time + _config.WorldViewLifetimeSeconds,
            };
            _activeWorldViews.Add(message.Killer, worldViewState);
            view.PlayShow(FormatHeader(worldViewState.Combo));
        }
        
        public void Initialize()
        {
            _infoPresenter.SetActive(true);
        }

        public void Tick()
        {
            if (TryUpdateWorldViews(out IEntity markedKey))
            {
                TryRemoveMarkedWorldView(markedKey);
            }
        }

        public void Dispose()
        {
            _eatPreySubscription?.Dispose();
            ClearState();
        }

        private void ClearState()
        {
            _activeWorldViews.Clear();
        }

        private bool TryUpdateWorldViews(out IEntity markedKey)
        {
            bool hasMarkedKey = false;
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
            if (combo <= 1)
            {
                return _config.WorldViewHeader;
            }

            return _config.WorldViewHeader + " x" + combo;
        }
    }
}