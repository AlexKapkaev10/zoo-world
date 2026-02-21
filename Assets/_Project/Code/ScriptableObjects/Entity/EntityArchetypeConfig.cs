using Project.Entities;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(EntityArchetypeConfig), menuName = "Config/Entity/Archetype")]
    public sealed class EntityArchetypeConfig : ScriptableObject
    {
        [field: SerializeField] public Entity Prefab { get; private set; }
        [field: SerializeField] public LinearMovementConfig LinearMovement { get; private set; }
        [field: SerializeField] public JumpMovementConfig JumpMovement { get; private set; }
        [field: SerializeField] public ArchetypeData Data { get; private set; }
    }

    [System.Serializable]
    public struct ArchetypeData
    {
        public EntityKind Kind;
        public float TurnBackAngle;
        public float TurnRandomDelta;
        public float CollisionBounceValue;
    }
}
