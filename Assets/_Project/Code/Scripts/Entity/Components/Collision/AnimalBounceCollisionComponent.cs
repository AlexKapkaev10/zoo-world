using UnityEngine;

namespace Project.Entities.Components
{
    public sealed class AnimalBounceCollisionComponent : IEntityCollisionComponent
    {
        private IEntity _entity;

        public void Initialize(IEntity entity)
        {
            _entity = entity;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.TryGetComponent(out IEntity otherEntity))
            {
                return;
            }

            if (otherEntity.Data.Kind == EntityKind.Hunter)
            {
                return;
            }
            
            var direction = _entity.GetPosition() - otherEntity.GetPosition();
            direction.y = 0f;
            _entity.SetBounce(direction.normalized);
        }

        public void Dispose()
        {
        }
    }
}
