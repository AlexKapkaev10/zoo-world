using System;
using Project.Entities.Components;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class Entity : MonoBehaviour, IEntity
    {
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _viewParent;

        [SerializeField] private AnimatorBehavior _animatorBehavior;
        [SerializeField] private PhysicsBehavior _physicsBehavior;

        private EntityComponentsController _componentsController;
        private EntityLifecycle _lifecycle;
        private EntityBounceController _bounceController;
        private ViewportExitHandler _viewportExitHandler;

        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public ArchetypeData Data { get; private set; }
        public int Id { get; private set; }

        private void Awake()
        {
            _physicsBehavior.Initialize();
            
            _componentsController = new EntityComponentsController(this);
            _lifecycle = new EntityLifecycle(_physicsBehavior, _animatorBehavior, OnDeath);
            _bounceController = new EntityBounceController(_body, _physicsBehavior, _componentsController);
            _viewportExitHandler = new ViewportExitHandler(_body);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_lifecycle.IsDying)
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

        public void Initialize(ArchetypeData data, int id)
        {
            Data = data;
            Id = id;
            _bounceController.SetData(data);
            _viewportExitHandler.SetData(data);
        }

        public void Spawn(Vector3 spawnPosition, Quaternion bodyRotation)
        {
            _lifecycle.Reset();

            SetPosition(spawnPosition);
            SetBodyRotation(bodyRotation);

            gameObject.SetActive(true);
            _animatorBehavior.PlaySpawn();
        }

        public void AddComponent(IEntityRuntimeComponent component)
        {
            _componentsController.AddComponent(component);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetBounce(Vector3 direction)
        {
            if (_lifecycle.IsDying)
            {
                return;
            }

            _bounceController.Apply(direction);
        }

        public void Tick()
        {
            if (_lifecycle.IsDying)
            {
                return;
            }

            _componentsController.Tick();
        }

        public void FixedTick()
        {
            if (_lifecycle.IsDying)
            {
                return;
            }

            _componentsController.FixedTick();
        }

        public void StartDeath()
        {
            _lifecycle.StartDeath();
        }

        public void CameraViewportExit()
        {
            if (_lifecycle.IsDying)
            {
                return;
            }

            _viewportExitHandler.HandleExit();
        }

        public Transform GetViewParent()
        {
            return _viewParent;
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
            return _body.forward;
        }

        private void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        private void SetBodyRotation(Quaternion rotation)
        {
            _body.rotation = rotation;
        }

        private void OnDeath()
        {
            SetActive(false);
        }
    }
}
