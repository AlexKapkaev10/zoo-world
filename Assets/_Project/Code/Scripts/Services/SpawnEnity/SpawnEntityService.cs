using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Entities;
using Project.ScriptableObjects;
using Project.Services.CameraService;
using VContainer;
using VContainer.Unity;

namespace Project.Services.SpawnEntity
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
        private readonly Dictionary<IEntity, SpawnArchetypeData> _ruleByEntity = new();
        private readonly Dictionary<EntityKind, int> _aliveByKind = new();

        private EntityPool _pool;
        
        [Inject]
        public SpawnEntityService(ICameraService cameraService, IEntityFactory entityFactory, SpawnEntityServiceConfig config)
        {
            _config = config;
            _cameraService = cameraService;
            _entityFactory = entityFactory;
        }
        
        public void Start()
        {
            _pool = new EntityPool(_entityFactory);
            _pool.Prewarm(_config.SpawnData);
            SpawnAsync(_spawnCts.Token).Forget();
        }

        public void Tick()
        {
            foreach (var entity in _entities)
            {
                entity.TickComponents();
            }
        }

        public void FixedTick()
        {
            foreach (var entity in _entities)
            {
                entity.FixedTickComponents();
            }
        }

        public void Dispose()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();

            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                DespawnEntity(_entities[i], releaseToPool: true);
            }
        }

        private void SpawnEntity(SpawnArchetypeData spawnData)
        {
            var entity = _pool.Get(spawnData.Archetype);

            entity.Deactivated += OnEntityDeactivated;
            entity.Destroyed += OnEntityDestroy;
            
            _entities.Add(entity);
            _ruleByEntity[entity] = spawnData;
            IncrementAlive(spawnData.Archetype.Kind);
            _cameraService.AddViewportObserved(entity);
        }

        private void DespawnEntity(IEntity entity, bool releaseToPool)
        {
            if (!_ruleByEntity.Remove(entity, out var spawnArchetypeData))
            {
                return;
            }

            _entities.Remove(entity);
            DecrementAlive(spawnArchetypeData.Archetype.Kind);

            entity.Deactivated -= OnEntityDeactivated;
            entity.Destroyed -= OnEntityDestroy;
            _cameraService.RemoveViewportObserved(entity);

            if (releaseToPool && _pool != null)
            {
                _pool.Release(entity);
            }
        }

        private void OnEntityDestroy(IEntity entity)
        {
            DespawnEntity(entity, releaseToPool: false);
        }

        private void OnEntityDeactivated(IEntity entity)
        {
            DespawnEntity(entity, releaseToPool: true);
        }

        private SpawnArchetypeData GetSpawnRule()
        {
            List<SpawnArchetypeData> availableRules = new();
            
            foreach (var data in _config.SpawnData)
            {
                int aliveForKind = _aliveByKind.GetValueOrDefault(data.Archetype.Kind, 0);
                if (aliveForKind >= data.MaxAliveCount)
                {
                    continue;
                }

                availableRules.Add(data);
            }

            if (availableRules.Count == 0)
            {
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, availableRules.Count);
            return availableRules[randomIndex];
        }

        private void IncrementAlive(EntityKind kind)
        {
            _aliveByKind[kind] = _aliveByKind.TryGetValue(kind, out int current) ? current + 1 : 1;
        }

        private void DecrementAlive(EntityKind kind)
        {
            if (!_aliveByKind.TryGetValue(kind, out int current))
            {
                return;
            }

            current--;
            if (current <= 0)
            {
                _aliveByKind.Remove(kind);
                return;
            }

            _aliveByKind[kind] = current;
        }
        
        private async UniTaskVoid SpawnAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SpawnArchetypeData spawnData = GetSpawnRule();
                if (spawnData != null)
                {
                    SpawnEntity(spawnData);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnIntervalSeconds), cancellationToken: token);
            }
        }
    }
}