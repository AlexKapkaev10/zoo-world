using Project.Entities;

namespace Project.Messages
{
    public readonly struct EatPreyMessage
    {
        public readonly IEntity Killer;
        public readonly IEntity Killed;

        public EatPreyMessage(IEntity killer, IEntity killed)
        {
            Killer = killer;
            Killed = killed;
        }
    }
}