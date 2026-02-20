using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(LinearMovementConfig), menuName = "Config/Entity/Movement/Linear")]
    public sealed class LinearMovementConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float Speed { get; private set; } = 3f;
    }
}
