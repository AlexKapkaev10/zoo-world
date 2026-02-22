using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.Services.CameraService;
using UnityEngine;
using VContainer;

namespace Project.Entities
{
    public sealed class EntityService : IEntityService
    {
        private readonly Dictionary<IEntity, SpawnArchetypeData> _entityMap = new();
        private readonly CancellationTokenSource _spawnCts = new();
        private readonly SpawnEntityModel _spawnModel;
        private readonly EntityServiceConfig _config;
        private readonly EntityPool _pool;
        private readonly ICameraService _cameraService;
        private readonly IDisposable _eatPreySubscription;

        [Inject]
        public EntityService(IGameScopeFactory factory, EntityServiceConfig config)
        {
            _config = config;
            _cameraService = factory.Get<ICameraService>();
            
            _pool = new EntityPool(factory.Get<IEntityFactory>());
            _spawnModel = new SpawnEntityModel(_config);

            _eatPreySubscription = factory
                .Get<ISubscriber<EatPreyMessage>>()
                .Subscribe(OnEatPreyMessage);
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            message.Killed.StartDeath();
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
                entity.Tick();
            }
        }

        public void FixedTick()
        {
            foreach (var entity in _entityMap.Keys)
            {
                entity.FixedTick();
            }
        }

        public void Dispose()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            _eatPreySubscription?.Dispose();

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
            entity.Spawn(spawnPosition, bodyRotation);

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