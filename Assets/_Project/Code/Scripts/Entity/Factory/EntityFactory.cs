using MessagePipe;
using Project.Entities.Components;
using Project.Entities.Components.Movement;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using VContainer;
using Object = UnityEngine.Object;

namespace Project.Entities
{
    public sealed class EntityFactory : IEntityFactory
    {
        private readonly IGameScopeFactory _gameScopeFactory;
        private int counter;

        [Inject]
        public EntityFactory(IGameScopeFactory gameScopeFactory)
        {
            _gameScopeFactory = gameScopeFactory;
        }
        
        public IEntity Create(EntityArchetypeConfig archetype)
        {
            IEntity entity = Object.Instantiate(archetype.Prefab);
            entity.Initialize(
                _gameScopeFactory.Get<IPublisher<EatPreyMessage>>(), 
                archetype.Data, 
                counter++);
            
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
