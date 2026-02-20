using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Entities;
using Project.ScriptableObjects;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Project.Services
{
    public interface ISpawnEntityService : IStartable, IDisposable
    {
        
    }
    
    public sealed class SpawnEntityService : ISpawnEntityService
    {
        private readonly List<IEntity> _entities = new ();
        private readonly ICameraService _cameraService;
        private readonly SpawnEntityServiceConfig _config;
        private readonly CancellationTokenSource _spawnCts = new ();
        
        [Inject]
        public SpawnEntityService(ICameraService cameraService, SpawnEntityServiceConfig config)
        {
            _config = config;
            _cameraService = cameraService;
        }
        
        public void Start()
        {
            SpawnAsync(_spawnCts.Token).Forget();
        }

        public void Dispose()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            
            foreach (var entity in _entities)
            {
                entity.Destroyed -= OnEntityDestroy;
            }
        }

        private void SpawnEntity(Entity prefab)
        {
            var entity = Object.Instantiate(prefab);
            entity.Destroyed += OnEntityDestroy;
            
            _entities.Add(entity);
            _cameraService.AddViewportObserved(entity);
        }

        private void OnEntityDestroy(IEntity entity)
        {
            entity.Destroyed -= OnEntityDestroy;
            
            _cameraService.RemoveViewportObserved(entity);
            _entities.Remove(entity);
        }

        private Entity GetEntity()
        {
            return _config.EntityPrefab;
        }
        
        private async UniTaskVoid SpawnAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SpawnEntity(GetEntity());
                await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnRange), cancellationToken: token);
            }
        }
    }
}