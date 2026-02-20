using Project.Entities;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(EntityArchetypeConfig), menuName = "Config/Entity/Archetype")]
    public sealed class EntityArchetypeConfig : ScriptableObject
    {
        [field: SerializeField] public EntityKind Kind { get; private set; }
        [field: SerializeField] public Entity Prefab { get; private set; }
        [field: SerializeField] public LinearMovementConfig LinearMovement { get; private set; }
        [field: SerializeField] public JumpMovementConfig JumpMovement { get; private set; }
        [field: SerializeField] public float ViewportExitTurnBackAngle { get; private set; } = 180f;
        [field: SerializeField] public float ViewportExitTurnRandomDelta { get; private set; } = 20f;
    }
}
