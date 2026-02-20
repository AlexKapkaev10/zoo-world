using System.Collections.Generic;
using Project.Services.SpawnEntity;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(SpawnEntityServiceConfig), menuName = "Config/Service/Spawn Entity")]
    public sealed class SpawnEntityServiceConfig : ScriptableObject
    {
        [field: SerializeField] public List<SpawnArchetypeData> SpawnData { get; private set; } = new();
        [field: SerializeField, Min(0.1f)] public float SpawnIntervalSeconds { get; private set; } = 2.0f;
        [field: SerializeField] public Vector3 SpawnCenter { get; private set; } = Vector3.zero;
        [field: SerializeField] public Vector2 SpawnRange { get; private set; } = new Vector2(10f, 10f);
        [field: SerializeField] public float SpawnGroundY { get; private set; } = 0f;
    }
}