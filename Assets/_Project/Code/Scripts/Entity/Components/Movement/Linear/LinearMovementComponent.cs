using Project.ScriptableObjects;

namespace Project.Entities.Components.Movement
{
    public sealed class LinearMovementComponent : IEntityFixedTickableComponent
    {
        private readonly LinearMovementModel _model;
        
        private IEntity _entity;

        public LinearMovementComponent(LinearMovementConfig config)
        {
            _model = new LinearMovementModel(config);
        }

        public void Initialize(IEntity entity)
        {
            _entity = entity;
        }

        public void FixedTick()
        {
            var velocity = _model.CalculateVelocity(_entity.GetMoveDirection(), _entity.GetVelocity());
            
            _entity.Rigidbody.linearVelocity = velocity;
        }

        public void Dispose()
        {
        }
    }
}
