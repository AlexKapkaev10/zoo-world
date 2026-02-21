using MessagePipe;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.Services;
using Project.Services.CameraService;
using Project.Services.SpawnEntity;
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
            builder.Register<EntityFactory>(Lifetime.Scoped)
                .As<IEntityFactory>();
            
            builder.Register<GameScopeFactory>(Lifetime.Scoped)
                .As<IGameScopeFactory>();

            builder.Register<CameraService>(Lifetime.Scoped)
                .As<ICameraService>()
                .As<IInitializable>()
                .As<ITickable>()
                .WithParameter(_cameraServiceConfig);
            
            builder.Register<SpawnEntityService>(Lifetime.Scoped)
                .As<ISpawnEntityService>()
                .As<IStartable>()
                .As<ITickable>()
                .As<IFixedTickable>()
                .WithParameter(_spawnEntityServiceConfig);
        }

        private void RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            
            builder.RegisterMessageBroker<EatPreyMessage>(options);
        }
    }
}