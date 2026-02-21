using System;
using MessagePipe;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.UI.MVP;
using VContainer;
using VContainer.Unity;

namespace Project.UI
{
    public class ViewService : IViewService, ITickable
    {
        private readonly EntityWorldViewRuntime _worldViewRuntime;
        private readonly IInfoPresenter _infoPresenter;
        private readonly IDisposable _eatPreySubscription;

        [Inject]
        public ViewService(IGameScopeFactory factory, ViewServiceConfig config)
        {
            _infoPresenter = factory.Get<IInfoPresenter>();
            
            _worldViewRuntime = new EntityWorldViewRuntime(config);
            
            _eatPreySubscription = factory
                .Get<ISubscriber<EatPreyMessage>>()
                .Subscribe(OnEatPreyMessage);
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            _worldViewRuntime.HandleEatPreyMessage(message.Killer);
        }
        
        public void Initialize()
        {
            _infoPresenter.SetActive(true);
        }

        public void Tick()
        {
            _worldViewRuntime.Tick();
        }

        public void Dispose()
        {
            _eatPreySubscription?.Dispose();
            _worldViewRuntime.Dispose();
        }
    }
}