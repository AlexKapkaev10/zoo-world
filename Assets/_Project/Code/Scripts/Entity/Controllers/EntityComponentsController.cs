using System.Collections.Generic;
using Project.Entities.Components;
using UnityEngine;

namespace Project.Entities
{
    public class EntityComponentsController
    {
        private readonly IEntity _owner;
        private readonly List<IEntityRuntimeComponent> _components = new();
        private readonly List<IEntityTickableComponent> _tickableComponents = new();
        private readonly List<IEntityFixedTickableComponent> _fixedTickableComponents = new();
        private readonly List<IEntityCollisionComponent> _collisionComponents = new();
        private readonly List<IEntityBounceApply> _bounceApplies = new();

        public EntityComponentsController(IEntity owner)
        {
            _owner = owner;
        }
        
        public void AddComponent(IEntityRuntimeComponent component)
        {
            component.Initialize(_owner);
            _components.Add(component);

            switch (component)
            {
                case IEntityTickableComponent tickableComponent:
                    _tickableComponents.Add(tickableComponent);
                    break;
                case IEntityFixedTickableComponent fixedTickableComponent:
                    _fixedTickableComponents.Add(fixedTickableComponent);
                    break;
                case IEntityCollisionComponent collisionComponent:
                    _collisionComponents.Add(collisionComponent);
                    break;
                case IEntityBounceApply bounceAwareComponent:
                    _bounceApplies.Add(bounceAwareComponent);
                    break;
            }
        }
        
        public void OnCollisionEnter(Collision collision)
        {
            foreach (var collisionComponent in _collisionComponents)
            {
                collisionComponent.OnCollisionEnter(collision);
            }
        }

        public void Tick()
        {
            foreach (var component in _tickableComponents)
            {
                component.Tick();
            }
        }

        public void FixedTick()
        {
            foreach (var component in _fixedTickableComponents)
            {
                component.FixedTick();
            }
        }

        public void BounceApply()
        {
            foreach (var bounceAwareComponent in _bounceApplies)
            {
                bounceAwareComponent.OnBounceApply();
            }
        }

        public void Clear()
        {
            _components.Clear();
            _tickableComponents.Clear();
            _fixedTickableComponents.Clear();
            _collisionComponents.Clear();
            _bounceApplies.Clear();
        }
    }
}