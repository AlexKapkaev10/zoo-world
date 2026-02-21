using Project.Entities;
using Project.ScriptableObjects;

namespace Project.Services
{
    public interface IEntityFactory
    {
        IEntity Create(EntityArchetypeConfig archetype);
    }
}
