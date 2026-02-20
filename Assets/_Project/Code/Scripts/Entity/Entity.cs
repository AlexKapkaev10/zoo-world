using System;
using System.Collections.Generic;
using Project.Entities.Components;
using UnityEngine;

namespace Project.Entities
{
    public interface IEntity
    {
        event Action<IEntity> Deactivated;
        event Action<IEntity> Destroyed;
        Rigidbody Rigidbody { get; }
        void SetVisible(bool isVisible);
        void AddComponent(IEntityRuntimeComponent component);
        void TickComponents();
        void FixedTickComponents();
        void CameraViewportExit();
        Vector3 GetVelocity();
        Vector3 GetPosition();
        Vector3 GetMoveDirection();
    }
    
    public sealed class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _bodyTransform;

        private readonly List<IEntityRuntimeComponent> _components = new();
        private readonly List<IEntityTickableComponent> _tickableComponents = new();
        private readonly List<IEntityFixedTickableComponent> _fixedTickableComponents = new();
        private readonly List<IEntityCollisionComponent> _collisionComponents = new();

        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public Transform Transform => transform;
        public Rigidbody Rigidbody => _rigidbody;

        public Vector3 GetVelocity()
        {
            return _rigidbody.linearVelocity;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void AddComponent(IEntityRuntimeComponent component)
        {
            component.Initialize(this);
            _components.Add(component);

            if (component is IEntityTickableComponent tickableComponent)
            {
                _tickableComponents.Add(tickableComponent);
            }

            if (component is IEntityFixedTickableComponent fixedTickableComponent)
            {
                _fixedTickableComponents.Add(fixedTickableComponent);
            }

            if (component is IEntityCollisionComponent collisionComponent)
            {
                _collisionComponents.Add(collisionComponent);
            }
        }

        public void TickComponents()
        {
            foreach (var tickableComponent in _tickableComponents)
            {
                tickableComponent.Tick();
            }
        }

        public void FixedTickComponents()
        {
            foreach (var fixedTickableComponent in _fixedTickableComponents)
            {
                fixedTickableComponent.FixedTick();
            }
        }

        public void CameraViewportExit()
        {
            Debug.Log($"{gameObject.name} Switch way point");
        }

        public Vector3 GetMoveDirection()
        {
            return _bodyTransform.forward;
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var collisionComponent in _collisionComponents)
            {
                collisionComponent.OnCollisionEnter(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach (var collisionComponent in _collisionComponents)
            {
                collisionComponent.OnCollisionStay(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            foreach (var collisionComponent in _collisionComponents)
            {
                collisionComponent.OnCollisionExit(collision);
            }
        }

        private void OnDestroy()
        {
            foreach (var component in _components)
            {
                component.Dispose();
            }

            _components.Clear();
            _tickableComponents.Clear();
            _fixedTickableComponents.Clear();
            _collisionComponents.Clear();

            Destroyed?.Invoke(this);
        }

        private void OnDisable()
        {
            Deactivated?.Invoke(this);
        }
    }
}