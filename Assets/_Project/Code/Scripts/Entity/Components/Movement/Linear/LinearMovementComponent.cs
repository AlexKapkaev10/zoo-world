using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities.Components.Movement
{
    public sealed class LinearMovementComponent : IEntityFixedTickableComponent
    {
        private readonly LinearMovementModel _model;
        
        private IEntity _entity;
        private Rigidbody _rigidbody;

        public LinearMovementComponent(LinearMovementConfig config)
        {
            _model = new LinearMovementModel(config);
        }

        public void Initialize(IEntity entity)
        {
            _entity = entity;
            _rigidbody = _entity.GetRigidbody();
        }

        public void FixedTick()
        {
            var velocity = _model.CalculateVelocity(
                _entity.GetMoveDirection(), 
                _rigidbody.linearVelocity);
            
            _rigidbody.linearVelocity = velocity;
        }
    }
}
