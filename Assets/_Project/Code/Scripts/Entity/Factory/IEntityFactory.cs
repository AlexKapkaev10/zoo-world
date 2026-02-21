using Project.ScriptableObjects;

namespace Project.Entities
{
    public interface IEntityFactory
    {
        IEntity Create(EntityArchetypeConfig archetype);
    }
}
