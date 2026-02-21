using UnityEngine;

namespace Project.Entities.Components
{
    public sealed class HunterCollisionComponent : IEntityCollisionComponent
    {
        private IEntity _self;

        public void Initialize(IEntity entity)
        {
            _self = entity;
        }

        public void OnCollisionEnter(Collision collision)
        {
            EntityCollision(collision);
        }

        private void EntityCollision(Collision collision)
        {
            if (!collision.collider.TryGetComponent(out IEntity otherEntity))
            {
                return;
            }

            if (HasFight(otherEntity))
            {
                return;
            }

            Eat(_self, otherEntity);
        }

        private bool HasFight(IEntity otherEntity)
        {
            if (otherEntity.Data.Kind != EntityKind.Hunter)
            {
                return false;
            }

            if (_self.ID < otherEntity.ID)
            {
                return true;
            }
            
            var loser = ResolveLoser(_self, otherEntity);
            var winner = loser == _self ? otherEntity : _self;
            
            Eat(winner, loser);
            
            return true;
        }

        private void Eat(IEntity killer, IEntity killed)
        {
            killer.EatPrey(killed);
        }

        private IEntity ResolveLoser(IEntity self, IEntity other)
        {
            return Random.Range(0, 2) == 1 ? self : other;
        }

        public void Dispose()
        {
        }
    }
}
