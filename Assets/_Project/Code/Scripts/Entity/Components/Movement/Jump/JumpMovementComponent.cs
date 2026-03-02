using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class JumpMovementComponent : IEntityFixedTickableComponent, IEntityBounceApply
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
                TryJump(nowTime, moveDirection);
                return;
            }

            UpdateMove(moveDirection);
        }

        public void OnBounceApply()
        {
            _model.RegisterBounceImpulse(Time.fixedTime);
        }

        private void TryJump(float nowTime, Vector3 moveDirection)
        {
            if (_model.CanJump(nowTime))
            {
                Jump(nowTime, moveDirection);
                return;
            }

            if (_model.CanResetHorizontalVelocity(nowTime))
            {
                _model.ResetHorizontalVelocity();
            }
        }

        private void Jump(float nowTime, Vector3 moveDirection)
        {
            _model.ScheduleNextJump(nowTime);
            _model.AddImpulse(moveDirection);
        }

        private void UpdateMove(Vector3 moveDirection)
        {
            var currentHorizontal = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            var acceleration = _model.GetAirborneAcceleration(currentHorizontal, moveDirection);
            _rigidbody.AddForce(acceleration, ForceMode.Acceleration);
        }
    }
}