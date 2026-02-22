using System;
using MessagePipe;
using Project.Entities.Components;
using Project.Messages;
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
        int ID { get; }
        void Initialize(
            IPublisher<EatPreyMessage> eatPreyPublisher, 
            ArchetypeData data, 
            int id);
        void AddComponent(IEntityRuntimeComponent component);
        void Spawn(Vector3 spawnPosition, Quaternion bodyRotation);
        void SetVisible(bool isVisible);
        void SetBounce(Vector3 normalizeDirection);
        void EatPrey(IEntity killed);
        void StartDeath();
        void CameraViewportExit();
        Transform GetWorldViewParent();
        Rigidbody GetRigidbody();
        Vector3 GetPosition();
        Vector3 GetMoveDirection();
    }
}