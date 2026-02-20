using UnityEngine;

namespace Project.Entities.Components
{
    public sealed class HunterCollisionComponent : IEntityCollisionComponent
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

            if (otherEntity.Kind == EntityKind.Hunter)
            {
                if (_entity.GetId() > otherEntity.GetId())
                {
                    otherEntity.SetVisible(false);
                }
                else
                {
                    _entity.SetVisible(false);
                }
                
                return;
            }

            otherEntity.SetVisible(false);
        }

        public void Dispose()
        {
        }
    }
}
