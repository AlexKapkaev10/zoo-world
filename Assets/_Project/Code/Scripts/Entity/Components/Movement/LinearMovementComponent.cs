using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class LinearMovementComponent : IEntityFixedTickableComponent
    {
        private readonly LinearMovementConfig _config;
        private IEntity _entity;
        private Vector3 _direction;

        public LinearMovementComponent(LinearMovementConfig config)
        {
            _config = config;
        }

        public void Initialize(IEntity entity)
        {
            _entity = entity;
            _direction = _config != null ? _config.Direction.normalized : Vector3.forward;
        }

        public void FixedTick()
        {
            if (_entity?.Rigidbody == null)
            {
                return;
            }

            float speed = _config != null ? _config.Speed : 0f;
            Vector3 velocity = _direction * speed;
            velocity.y = _entity.Rigidbody.linearVelocity.y;
            _entity.Rigidbody.linearVelocity = velocity;
        }

        public void Dispose()
        {
        }
    }
}
