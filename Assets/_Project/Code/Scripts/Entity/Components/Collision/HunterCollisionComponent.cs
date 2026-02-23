using MessagePipe;
using Project.Messages;
using UnityEngine;

namespace Project.Entities.Components
{
    public sealed class HunterCollisionComponent : IEntityCollisionComponent
    {
        private readonly IPublisher<EatPreyMessage> _eatPreyPublisher;
        private IEntity _self;

        public HunterCollisionComponent(IPublisher<EatPreyMessage> eatPreyPublisher)
        {
            _eatPreyPublisher = eatPreyPublisher;
        }

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

        private void Eat(IEntity killer, IEntity killed)
        {
            _eatPreyPublisher.Publish(new EatPreyMessage(killer, killed));
        }

        private IEntity ResolveLoser(IEntity self, IEntity other)
        {
            return Random.Range(0, 2) == 1 ? self : other;
        }

        private bool HasFight(IEntity otherEntity)
        {
            if (otherEntity.Data.Kind != EntityKind.Hunter)
            {
                return false;
            }

            if (_self.Id < otherEntity.Id)
            {
                return true;
            }
            
            var loser = ResolveLoser(_self, otherEntity);
            var winner = loser == _self ? otherEntity : _self;
            
            Eat(winner, loser);
            
            return true;
        }
    }
}
