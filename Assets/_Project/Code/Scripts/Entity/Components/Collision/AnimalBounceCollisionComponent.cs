using UnityEngine;

namespace Project.Entities.Components
{
    public sealed class AnimalBounceCollisionComponent : IEntityCollisionComponent
    {
        private const float PushImpulse = 40f;

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
                return;
            }

            if (_entity.GetId() < otherEntity.GetId())
            {
                return;
            }
            
            var directionSelf = _entity.GetPosition() - otherEntity.GetPosition();
            directionSelf.y = 0f;
            directionSelf.Normalize();
            
            var directionOther = otherEntity.GetPosition() - _entity.GetPosition();
            directionOther.y = 0f;
            directionOther.Normalize();
            
            _entity.SetBodyRotation(Quaternion.LookRotation(directionSelf, Vector3.up));
            _entity.Rigidbody.AddForce(directionSelf * PushImpulse, ForceMode.Impulse);
            
            otherEntity.SetBodyRotation(Quaternion.LookRotation(directionOther, Vector3.up));
            otherEntity.Rigidbody.AddForce(directionOther * PushImpulse, ForceMode.Impulse);
        }

        public void Dispose()
        {
        }
    }
}
