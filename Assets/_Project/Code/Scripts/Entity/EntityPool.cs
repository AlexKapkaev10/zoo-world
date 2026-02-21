using System.Collections.Generic;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class EntityPool
    {
        private readonly IEntityFactory _entityFactory;
        private readonly Dictionary<EntityArchetypeConfig, Queue<IEntity>> _availableByArchetype = new();
        private readonly Dictionary<IEntity, EntityArchetypeConfig> _archetypeByEntity = new();
        private readonly HashSet<IEntity> _inPool = new();

        public EntityPool(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public void Prewarm(IEnumerable<SpawnArchetypeData> dates)
        {
            foreach (SpawnArchetypeData data in dates)
            {
                EnsureQueue(data.Archetype);
                
                for (int i = 0; i < data.PrewarmCount; i++)
                {
                    IEntity entity = Create(data.Archetype);
                    PrepareForRelease(entity);
                    _availableByArchetype[data.Archetype].Enqueue(entity);
                    _inPool.Add(entity);
                }
            }
        }

        public IEntity Get(EntityArchetypeConfig archetype)
        {
            EnsureQueue(archetype);
            Queue<IEntity> queue = _availableByArchetype[archetype];

            IEntity entity = null;
            while (queue.Count > 0 && entity == null)
            {
                entity = queue.Dequeue();
            }

            if (entity == null)
            {
                entity = Create(archetype);
            }

            _inPool.Remove(entity);
            PrepareForGet(entity);
            return entity;
        }

        public void Release(IEntity entity)
        {
            if (_inPool.Contains(entity))
            {
                return;
            }

            if (!_archetypeByEntity.TryGetValue(entity, out EntityArchetypeConfig archetype))
            {
                return;
            }

            EnsureQueue(archetype);
            PrepareForRelease(entity);
            _availableByArchetype[archetype].Enqueue(entity);
            _inPool.Add(entity);
        }

        private IEntity Create(EntityArchetypeConfig archetype)
        {
            IEntity entity = _entityFactory.Create(archetype);
            _archetypeByEntity[entity] = archetype;
            return entity;
        }

        private void EnsureQueue(EntityArchetypeConfig archetype)
        {
            if (!_availableByArchetype.ContainsKey(archetype))
            {
                _availableByArchetype[archetype] = new Queue<IEntity>();
            }
        }

        private void PrepareForGet(IEntity entity)
        {
            entity.PrepareForSpawn();
            entity.SetVisible(true);
        }

        private void PrepareForRelease(IEntity entity)
        {
            var rigidbody = entity.GetRigidbody();
            if (!rigidbody.isKinematic)
            {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
            
            entity.SetVisible(false);
        }
    }
}
