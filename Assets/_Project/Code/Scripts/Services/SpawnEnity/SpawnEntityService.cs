using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Entities;
using Project.ScriptableObjects;
using VContainer;
using VContainer.Unity;

namespace Project.Services
{
    public interface ISpawnEntityService : IStartable, ITickable, IFixedTickable, IDisposable
    {
        
    }
    
    public sealed class SpawnEntityService : ISpawnEntityService
    {
        private readonly List<IEntity> _entities = new();
        private readonly ICameraService _cameraService;
        private readonly IEntityFactory _entityFactory;
        private readonly SpawnEntityServiceConfig _config;
        private readonly CancellationTokenSource _spawnCts = new();
        
        [Inject]
        public SpawnEntityService(ICameraService cameraService, IEntityFactory entityFactory, SpawnEntityServiceConfig config)
        {
            _config = config;
            _cameraService = cameraService;
            _entityFactory = entityFactory;
        }
        
        public void Start()
        {
            SpawnAsync(_spawnCts.Token).Forget();
        }

        public void Tick()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].TickComponents();
            }
        }

        public void FixedTick()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].FixedTickComponents();
            }
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

        private void SpawnEntity(EntityArchetypeConfig archetype)
        {
            IEntity entity = _entityFactory.Create(archetype);
            if (entity == null)
            {
                return;
            }

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

        private EntityArchetypeConfig GetEntityArchetype()
        {
            if (_config.Archetypes == null || _config.Archetypes.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, _config.Archetypes.Count);
            return _config.Archetypes[randomIndex];
        }
        
        private async UniTaskVoid SpawnAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_entities.Count < _config.MaxSpawnCount)
                {
                    SpawnEntity(GetEntityArchetype());
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnIntervalSeconds), cancellationToken: token);
            }
        }
    }
}