using System.Collections.Generic;
using UnityEngine;

namespace Project.Entities
{
    public sealed class EntityPhysicsComponent : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        private readonly List<Collider> _colliders = new();
        private bool _initialIsKinematic;

        public void InitializePhysics()
        {
            _initialIsKinematic = _rigidbody.isKinematic;
            GetComponentsInChildren(true, _colliders);
        }

        public void ApplyBounce(Vector3 direction, float forceValue)
        {
            _rigidbody.AddForce(direction * forceValue, ForceMode.Impulse);
        }

        public void FreezeForDeath()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            SetCollidersEnabled(false);
        }

        public void RestoreForSpawn()
        {
            _rigidbody.isKinematic = _initialIsKinematic;
            if (!_rigidbody.isKinematic)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            SetCollidersEnabled(true);
        }

        public Rigidbody GetRigidbody()
        {
            return _rigidbody;
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            foreach (var collider in _colliders)
            {
                collider.enabled = isEnabled;
            }
        }
    }
}
