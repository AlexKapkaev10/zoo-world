using System;
using System.Collections.Generic;
using Project.Services.CameraService;
using UnityEngine;

namespace Project.Entities
{
    public sealed class EntityRuntimeController : IDisposable
    {
        private readonly Dictionary<IEntity, SpawnArchetypeData> _spawnDataByEntity = new();
        private readonly List<PendingDespawnData> _pendingDespawns = new();

        private readonly EntityPool _pool;
        private readonly SpawnEntityModel _spawnModel;
        private readonly ICameraService _cameraService;

        private bool _isTicking;

        public EntityRuntimeController(EntityPool pool, SpawnEntityModel spawnModel, ICameraService cameraService)
        {
            _pool = pool;
            _spawnModel = spawnModel;
            _cameraService = cameraService;
        }

        public void TrySpawnOne()
        {
            if (!_spawnModel.TryGetSpawnRequest(
                    out var spawnData,
                    out var spawnPosition,
                    out var bodyRotation))
            {
                return;
            }

            SpawnInternal(spawnData, spawnPosition, bodyRotation);
        }

        private void SpawnInternal(SpawnArchetypeData spawnData, Vector3 spawnPosition, Quaternion bodyRotation)
        {
            var entity = _pool.Get(spawnData.Archetype);
            entity.Spawn(spawnPosition, bodyRotation);

            entity.Deactivated += OnEntityDeactivated;
            entity.Destroyed += OnEntityDestroyed;

            _spawnDataByEntity[entity] = spawnData;
            _spawnModel.RegisterSpawn(spawnData.Archetype.Data.Kind);
            _cameraService.AddViewportObserved(entity);
        }

        public void Tick()
        {
            _isTicking = true;

            foreach (var entity in _spawnDataByEntity.Keys)
            {
                entity.Tick();
            }

            _isTicking = false;
            ProcessPendingDespawns();
        }

        public void FixedTick()
        {
            _isTicking = true;

            foreach (var entity in _spawnDataByEntity.Keys)
            {
                entity.FixedTick();
            }

            _isTicking = false;
            ProcessPendingDespawns();
        }

        public void Dispose()
        {
            while (_spawnDataByEntity.Count > 0)
            {
                using var enumerator = _spawnDataByEntity.Keys.GetEnumerator();
                enumerator.MoveNext();
                DespawnEntity(enumerator.Current, returnToPool: true);
            }

            _pendingDespawns.Clear();
        }

        private void OnEntityDestroyed(IEntity entity)
        {
            DespawnOrDefer(entity, returnToPool: false);
        }

        private void OnEntityDeactivated(IEntity entity)
        {
            DespawnOrDefer(entity, returnToPool: true);
        }

        private void DespawnOrDefer(IEntity entity, bool returnToPool)
        {
            if (!_isTicking)
            {
                DespawnEntity(entity, returnToPool);
                return;
            }

            _pendingDespawns.Add(new PendingDespawnData(entity, returnToPool));
        }

        private void ProcessPendingDespawns()
        {
            if (_pendingDespawns.Count == 0)
            {
                return;
            }

            foreach (var pending in _pendingDespawns)
            {
                DespawnEntity(pending.Entity, pending.ReturnToPool);
            }

            _pendingDespawns.Clear();
        }

        private void DespawnEntity(IEntity entity, bool returnToPool)
        {
            if (!_spawnDataByEntity.Remove(entity, out var spawnData))
            {
                return;
            }

            _spawnModel.RegisterDespawn(spawnData.Archetype.Data.Kind);

            entity.Deactivated -= OnEntityDeactivated;
            entity.Destroyed -= OnEntityDestroyed;

            _cameraService.RemoveViewportObserved(entity);

            if (returnToPool)
            {
                _pool.Release(entity);
            }
        }
    }
}