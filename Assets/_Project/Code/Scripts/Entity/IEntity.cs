using System;
using Project.Entities.Components;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public interface IEntity
    {
        event Action<IEntity> Deactivated;
        event Action<IEntity> Destroyed;
        ArchetypeData Data { get; }
        int ID { get; }
        void Initialize(ArchetypeData data, int id);
        void AddComponent(IEntityRuntimeComponent component);
        void SetVisible(bool isVisible);
        void SetPosition(Vector3 position);
        void SetBodyRotation(Quaternion rotation);
        void SetBounce(Vector3 direction);
        void TickComponents();
        void FixedTickComponents();
        void CameraViewportExit();
        Rigidbody GetRigidbody();
        Vector3 GetPosition();
        Vector3 GetMoveDirection();
    }
}