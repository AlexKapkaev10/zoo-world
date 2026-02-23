using System;
using Project.Entities.Components;
using Project.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Entities
{
    public sealed class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Transform _worldViewParent;
        
        [SerializeField] private AnimatorBehavior _animatorBehavior;
        [SerializeField] private PhysicsBehavior _physicsBehavior;
        
        private EntityComponentsController _componentsController;
        private bool _isDying;

        #region Properties

        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public ArchetypeData Data { get; private set; }
        public int Id { get; private set; }

        #endregion

        #region UnityEvents

        private void Awake()
        {
            _physicsBehavior.Initialize();
            _componentsController = new EntityComponentsController(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isDying)
            {
                return;
            }
            
            _componentsController.OnCollisionEnter(collision);
        }

        private void OnDisable()
        {
            Deactivated?.Invoke(this);
        }

        private void OnDestroy()
        {
            _componentsController.Clear();

            Destroyed?.Invoke(this);
        }

        #endregion
        
        public void Initialize(ArchetypeData data, int id)
        {
            Data = data;
            Id = id;
        }

        public void Spawn(Vector3 spawnPosition, Quaternion bodyRotation)
        {
            Reset();
            
            SetPosition(spawnPosition);
            SetBodyRotation(bodyRotation);
            
            gameObject.SetActive(true);
            _animatorBehavior.PlaySpawn();
        }

        public void AddComponent(IEntityRuntimeComponent component)
        {
            _componentsController.AddComponent(component);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetBounce(Vector3 normalizeDirection)
        {
            if (_isDying)
            {
                return;
            }
            
            BounceApply(normalizeDirection);
        }

        public void Tick()
        {
            if (_isDying)
            {
                return;
            }

            _componentsController.Tick();
        }

        public void FixedTick()
        {
            if (_isDying)
            {
                return;
            }
            
            _componentsController.FixedTick();
        }

        public void StartDeath()
        {
            if (_isDying)
            {
                return;
            }

            _isDying = true;
            _physicsBehavior.Stop();
            _animatorBehavior.PlayDeath(OnDeath);
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

        public Transform GetViewParent()
        {
            return _worldViewParent;
        }

        public Rigidbody GetRigidbody()
        {
            return _physicsBehavior.GetRigidbody();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Vector3 GetMoveDirection()
        {
            return _bodyTransform.forward;
        }
        
        private void Reset()
        {
            _isDying = false;
            _physicsBehavior.Reset();
            _animatorBehavior.Reset();
        }
        
        private void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void SetBodyRotation(Quaternion rotation)
        {
            _bodyTransform.rotation = rotation;
        }

        private void OnDeath()
        {
            SetVisible(false);
        }

        private void BounceApply(Vector3 normalizeDirection)
        {
            var lookDirection = new Vector3(normalizeDirection.x, 0f, normalizeDirection.z);
            SetBodyRotation(Quaternion.LookRotation(lookDirection.normalized, Vector3.up));
            
            var bounceDirection = new Vector3(normalizeDirection.x, 
                Data.BounceUpValue, 
                normalizeDirection.z);
            
            _physicsBehavior.AddBounceImpulse(bounceDirection, Data.BounceForce);
            
            _componentsController.BounceApply();
        }
    }
}