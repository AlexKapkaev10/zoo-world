using System.Collections.Generic;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(SpawnEntityServiceConfig), menuName = "Config/Service/Spawn Entity")]
    public sealed class SpawnEntityServiceConfig : ScriptableObject
    {
        [field: SerializeField] public List<EntityArchetypeConfig> Archetypes { get; private set; } = new();
        [field: SerializeField, Min(0.1f)] public float SpawnIntervalSeconds { get; private set; } = 2.0f;
        [field: SerializeField, Min(1)] public int MaxSpawnCount { get; private set; } = 20;
    }
}