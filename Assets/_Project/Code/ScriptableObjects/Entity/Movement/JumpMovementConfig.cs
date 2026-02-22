using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(JumpMovementConfig), menuName = "Config/Entity/Component/Movement/Jump")]
    public sealed class JumpMovementConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float HorizontalSpeed { get; private set; } = 2f;
        [field: SerializeField, Min(0f)] public float JumpForce { get; private set; } = 5f;
        [field: SerializeField, Min(0.05f)] public float MinJumpInterval { get; private set; } = 1f;
        [field: SerializeField, Min(0.05f)] public float MaxJumpInterval { get; private set; } = 2f;
        [field: SerializeField, Min(0.05f)] public float GroundCheckLock { get; private set; } = 0.2f;
        [field: SerializeField, Min(0f)] public float ResetVelocityCooldown { get; private set; } = 0.2f;
        [field: SerializeField] public LayerMask GroundMask { get; private set; } = ~0;
        [field: SerializeField, Min(0.01f)] public float GroundCheckDistance { get; private set; } = 0.2f;
        [field: SerializeField] public Vector3 GroundCheckOffset { get; private set; } = new (0f, 0.05f, 0f);
    }
}
