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
        private int _nextId;

        [Inject]
        public EntityFactory(IGameScopeFactory gameScopeFactory)
        {
            _gameScopeFactory = gameScopeFactory;
        }

        public IEntity Create(EntityArchetypeConfig archetype)
        {
            var entity = Object.Instantiate(archetype.Prefab);
            entity.Initialize(archetype.Data, _nextId++);

            AttachMovementComponent(entity, archetype);
            AttachCollisionComponent(entity, archetype);

            return entity;
        }

        private void AttachMovementComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            if (archetype.Data.Kind == EntityKind.JumpAnimal)
            {
                entity.AddComponent(new JumpMovementComponent(archetype.JumpMovement));
                return;
            }

            entity.AddComponent(new LinearMovementComponent(archetype.LinearMovement));
        }

        private void AttachCollisionComponent(IEntity entity, EntityArchetypeConfig archetype)
        {
            if (archetype.Data.Kind == EntityKind.Hunter)
            {
                entity.AddComponent(new HunterCollisionComponent(
                    _gameScopeFactory.Get<IPublisher<EatPreyMessage>>()));
                return;
            }

            entity.AddComponent(new AnimalCollisionComponent());
        }
    }
}