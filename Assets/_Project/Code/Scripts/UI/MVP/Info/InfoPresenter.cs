using System;
using MessagePipe;
using Project.Entities;
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
            if (isActive && _view == null)
            {
                _view = Object.Instantiate(_config.ViewPrefab);
            }
            else
            {
                _view.Destroy();
                _view = null;
            }
        }

        public void Dispose()
        {
            _eatPreySubscription?.Dispose();
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            var killedData = message.Killed.Data;
            
            _model.UpdateCounter(killedData, out int killedCount);

            switch (killedData.Kind)
            {
                case EntityKind.Hunter:
                    _view.UpdateHuntersInfo(killedCount.ToString());
                    break;
                case EntityKind.Frog:
                case EntityKind.Snake:
                    _view.UpdateAnimalsInfo(killedCount.ToString());
                    break;
            }
        }
    }
}