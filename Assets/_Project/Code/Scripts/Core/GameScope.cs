using MessagePipe;
using Project.Entities;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.Services.CameraService;
using Project.UI;
using Project.UI.MVP;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Core
{
    public sealed class GameScope : LifetimeScope
    {
        [SerializeField] private EntityServiceConfig _entityServiceConfig;
        [SerializeField] private CameraServiceConfig _cameraServiceConfig;
        [SerializeField] private InfoPresenterConfig _infoPresenterConfig;
        [SerializeField] private ViewServiceConfig _viewServiceConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessagePipe(builder);

            RegisterServices(builder);
            
            RegisterMVP(builder);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntityService>()
                .As<IEntityService>()
                .WithParameter(_entityServiceConfig);
            
            builder.Register<EntityFactory>(Lifetime.Scoped)
                .As<IEntityFactory>();

            builder.Register<GameScopeFactory>(Lifetime.Scoped)
                .As<IGameScopeFactory>();

            builder.Register<CameraService>(Lifetime.Scoped)
                .As<ICameraService>()
                .As<IInitializable>()
                .As<ITickable>()
                .WithParameter(_cameraServiceConfig);

            builder.Register<ViewService>(Lifetime.Scoped)
                .As<IViewService>()
                .As<IInitializable>()
                .As<ITickable>()
                .WithParameter(_viewServiceConfig);
        }

        private void RegisterMVP(IContainerBuilder builder)
        {
            builder.Register<InfoModel>(Lifetime.Scoped)
                .As<IInfoModel>();
            
            builder.Register<InfoPresenter>(Lifetime.Scoped)
                .As<IInfoPresenter>()
                .WithParameter(_infoPresenterConfig);
        }

        private void RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<EatPreyMessage>(options);
        }
    }
}