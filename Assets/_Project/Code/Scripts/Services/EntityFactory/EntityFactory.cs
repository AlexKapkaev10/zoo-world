using Project.Entities;
using Project.Entities.Components;
using Project.Entities.Components.Movement;
using Project.ScriptableObjects;
using Object = UnityEngine.Object;

namespace Project.Services
{
    public sealed class EntityFactory : IEntityFactory
    {
        private int counter = 0;
        
        public IEntity Create(EntityArchetypeConfig archetype)
        {
            IEntity entity = Object.Instantiate(archetype.Prefab);
            entity.SetId(counter);
            entity.SetKind(archetype.Kind);
            entity.SetViewportExitTurn(archetype.ViewportExitTurnBackAngle, archetype.ViewportExitTurnRandomDelta);
            AttachMovementComponent(entity, archetype);
            AttachCollisionComponent(entity, archetype);

            counter++;
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

        private static void AttachCollisionComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            if (archetype.Kind == EntityKind.Hunter)
            {
                entity.AddComponent(new HunterCollisionComponent());
                return;
            }

            entity.AddComponent(new AnimalBounceCollisionComponent());
        }
    }
}
