using System;
using Project.Entities.Components;
using Project.ScriptableObjects;
using UnityEngine;
using VContainer.Unity;

namespace Project.Entities
{
    public interface IEntity : ITickable, IFixedTickable
    {
        event Action<IEntity> Deactivated;
        event Action<IEntity> Destroyed;

        ArchetypeData Data { get; }
        int Id { get; }

        void Initialize(ArchetypeData data, int id);
        void AddComponent(IEntityRuntimeComponent component);

        void Spawn(Vector3 spawnPosition, Quaternion bodyRotation);
        void SetActive(bool isActive);

        void SetBounce(Vector3 direction);
        void CameraViewportExit();
        void StartDeath();

        Vector3 GetPosition();
        Vector3 GetMoveDirection();
        Rigidbody GetRigidbody();
        Transform GetViewParent();
    }
}