using System;
using VContainer.Unity;

namespace Project.Entities
{
    public interface IEntityService : IStartable, ITickable, IFixedTickable, IDisposable { }
}