using Project.Entities;
using UnityEngine;

namespace Project.ScriptableObjects
{
    public enum EntityKind
    {
        Hunter = 0,
        Frog = 1,
        Snake = 2
    }

    [CreateAssetMenu(fileName = nameof(EntityArchetypeConfig), menuName = "Config/Entity/Archetype")]
    public sealed class EntityArchetypeConfig : ScriptableObject
    {
        [field: SerializeField] public EntityKind Kind { get; private set; }
        [field: SerializeField] public Entity Prefab { get; private set; }
        [field: SerializeField] public LinearMovementConfig LinearMovement { get; private set; }
        [field: SerializeField] public JumpMovementConfig JumpMovement { get; private set; }
    }
}
