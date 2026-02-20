using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class JumpMovementModel
    {
        private readonly JumpMovementConfig _config;
        
        private Rigidbody _rigidbody;
        private float _nextJumpTime;
        private float _groundCheckUnlockTime;

        public JumpMovementModel(JumpMovementConfig config)
        {
            _config = config;
        }

        public void Initialize(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
            _nextJumpTime = Time.fixedTime;
            _groundCheckUnlockTime = 0f;
        }
        
        public void AddImpulse(Vector3 moveDirection)
        {
            _rigidbody.AddForce(GetJumpImpulse(moveDirection), ForceMode.Impulse);
        }

        public bool IsGroundedAllowed(float nowTime)
        {
            return nowTime >= _groundCheckUnlockTime;
        }

        public bool CanJump(float nowTime)
        {
            return nowTime >= _nextJumpTime;
        }

        public void ScheduleNextJump(float nowTime)
        {
            _nextJumpTime = nowTime + GetRandomJumpInterval();
            _groundCheckUnlockTime = nowTime + _config.GroundCheckLock;
        }

        public void ResetHorizontalVelocity()
        {
            var velocity = _rigidbody.linearVelocity;
            velocity.x = 0f;
            velocity.z = 0f;

            _rigidbody.linearVelocity = velocity;
        }

        private Vector3 GetJumpImpulse(Vector3 moveDirection)
        {
            Vector3 jumpImpulse = moveDirection * _config.HorizontalSpeed;
            jumpImpulse.y = _config.JumpForce;
            return jumpImpulse;
        }

        public Vector3 GetAirborneAcceleration(Vector3 currentHorizontal, Vector3 moveDirection)
        {
            Vector3 targetHorizontal = moveDirection * _config.HorizontalSpeed;
            return targetHorizontal - currentHorizontal;
        }

        public Vector3 GetNormalizedMoveDirection(Vector3 moveDirection)
        {
            return moveDirection.sqrMagnitude > 0f ? moveDirection.normalized : Vector3.zero;
        }

        public bool IsGrounded(Vector3 position)
        {
            var origin = position + _config.GroundCheckOffset;

            return Physics.Raycast(
                origin,
                Vector3.down,
                _config.GroundCheckDistance,
                _config.GroundMask,
                QueryTriggerInteraction.Ignore
            );
        }

        private float GetRandomJumpInterval()
        {
            return Random.Range(_config.MinJumpInterval, _config.MaxJumpInterval);
        }
    }
}
