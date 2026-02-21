using VContainer;

namespace Project.ScopeFactory
{
    public sealed class GameScopeFactory : IGameScopeFactory
    {
        private readonly IObjectResolver _resolver;

        public GameScopeFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public T Get<T>() where T : notnull
        {
            return _resolver.Resolve<T>();
        }
    }
}