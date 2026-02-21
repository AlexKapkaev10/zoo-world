using MessagePipe;
using Project.Entities;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.Services.CameraService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Core
{
    public sealed class GameScope : LifetimeScope
    {
        [SerializeField] private SpawnEntityServiceConfig _spawnEntityServiceConfig;
        [SerializeField] private CameraServiceConfig _cameraServiceConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessagePipe(builder);

            RegisterServices(builder);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntityService>()
                .As<IEntityService>()
                .WithParameter(_spawnEntityServiceConfig);
            
            builder.Register<EntityFactory>(Lifetime.Scoped)
                .As<IEntityFactory>();

            builder.Register<GameScopeFactory>(Lifetime.Scoped)
                .As<IGameScopeFactory>();

            builder.Register<CameraService>(Lifetime.Scoped)
                .As<ICameraService>()
                .As<IInitializable>()
                .As<ITickable>()
                .WithParameter(_cameraServiceConfig);
        }

        private void RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<EatPreyMessage>(options);
        }
    }
}