using Project.Entities;
using Project.Entities.Components.Movement;
using Project.ScriptableObjects;
using Object = UnityEngine.Object;

namespace Project.Services
{
    public sealed class EntityFactory : IEntityFactory
    {
        public IEntity Create(EntityArchetypeConfig archetype)
        {
            var entity = Object.Instantiate(archetype.Prefab);
            entity.SetViewportExitTurn(archetype.ViewportExitTurnBackAngle, archetype.ViewportExitTurnRandomDelta);
            AttachMovementComponent(entity, archetype);
            return entity;
        }

        private static void AttachMovementComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            switch (archetype.Kind)
            {
                case EntityKind.Frog:
                    entity.AddComponent(new JumpMovementComponent(archetype.JumpMovement));
                    break;

                case EntityKind.Hunter:
                case EntityKind.Snake:
                default:
                    entity.AddComponent(new LinearMovementComponent(archetype.LinearMovement));
                    break;
            }
        }
    }
}
