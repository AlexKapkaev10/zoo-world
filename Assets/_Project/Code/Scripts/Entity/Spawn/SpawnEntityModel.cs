using System.Collections.Generic;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class SpawnEntityModel
    {
        private readonly EntityServiceConfig _config;
        private readonly SpawnAreaModel _spawnArea;

        private readonly Dictionary<EntityKind, int> _aliveCountByKind = new();
        private readonly List<SpawnArchetypeData> _eligibleSpawnDataBuffer = new();

        public SpawnEntityModel(EntityServiceConfig config)
        {
            _config = config;
            _spawnArea = new SpawnAreaModel(_config.SpawnCenter, _config.SpawnRange);
        }

        public bool TryGetSpawnRequest(out SpawnArchetypeData spawnData,
            out Vector3 spawnPosition,
            out Quaternion bodyRotation)
        {
            _eligibleSpawnDataBuffer.Clear();

            foreach (var candidate in _config.SpawnData)
            {
                if (!CanSpawn(candidate))
                {
                    continue;
                }

                _eligibleSpawnDataBuffer.Add(candidate);
            }

            if (_eligibleSpawnDataBuffer.Count == 0)
            {
                spawnData = null;
                spawnPosition = default;
                bodyRotation = default;
                return false;
            }

            spawnData = _eligibleSpawnDataBuffer[Random.Range(0, _eligibleSpawnDataBuffer.Count)];
            spawnPosition = _spawnArea.GetRandomPosition(spawnData.MinSpawnY, spawnData.MaxSpawnY);
            bodyRotation = _spawnArea.GetRandomBodyRotation();
            return true;
        }

        public void RegisterSpawn(EntityKind kind)
        {
            if (_aliveCountByKind.TryGetValue(kind, out var current))
            {
                _aliveCountByKind[kind] = current + 1;
                return;
            }

            _aliveCountByKind[kind] = 1;
        }

        public void RegisterDespawn(EntityKind kind)
        {
            if (!_aliveCountByKind.TryGetValue(kind, out var current))
            {
                return;
            }

            current--;
            if (current <= 0)
            {
                _aliveCountByKind.Remove(kind);
                return;
            }

            _aliveCountByKind[kind] = current;
        }

        private bool CanSpawn(SpawnArchetypeData spawnData)
        {
            var kind = spawnData.Archetype.Data.Kind;

            var aliveForKind = _aliveCountByKind.GetValueOrDefault(kind, 0);
            return aliveForKind < spawnData.MaxAliveCount;
        }
    }
}