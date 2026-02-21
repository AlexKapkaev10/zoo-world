using System;
using MessagePipe;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.UI.MVP;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Project.UI
{
    public interface IViewService : IInitializable, IDisposable
    {
        
    }
    
    public class ViewService : IViewService
    {
        private readonly IInfoPresenter _infoPresenter;
        private readonly ViewServiceConfig _config;
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
            Object.Instantiate(_config.WorldViewPrefab, message.Killer.GetWorldViewParent());
        }
        
        public void Initialize()
        {
            _infoPresenter.SetActive(true);
        }

        public void Dispose()
        {
            _eatPreySubscription?.Dispose();
        }
    }
}