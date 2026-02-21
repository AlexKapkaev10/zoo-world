using UnityEngine;

namespace Project.Entities
{
    public sealed class EntityPhysicsComponent : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _rootCollider;

        private bool _initialIsKinematic;

        public void Initialize()
        {
            _initialIsKinematic = _rigidbody.isKinematic;
        }

        public void AddBounceImpulse(Vector3 direction, float forceValue)
        {
            _rigidbody.AddForce(direction * forceValue, ForceMode.Impulse);
        }

        public void PrepareForDeath()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            _rootCollider.enabled = false;
        }

        public void PrepareForSpawn()
        {
            _rigidbody.isKinematic = _initialIsKinematic;
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            _rootCollider.enabled = true;
        }

        public Rigidbody GetRigidbody()
        {
            return _rigidbody;
        }
    }
}
