using Project.Entities;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(SpawnEntityServiceConfig), menuName = "Config/Service/Spawn Entity")]
    public class SpawnEntityServiceConfig : ScriptableObject
    {
        [field: SerializeField] public Entity EntityPrefab { get; private set; }
        [field: SerializeField] public float SpawnRange { get; private set; } = 2.0f;
        [field: SerializeField] public float MaxSpawnCount { get; private set; } = 20f;
    }
}