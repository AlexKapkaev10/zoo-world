namespace Project.Entities
{
    public readonly struct PendingDespawnData
    {
        public readonly IEntity Entity;
        public readonly bool ReturnToPool;

        public PendingDespawnData(IEntity entity, bool returnToPool)
        {
            Entity = entity;
            ReturnToPool = returnToPool;
        }
    }
}