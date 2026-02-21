using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class JumpMovementComponent : IEntityFixedTickableComponent
    {
        private readonly JumpMovementModel _model;
        private IEntity _entity;
        private Rigidbody _rigidbody;

        public JumpMovementComponent(JumpMovementConfig config)
        {
            _model = new JumpMovementModel(config);
        }

        public void Initialize(IEntity entity)
        {
            _entity = entity;
            _rigidbody = entity.GetRigidbody();
            _model.Initialize(_rigidbody);
        }

        public void FixedTick()
        {
            var nowTime = Time.fixedTime;
            var moveDirection = _model.GetNormalizedMoveDirection(_entity.GetMoveDirection());

            if (_model.IsGroundedAllowed(nowTime) && _model.IsGrounded(_entity.GetPosition()))
            {
                HandleGrounded(nowTime, moveDirection);
                return;
            }

            HandleAirborne(moveDirection);
        }

        private void HandleGrounded(float nowTime, Vector3 moveDirection)
        {
            if (_model.CanJump(nowTime))
            {
                Jump(nowTime, moveDirection);
                return;
            }

            _model.ResetHorizontalVelocity();
        }

        private void Jump(float nowTime, Vector3 moveDirection)
        {
            _model.ScheduleNextJump(nowTime);
            _model.AddImpulse(moveDirection);
        }

        private void HandleAirborne(Vector3 moveDirection)
        {
            var currentHorizontal = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            var acceleration = _model.GetAirborneAcceleration(currentHorizontal, moveDirection);
            _rigidbody.AddForce(acceleration, ForceMode.Acceleration);
        }

        public void Dispose()
        {
        }
    }
}