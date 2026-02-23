using System;

namespace Project.Entities
{
    public sealed class EntityLifecycle
    {
        private readonly PhysicsBehavior _physicsBehavior;
        private readonly AnimatorBehavior _animatorBehavior;
        private readonly Action _onDeath;

        private bool _isDying;

        public EntityLifecycle(PhysicsBehavior physicsBehavior, AnimatorBehavior animatorBehavior, Action onDeath)
        {
            _physicsBehavior = physicsBehavior;
            _animatorBehavior = animatorBehavior;
            _onDeath = onDeath;
        }

        public bool IsDying => _isDying;

        public void Reset()
        {
            _isDying = false;
            _physicsBehavior.Reset();
            _animatorBehavior.Reset();
        }

        public void StartDeath()
        {
            if (_isDying)
            {
                return;
            }

            _isDying = true;
            _physicsBehavior.Stop();
            _animatorBehavior.PlayDeath(_onDeath);
        }
    }
}
