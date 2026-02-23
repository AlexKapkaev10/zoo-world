using System.Collections.Generic;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class EntityPool
    {
        private readonly IEntityFactory _entityFactory;
        private readonly Dictionary<EntityArchetypeConfig, Queue<IEntity>> _availableEntitiesByArchetype = new();
        private readonly Dictionary<IEntity, EntityArchetypeConfig> _archetypeByEntity = new();
        private readonly HashSet<IEntity> _entitiesInPool = new();

        public EntityPool(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public void Prewarm(IEnumerable<SpawnArchetypeData> data)
        {
            foreach (SpawnArchetypeData spawnData in data)
            {
                EnsureQueue(spawnData.Archetype);

                for (int i = 0; i < spawnData.StartPoolCount; i++)
                {
                    IEntity entity = Create(spawnData.Archetype);
                    ResetForPool(entity);
                    _availableEntitiesByArchetype[spawnData.Archetype].Enqueue(entity);
                    _entitiesInPool.Add(entity);
                }
            }
        }

        public IEntity Get(EntityArchetypeConfig archetype)
        {
            EnsureQueue(archetype);
            Queue<IEntity> queue = _availableEntitiesByArchetype[archetype];

            IEntity entity = null;
            while (queue.Count > 0)
            {
                entity = queue.Dequeue();
                if (entity != null)
                {
                    break;
                }
            }

            if (entity == null)
            {
                entity = Create(archetype);
            }

            _entitiesInPool.Remove(entity);
            return entity;
        }

        public void Release(IEntity entity)
        {
            if (_entitiesInPool.Contains(entity))
            {
                return;
            }

            if (!_archetypeByEntity.TryGetValue(entity, out EntityArchetypeConfig archetype))
            {
                return;
            }

            EnsureQueue(archetype);
            ResetForPool(entity);
            _availableEntitiesByArchetype[archetype].Enqueue(entity);
            _entitiesInPool.Add(entity);
        }

        private IEntity Create(EntityArchetypeConfig archetype)
        {
            IEntity entity = _entityFactory.Create(archetype);
            _archetypeByEntity[entity] = archetype;
            return entity;
        }

        private void EnsureQueue(EntityArchetypeConfig archetype)
        {
            if (!_availableEntitiesByArchetype.ContainsKey(archetype))
            {
                _availableEntitiesByArchetype[archetype] = new Queue<IEntity>();
            }
        }

        private void ResetForPool(IEntity entity)
        {
            var rigidbody = entity.GetRigidbody();

            if (!rigidbody.isKinematic)
            {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            entity.SetActive(false);
        }
    }
}