using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class JumpMovementComponent : IEntityFixedTickableComponent
    {
        private readonly JumpMovementConfig _config;
        private IEntity _entity;
        private Vector3 _direction;
        private float _nextJumpAt;

        public JumpMovementComponent(JumpMovementConfig config)
        {
            _config = config;
        }

        public void Initialize(IEntity entity)
        {
            _entity = entity;
            _direction = _config != null ? _config.Direction.normalized : Vector3.forward;
            _nextJumpAt = Time.time;
        }

        public void FixedTick()
        {
            if (_entity?.Rigidbody == null)
            {
                return;
            }

            float speed = _config != null ? _config.HorizontalSpeed : 0f;
            Vector3 velocity = _direction * speed;
            velocity.y = _entity.Rigidbody.linearVelocity.y;
            _entity.Rigidbody.linearVelocity = velocity;

            if (Time.time < _nextJumpAt)
            {
                return;
            }

            _nextJumpAt = Time.time + (_config != null ? _config.JumpInterval : 1f);
            _entity.Rigidbody.AddForce(Vector3.up * (_config != null ? _config.JumpForce : 0f), ForceMode.Impulse);
        }

        public void Dispose()
        {
        }
    }
}
