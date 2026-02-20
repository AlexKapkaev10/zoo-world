using System;
using System.Collections.Generic;
using Project.Entities.Components;
using UnityEngine;

namespace Project.Entities
{
    public interface IEntity
    {
        event Action<IEntity> Destroyed;
        Transform Transform { get; }
        Rigidbody Rigidbody { get; }
        void AddRuntimeComponent(IEntityRuntimeComponent component);
        void TickComponents();
        void FixedTickComponents();
        void CameraViewportExit();
    }
    
    public sealed class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] private Rigidbody _rigidbody;

        private readonly List<IEntityRuntimeComponent> _components = new();
        private readonly List<IEntityTickableComponent> _tickableComponents = new();
        private readonly List<IEntityFixedTickableComponent> _fixedTickableComponents = new();
        private readonly List<IEntityCollisionComponent> _collisionComponents = new();

        public event Action<IEntity> Destroyed;
        public Transform Transform => transform;
        public Rigidbody Rigidbody => _rigidbody;

        public void AddRuntimeComponent(IEntityRuntimeComponent component)
        {
            if (component == null)
            {
                return;
            }

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
            for (int i = 0; i < _tickableComponents.Count; i++)
            {
                _tickableComponents[i].Tick();
            }
        }

        public void FixedTickComponents()
        {
            for (int i = 0; i < _fixedTickableComponents.Count; i++)
            {
                _fixedTickableComponents[i].FixedTick();
            }
        }
        
        public void CameraViewportExit()
        {
            Debug.Log($"{gameObject.name} Switch way point");
        }

        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < _collisionComponents.Count; i++)
            {
                _collisionComponents[i].OnCollisionEnter(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            for (int i = 0; i < _collisionComponents.Count; i++)
            {
                _collisionComponents[i].OnCollisionStay(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            for (int i = 0; i < _collisionComponents.Count; i++)
            {
                _collisionComponents[i].OnCollisionExit(collision);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Dispose();
            }

            _components.Clear();
            _tickableComponents.Clear();
            _fixedTickableComponents.Clear();
            _collisionComponents.Clear();

            Destroyed?.Invoke(this);
        }
    }
}