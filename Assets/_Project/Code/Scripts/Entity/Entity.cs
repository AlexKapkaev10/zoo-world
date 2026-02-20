using System;
using UnityEngine;

namespace Project.Entities
{
    public interface IEntity
    {
        event Action<IEntity> Destroyed;
        Transform Transform { get; }
        void CameraViewportExit();
    }
    
    public class Entity : MonoBehaviour, IEntity
    {
        public event Action<IEntity> Destroyed;
        public Transform Transform => transform;
        
        public void CameraViewportExit()
        {
            Debug.Log($"{gameObject.name} Switch way point");
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}