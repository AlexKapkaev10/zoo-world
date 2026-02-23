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

        public event Action<IEntity> Deactivated;
        public event Action<IEntity> Destroyed;
        public ArchetypeData Data { get; private set; }
        public int Id { get; private set; }

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

        public void Initialize(ArchetypeData data, int id)
        {
            Data = data;
            Id = id;
        }

        public void Spawn(Vector3 spawnPosition, Quaternion bodyRotation)
        {
            ResetState();

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
            if (_isDying)
            {
                return;
            }

            ApplyBounce(direction.normalized);
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

        private void ResetState()
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
            SetActive(false);
        }

        private void ApplyBounce(Vector3 directionNormalized)
        {
            var lookDirection = new Vector3(directionNormalized.x, 0f, directionNormalized.z);
            SetBodyRotation(Quaternion.LookRotation(lookDirection, Vector3.up));

            var bounceDirection = new Vector3(
                directionNormalized.x,
                Data.BounceUpValue,
                directionNormalized.z);

            _physicsBehavior.AddBounceImpulse(bounceDirection, Data.BounceForce);

            _componentsController.BounceApply();
        }
    }
}