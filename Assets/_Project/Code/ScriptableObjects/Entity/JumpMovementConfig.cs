using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(JumpMovementConfig), menuName = "Config/Entity/Movement/Jump")]
    public sealed class JumpMovementConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float HorizontalSpeed { get; private set; } = 2f;
        [field: SerializeField, Min(0f)] public float JumpForce { get; private set; } = 5f;
        [field: SerializeField, Min(0.05f)] public float JumpInterval { get; private set; } = 1f;
        [field: SerializeField] public Vector3 Direction { get; private set; } = Vector3.forward;
    }
}
