using System;
using MessagePipe;
using Project.Entities.Components;
using Project.Messages;
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
        void Initialize(IPublisher<EatPreyMessage> eatPreyPublisher, 
            ArchetypeData data, 
            int id);
        void AddComponent(IEntityRuntimeComponent component);
        void Spawn(Vector3 spawnPosition, Quaternion bodyRotation);
        void PrepareForSpawn();
        void BeginDeath();
        void SetVisible(bool isVisible);
        void SetBounce(Vector3 direction);
        void EatPrey(IEntity killed);
        void TickComponents();
        void FixedTickComponents();
        void CameraViewportExit();
        Transform GetWorldViewParent();
        Rigidbody GetRigidbody();
        Vector3 GetPosition();
        Vector3 GetMoveDirection();
    }
}