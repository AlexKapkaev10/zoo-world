using Project.Entities;
using Project.Entities.Components.Movement;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Services
{
    public sealed class EntityFactory : IEntityFactory
    {
        public IEntity Create(EntityArchetypeConfig archetype)
        {
            if (archetype == null || archetype.Prefab == null)
            {
                return null;
            }

            var entity = Object.Instantiate(archetype.Prefab);
            AttachMovementComponent(entity, archetype);
            return entity;
        }

        private static void AttachMovementComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            if (entity == null)
            {
                return;
            }

            switch (archetype.Kind)
            {
                case EntityKind.Frog:
                    entity.AddRuntimeComponent(new JumpMovementComponent(archetype.JumpMovement));
                    break;

                case EntityKind.Hunter:
                case EntityKind.Snake:
                default:
                    entity.AddRuntimeComponent(new LinearMovementComponent(archetype.LinearMovement));
                    break;
            }
        }
    }
}
