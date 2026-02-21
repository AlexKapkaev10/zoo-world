using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class LinearMovementModel
    {
        private readonly LinearMovementConfig _config;

        public LinearMovementModel(LinearMovementConfig config)
        {
            _config = config;
        }

        public Vector3 CalculateVelocity(Vector3 moveDirection, Vector3 currentVelocity)
        {
            var velocity = moveDirection.normalized * _config.Speed;
            velocity.y = currentVelocity.y;
            
            return velocity;
        }
    }
}