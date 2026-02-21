using System.Collections.Generic;
using Project.Entities;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(EntityServiceConfig), menuName = "Config/Service/Entity")]
    public sealed class EntityServiceConfig : ScriptableObject
    {
        [field: SerializeField] public List<SpawnArchetypeData> SpawnData { get; private set; } = new();
        [field: SerializeField, Min(0.1f)] public float SpawnIntervalSeconds { get; private set; } = 2.0f;
        [field: SerializeField] public Vector3 SpawnCenter { get; private set; }
        [field: SerializeField] public Vector2 SpawnRange { get; private set; }
    }
}