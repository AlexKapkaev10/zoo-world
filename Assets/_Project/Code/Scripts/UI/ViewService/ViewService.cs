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
        private readonly WorldViewRuntime _worldViewRuntime;
        private readonly IInfoPresenter _infoPresenter;
        private readonly IDisposable _eatPreySubscription;

        [Inject]
        public ViewService(IGameScopeFactory factory, ViewServiceConfig config)
        {
            _infoPresenter = factory.Get<IInfoPresenter>();

            _worldViewRuntime = new WorldViewRuntime(config);

            _eatPreySubscription = factory
                .Get<ISubscriber<EatPreyMessage>>()
                .Subscribe(OnEatPreyMessage);
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            _worldViewRuntime.Handle(message.Killer?.GetViewParent());
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