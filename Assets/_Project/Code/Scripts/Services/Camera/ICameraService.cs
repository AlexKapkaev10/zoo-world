using Project.Entities;
using VContainer.Unity;

namespace Project.Services
{
    public interface ICameraService : IInitializable, ITickable
    {
        void AddViewportObserved(IEntity entity);
        void RemoveViewportObserved(IEntity entity);
    }
}