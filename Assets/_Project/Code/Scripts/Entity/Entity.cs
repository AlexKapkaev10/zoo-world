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
        [SerializeField] private Transform _worldViewParent;
        [SerializeField] private ScaleAnimatorComponent _animatorComponent;
        [SerializeField] private EntityPhysicsComponent _physicsComponent;
        
        private IPublisher<EatPreyMessage> _eatPreyPublisher;
        private bool _isDying;
        
        #region RuntimeComponents

        private readonly List<IEntityRuntimeComponent> _components = new();
        private readonly List<IEntityTickableComponent> _tickableComponents = new();
        private readonly List<IEntityFixedTickableComponent> _fixedTickableComponents = new();
        private readonly List<IEntityCollisionComponent> _collisionComponents = new();
        private readonly List<IEntityBounceAwareComponent> _bounceAwareComponents = new();

        #endregion

        #region Properties

        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public ArchetypeData Data { get; private set; }
        public int ID { get; private set; }

        #endregion

        #region UnityEvents

        private void Awake()
        {
            _physicsComponent.Initialize();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isDying)
            {
                return;
            }

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
            _components.Clear();
            _tickableComponents.Clear();
            _fixedTickableComponents.Clear();
            _collisionComponents.Clear();

            Destroyed?.Invoke(this);
        }

        #endregion
        
        public void Initialize(IPublisher<EatPreyMessage> eatPreyPublisher, ArchetypeData data, int id)
        {
            Data = data;
            ID = id;
            _eatPreyPublisher = eatPreyPublisher;
        }

        public void Spawn(Vector3 spawnPosition, Quaternion bodyRotation)
        {
            Reset();
            
            SetPosition(spawnPosition);
            SetBodyRotation(bodyRotation);
            
            gameObject.SetActive(true);
            _animatorComponent.PlaySpawn();
        }

        private void Reset()
        {
            _isDying = false;
            _physicsComponent.Reset();
            _animatorComponent.Reset();
        }

        public void SetBounce(Vector3 normalizeDirection)
        {
            if (_isDying)
            {
                return;
            }

            var lookDirection = new Vector3(normalizeDirection.x, 0f, normalizeDirection.z);
            SetBodyRotation(Quaternion.LookRotation(lookDirection.normalized, Vector3.up));
            
            var bounceDirection = new Vector3(normalizeDirection.x, 
                Data.BounceUpValue, 
                normalizeDirection.z);
            
            _physicsComponent.AddBounceImpulse(bounceDirection, Data.BounceForce);
            NotifyBounceAwareComponents();
        }

        public void StartDeath()
        {
            if (_isDying)
            {
                return;
            }

            _isDying = true;
            _physicsComponent.Stop();
            _animatorComponent.PlayDeath(OnDeath);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void SetBodyRotation(Quaternion rotation)
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
                case IEntityBounceAwareComponent bounceAwareComponent:
                    _bounceAwareComponents.Add(bounceAwareComponent);
                    break;
            }
        }

        public void EatPrey(IEntity killed)
        {
            _eatPreyPublisher?.Publish(new EatPreyMessage(this, killed));
        }

        public void Tick()
        {
            if (_isDying)
            {
                return;
            }

            foreach (var component in _tickableComponents)
            {
                component.Tick();
            }
        }

        public void FixedTick()
        {
            if (_isDying)
            {
                return;
            }

            foreach (var component in _fixedTickableComponents)
            {
                component.FixedTick();
            }
        }

        public void CameraViewportExit()
        {
            if (_isDying)
            {
                return;
            }

            var backAngle = 
                _bodyTransform.eulerAngles.y 
                + Data.TurnBackAngle 
                + Random.Range(-Data.TurnRandomDelta, Data.TurnRandomDelta);
            
            SetBodyRotation(Quaternion.Euler(0f, backAngle, 0f));
        }

        public Transform GetWorldViewParent()
        {
            return _worldViewParent;
        }

        public Rigidbody GetRigidbody()
        {
            return _physicsComponent.GetRigidbody();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Vector3 GetMoveDirection()
        {
            return _bodyTransform.forward;
        }

        private void OnDeath()
        {
            SetVisible(false);
        }

        private void NotifyBounceAwareComponents()
        {
            foreach (var bounceAwareComponent in _bounceAwareComponents)
            {
                bounceAwareComponent.OnBounceImpulseApplied();
            }
        }
    }
}