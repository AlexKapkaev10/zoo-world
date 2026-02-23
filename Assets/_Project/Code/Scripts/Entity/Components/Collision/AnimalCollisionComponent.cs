using UnityEngine;

namespace Project.Entities.Components
{
    public sealed class AnimalCollisionComponent : IEntityCollisionComponent
    {
        private IEntity _self;
        
        public void Initialize(IEntity entity)
        {
            _self = entity;
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
            
            _self.SetBounce(_self.GetPosition() - otherEntity.GetPosition());
        }
    }
}