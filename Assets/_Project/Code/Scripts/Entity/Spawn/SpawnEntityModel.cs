using System.Collections.Generic;
using Project.Entities;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class SpawnEntityModel
    {
        private readonly EntityServiceConfig _config;
        private readonly SpawnAreaModel _spawnArea;
        private readonly Dictionary<EntityKind, int> _kindAliveMap = new();
        private readonly List<SpawnArchetypeData> _availableDates = new();

        public SpawnEntityModel(EntityServiceConfig config)
        {
            _config = config;
            _spawnArea = new SpawnAreaModel(_config.SpawnCenter, _config.SpawnRange);
        }

        public bool TryGetSpawnRequest(out SpawnArchetypeData spawnData, out Vector3 spawnPosition, out Quaternion bodyRotation)
        {
            _availableDates.Clear();
            
            foreach (SpawnArchetypeData data in _config.SpawnData)
            {
                int aliveForKind = _kindAliveMap.GetValueOrDefault(data.Archetype.Data.Kind, 0);
                
                if (aliveForKind >= data.MaxAliveCount)
                {
                    continue;
                }

                _availableDates.Add(data);
            }

            if (_availableDates.Count == 0)
            {
                spawnData = null;
                spawnPosition = default;
                bodyRotation = default;
                return false;
            }
            
            spawnData = _availableDates[Random.Range(0, _availableDates.Count)];
            spawnPosition = _spawnArea.GetRandomPosition(spawnData.MinSpawnY, spawnData.MaxSpawnY);
            bodyRotation = _spawnArea.GetRandomBodyRotation();
            return true;
        }

        public void RegisterSpawn(EntityKind kind)
        {
            _kindAliveMap[kind] = _kindAliveMap.TryGetValue(kind, out int current) ? current + 1 : 1;
        }

        public void RegisterDespawn(EntityKind kind)
        {
            if (!_kindAliveMap.TryGetValue(kind, out int current))
            {
                return;
            }

            current--;
            if (current <= 0)
            {
                _kindAliveMap.Remove(kind);
                return;
            }

            _kindAliveMap[kind] = current;
        }
    }
}
