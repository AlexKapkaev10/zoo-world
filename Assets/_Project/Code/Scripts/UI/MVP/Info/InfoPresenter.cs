using System;
using System.Collections.Generic;
using MessagePipe;
using Project.Messages;
using Project.ScriptableObjects;
using VContainer;
using Object = UnityEngine.Object;

namespace Project.UI.MVP
{
    public interface IInfoPresenter : IDisposable
    {
        void SetActive(bool isActive);
    }
    
    public sealed class InfoPresenter : IInfoPresenter
    {
        private readonly InfoPresenterConfig _config;
        private readonly Dictionary<InfoCounterKind, Action<string>> _viewUpdateMap = new();
        private readonly IInfoModel _model;
        private readonly IDisposable _eatPreySubscription;

        private IInfoView _view;

        [Inject]
        public InfoPresenter(ISubscriber<EatPreyMessage> eatPreySubscriber, InfoPresenterConfig config)
        {
            _config = config;
            _model = new InfoModel();

            _eatPreySubscription = eatPreySubscriber.Subscribe(OnEatPreyMessage);
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                if (_view != null)
                {
                    return;
                }
                
                _view = Object.Instantiate(_config.ViewPrefab);
                BindViewHandlers();

                return;
            }
            
            _view?.Destroy();

            Clear();
        }

        public void Dispose()
        {
            _eatPreySubscription?.Dispose();
            Clear();
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            if (_view == null)
            {
                return;
            }

            var counterUpdate = _model.UpdateCounter(message.Killed.Data);
            if (_viewUpdateMap.TryGetValue(counterUpdate.Kind, out var updateView))
            {
                updateView.Invoke(counterUpdate.Value.ToString());
            }
        }

        private void BindViewHandlers()
        {
            _viewUpdateMap[InfoCounterKind.Hunters] = _view.UpdateHuntersInfo;
            _viewUpdateMap[InfoCounterKind.Animals] = _view.UpdateAnimalsInfo;
        }

        private void Clear()
        {
            _view = null;
            _viewUpdateMap.Clear();
        }
    }
}