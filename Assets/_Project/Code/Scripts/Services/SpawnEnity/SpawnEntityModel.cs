using System.Collections.Generic;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Services.SpawnEntity
{
    public sealed class SpawnEntityModel
    {
        private readonly SpawnEntityServiceConfig _config;
        private readonly SpawnAreaModel _spawnArea;
        private readonly Dictionary<EntityKind, int> _aliveByKind = new();
        private readonly List<SpawnArchetypeData> _availableRules = new();

        public SpawnEntityModel(SpawnEntityServiceConfig config)
        {
            _config = config;
            _spawnArea = new SpawnAreaModel(_config.SpawnCenter, _config.SpawnRange, _config.SpawnGroundY);
        }

        public bool TryGetSpawnRequest(out SpawnArchetypeData spawnData, out Vector3 spawnPosition, out Quaternion bodyRotation)
        {
            _availableRules.Clear();
            foreach (SpawnArchetypeData data in _config.SpawnData)
            {
                int aliveForKind = _aliveByKind.GetValueOrDefault(data.Archetype.Kind, 0);
                if (aliveForKind >= data.MaxAliveCount)
                {
                    continue;
                }

                _availableRules.Add(data);
            }

            if (_availableRules.Count == 0)
            {
                spawnData = null;
                spawnPosition = default;
                bodyRotation = default;
                return false;
            }

            int randomIndex = Random.Range(0, _availableRules.Count);
            spawnData = _availableRules[randomIndex];
            spawnPosition = _spawnArea.GetRandomPosition();
            bodyRotation = _spawnArea.GetRandomBodyRotation();
            return true;
        }

        public void RegisterSpawn(EntityKind kind)
        {
            _aliveByKind[kind] = _aliveByKind.TryGetValue(kind, out int current) ? current + 1 : 1;
        }

        public void RegisterDespawn(EntityKind kind)
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
    }
}
