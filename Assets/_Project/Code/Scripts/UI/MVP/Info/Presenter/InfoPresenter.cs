using System;
using System.Collections.Generic;
using MessagePipe;
using Project.Messages;
using Project.ScriptableObjects;
using VContainer;
using Object = UnityEngine.Object;

namespace Project.UI.MVP
{
    public sealed class InfoPresenter : IInfoPresenter
    {
        private readonly InfoPresenterConfig _config;
        private readonly Dictionary<InfoCounterKind, Action<string>> _viewUpdateMap = new();
        private readonly IInfoModel _model;
        private readonly IDisposable _eatPreySubscription;

        private IInfoView _view;

        [Inject]
        public InfoPresenter(IInfoModel model, 
            ISubscriber<EatPreyMessage> eatPreySubscriber, 
            InfoPresenterConfig config)
        {
            _config = config;
            _model = model;

            _eatPreySubscription = eatPreySubscriber.Subscribe(OnEatPreyMessage);
        }

        void IInfoPresenter.SetActive(bool isActive)
        {
            if (isActive)
            {
                CreateView();
            }
            else
            {
                _view?.Destroy();
                Clear();
            }
        }

        void IDisposable.Dispose()
        {
            _eatPreySubscription?.Dispose();
            Clear();
        }

        private void CreateView()
        {
            if (_view != null)
            {
                return;
            }
                
            _view = Object.Instantiate(_config.ViewPrefab);
            BindViewHandlers();
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            if (_view == null)
            {
                return;
            }

            var infoModelData = _model.CalculateInfoData(message.Killed.Data);
            
            if (_viewUpdateMap.TryGetValue(infoModelData.Kind, out var updateViewAction))
            {
                updateViewAction.Invoke(infoModelData.Value.ToString());
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