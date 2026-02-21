using Project.Entities;
using Project.Entities.Components;
using Project.Entities.Components.Movement;
using Project.ScriptableObjects;
using Object = UnityEngine.Object;

namespace Project.Services
{
    public sealed class EntityFactory : IEntityFactory
    {
        private int counter;
        
        public IEntity Create(EntityArchetypeConfig archetype)
        {
            IEntity entity = Object.Instantiate(archetype.Prefab);
            entity.Initialize(archetype.Data, counter++);
            
            AttachMovementComponent(entity, archetype);
            AttachCollisionComponent(entity, archetype);
            
            return entity;
        }

        private static void AttachMovementComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            switch (archetype.Data.Kind)
            {
                case EntityKind.Frog:
                    entity.AddComponent(new JumpMovementComponent(archetype.JumpMovement));
                    break;
                case EntityKind.Hunter:
                    entity.AddComponent(new LinearMovementComponent(archetype.LinearMovement));
                    break;
                case EntityKind.Snake:
                    entity.AddComponent(new LinearMovementComponent(archetype.LinearMovement));
                    break;
            }
        }

        private static void AttachCollisionComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            if (archetype.Data.Kind == EntityKind.Hunter)
            {
                entity.AddComponent(new HunterCollisionComponent());
                return;
            }

            entity.AddComponent(new AnimalBounceCollisionComponent());
        }
    }
}
