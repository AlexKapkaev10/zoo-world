using UnityEngine;

namespace Project.Entities.Components
{
    public interface IEntityRuntimeComponent
    {
        void Initialize(IEntity entity);
    }

    public interface IEntityTickableComponent : IEntityRuntimeComponent
    {
        void Tick();
    }

    public interface IEntityFixedTickableComponent : IEntityRuntimeComponent
    {
        void FixedTick();
    }

    public interface IEntityCollisionComponent : IEntityRuntimeComponent
    {
        void OnCollisionEnter(Collision collision);
    }
}
