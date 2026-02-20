using System;
using System.Collections.Generic;
using Project.Entities.Components;
using UnityEngine;

namespace Project.Entities
{
    public sealed class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] private Transform _bodyTransform;

        private readonly List<IEntityRuntimeComponent> _components = new();
        private readonly List<IEntityTickableComponent> _tickableComponents = new();
        private readonly List<IEntityFixedTickableComponent> _fixedTickableComponents = new();
        private readonly List<IEntityCollisionComponent> _collisionComponents = new();

        private float _turnBackAngle = 180f;
        private float _turnRandomDelta = 20f;
        private int _id;

        public EntityKind Kind { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public Transform Transform => transform;

        public int GetId()
        {
            return _id;
        }

        public Vector3 GetVelocity()
        {
            return Rigidbody.linearVelocity;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetKind(EntityKind kind)
        {
            Kind = kind;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetBodyRotation(Quaternion rotation)
        {
            _bodyTransform.rotation = rotation;
        }

        public void SetViewportExitTurn(float turnBackAngle, float turnRandomDelta)
        {
            _turnBackAngle = turnBackAngle;
            _turnRandomDelta = turnRandomDelta;
        }

        public void AddComponent(IEntityRuntimeComponent component)
        {
            component.Initialize(this);
            _components.Add(component);

            switch (component)
            {
                case IEntityTickableComponent tickableComponent:
                    _tickableComponents.Add(tickableComponent);
                    break;
                case IEntityFixedTickableComponent fixedTickableComponent:
                    _fixedTickableComponents.Add(fixedTickableComponent);
                    break;
                case IEntityCollisionComponent collisionComponent:
                    _collisionComponents.Add(collisionComponent);
                    break;
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
            var currentYaw = _bodyTransform.eulerAngles.y;
            var randomDelta = UnityEngine.Random.Range(-_turnRandomDelta, _turnRandomDelta);
            var targetYaw = currentYaw + _turnBackAngle + randomDelta;

            _bodyTransform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
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