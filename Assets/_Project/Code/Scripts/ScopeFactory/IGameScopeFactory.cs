namespace Project.ScopeFactory
{
    public interface IGameScopeFactory
    {
        T Get<T>() where T : notnull;
    }
}