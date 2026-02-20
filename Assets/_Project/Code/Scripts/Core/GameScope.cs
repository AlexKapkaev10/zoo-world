using Project.ScriptableObjects;
using Project.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Core
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private SpawnEntityServiceConfig _spawnEntityServiceConfig;
        [SerializeField] private CameraServiceConfig _cameraServiceConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterServices(builder);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<CameraService>(Lifetime.Scoped)
                .As<ICameraService>()
                .As<IInitializable>()
                .As<ITickable>()
                .WithParameter(_cameraServiceConfig);
            
            builder.Register<SpawnEntityService>(Lifetime.Scoped)
                .As<ISpawnEntityService>()
                .As<IStartable>()
                .WithParameter(_spawnEntityServiceConfig);
        }
    }
}