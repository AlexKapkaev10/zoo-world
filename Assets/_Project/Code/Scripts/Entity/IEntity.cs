using System;
using Project.Entities.Components;
using UnityEngine;

namespace Project.Entities
{
    public interface IEntity
    {
        event Action<IEntity> Deactivated;
        event Action<IEntity> Destroyed;
        EntityKind Kind { get; }
        Rigidbody Rigidbody { get; }
        void SetId(int id);
        void SetVisible(bool isVisible);
        void SetKind(EntityKind kind);
        void SetPosition(Vector3 position);
        void SetBodyRotation(Quaternion rotation);
        void SetViewportExitTurn(float turnBackAngle, float turnRandomDelta);
        void AddComponent(IEntityRuntimeComponent component);
        void TickComponents();
        void FixedTickComponents();
        void CameraViewportExit();
        int GetId();
        Vector3 GetVelocity();
        Vector3 GetPosition();
        Vector3 GetMoveDirection();
    }
}