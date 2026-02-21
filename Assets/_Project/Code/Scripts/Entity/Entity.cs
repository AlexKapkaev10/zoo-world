using System;
using System.Collections.Generic;
using MessagePipe;
using Project.Entities.Components;
using Project.Messages;
using Project.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Entities
{
    public sealed class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Rigidbody _rigidbody;
        
        private IPublisher<EatPreyMessage> _eatPreyPublisher;

        #region Components
        
        private readonly List<IEntityRuntimeComponent> _components = new();
        private readonly List<IEntityTickableComponent> _tickableComponents = new();
        private readonly List<IEntityFixedTickableComponent> _fixedTickableComponents = new();
        private readonly List<IEntityCollisionComponent> _collisionComponents = new();
        
        #endregion

        #region Properties

        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public ArchetypeData Data { get; private set; }
        public int ID { get; private set; }

        #endregion

        #region UnityEvents

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var collisionComponent in _collisionComponents)
            {
                collisionComponent.OnCollisionEnter(collision);
            }
        }

        private void OnDisable()
        {
            Deactivated?.Invoke(this);
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

        #endregion
        
        public void Initialize(IPublisher<EatPreyMessage> eatPreyPublisher, ArchetypeData data, int id)
        {
            _eatPreyPublisher = eatPreyPublisher;
            Data = data;
            ID = id;
        }

        public void SetBounce(Vector3 direction)
        {
            SetBodyRotation(Quaternion.LookRotation(direction, Vector3.up));
            _rigidbody.AddForce(direction * Data.CollisionBounceValue, ForceMode.Impulse);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetBodyRotation(Quaternion rotation)
        {
            _bodyTransform.rotation = rotation;
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

        public void EatPrey(IEntity killed)
        {
            _eatPreyPublisher?.Publish(new EatPreyMessage(this, killed));
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
            var targetYaw = 
                _bodyTransform.eulerAngles.y 
                + Data.TurnBackAngle 
                + Random.Range(-Data.TurnRandomDelta, Data.TurnRandomDelta);
            
            SetBodyRotation(Quaternion.Euler(0f, targetYaw, 0f));
        }

        public void Dead()
        {
            SetVisible(false);
        }

        public Rigidbody GetRigidbody()
        {
            return _rigidbody;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Vector3 GetMoveDirection()
        {
            return _bodyTransform.forward;
        }
    }
}