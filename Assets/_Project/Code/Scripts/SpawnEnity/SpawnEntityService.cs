using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Entities;
using Project.ScriptableObjects;
using Project.Services.CameraService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Services.SpawnEntity
{
    public interface ISpawnEntityService : IStartable, ITickable, IFixedTickable, IDisposable
    {
        
    }
    
    public sealed class SpawnEntityService : ISpawnEntityService
    {
        private readonly Dictionary<IEntity, SpawnArchetypeData> _entityMap = new();
        private readonly CancellationTokenSource _spawnCts = new();
        private readonly SpawnEntityModel _spawnModel;
        private readonly SpawnEntityServiceConfig _config;
        private readonly EntityPool _pool;
        private readonly ICameraService _cameraService;

        [Inject]
        public SpawnEntityService(
            ICameraService cameraService, 
            IEntityFactory entityFactory, 
            SpawnEntityServiceConfig config)
        {
            _config = config;
            _cameraService = cameraService;
            
            _pool = new EntityPool(entityFactory);
            _spawnModel = new SpawnEntityModel(_config);
        }
        
        public void Start()
        {
            _pool.Prewarm(_config.SpawnData);
            SpawnAsync(_spawnCts.Token).Forget();
        }

        public void Tick()
        {
            foreach (var entity in _entityMap.Keys)
            {
                entity.TickComponents();
            }
        }

        public void FixedTick()
        {
            foreach (var entity in _entityMap.Keys)
            {
                entity.FixedTickComponents();
            }
        }

        public void Dispose()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();

            while (_entityMap.Count > 0)
            {
                using var enumerator = _entityMap.Keys.GetEnumerator();
                enumerator.MoveNext();
                DespawnEntity(enumerator.Current, releaseToPool: true);
            }
        }

        private void SpawnEntity(SpawnArchetypeData spawnData, Vector3 spawnPosition, Quaternion bodyRotation)
        {
            var entity = _pool.Get(spawnData.Archetype);
            entity.SetPosition(spawnPosition);
            entity.SetBodyRotation(bodyRotation);

            entity.Deactivated += OnEntityDeactivated;
            entity.Destroyed += OnEntityDestroy;
            
            _entityMap[entity] = spawnData;
            _spawnModel.RegisterSpawn(spawnData.Archetype.Data.Kind);
            _cameraService.AddViewportObserved(entity);
        }

        private void DespawnEntity(IEntity entity, bool releaseToPool)
        {
            if (!_entityMap.Remove(entity, out var spawnArchetypeData))
            {
                return;
            }

            _spawnModel.RegisterDespawn(spawnArchetypeData.Archetype.Data.Kind);

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

        private async UniTaskVoid SpawnAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_spawnModel.TryGetSpawnRequest(out var spawnData, out var spawnPosition, out var bodyRotation))
                {
                    SpawnEntity(spawnData, spawnPosition, bodyRotation);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnIntervalSeconds), cancellationToken: token);
            }
        }

    }
}