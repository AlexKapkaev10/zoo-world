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
        private readonly List<SpawnArchetypeData> _spawnDataBuffer = new();

        public SpawnEntityModel(EntityServiceConfig config)
        {
            _config = config;
            _spawnArea = new SpawnAreaModel(_config.SpawnCenter, _config.SpawnRange);
        }

        public SpawnEntityData TryGetSpawnRequest()
        {
            _spawnDataBuffer.Clear();

            if (!TryFillSpawnBuffer())
            {
                return null;
            }

            var archetypeData = _spawnDataBuffer[Random.Range(0, _spawnDataBuffer.Count)];
            return new SpawnEntityData
            {
                ArchetypeData = archetypeData,
                SpawnPosition = _spawnArea.GetRandomPosition(archetypeData.MinSpawnY, archetypeData.MaxSpawnY),
                BodyRotation = _spawnArea.GetRandomBodyRotation()
            };
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

        private bool TryFillSpawnBuffer()
        {
            foreach (var spawnData in _config.SpawnData)
            {
                if (!CanSpawn(spawnData))
                {
                    continue;
                }

                _spawnDataBuffer.Add(spawnData);
            }

            if (_spawnDataBuffer.Count == 0)
            {
                return false;
            }

            return true;
        }

        private bool CanSpawn(SpawnArchetypeData spawnData)
        {
            var kind = spawnData.Archetype.Data.Kind;

            var aliveForKind = _aliveCountByKind.GetValueOrDefault(kind, 0);
            return aliveForKind < spawnData.MaxAliveCount;
        }
    }
}