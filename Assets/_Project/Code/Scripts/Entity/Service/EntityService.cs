using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Project.Messages;
using Project.ScopeFactory;
using Project.ScriptableObjects;
using Project.Services.CameraService;
using VContainer;

namespace Project.Entities
{
    public sealed class EntityService : IEntityService
    {
        private readonly CancellationTokenSource _spawnCts = new();
        private readonly EntityServiceConfig _config;
        private readonly EntityPool _pool;
        private readonly EntityRuntimeController _runtime;
        private readonly IDisposable _eatPreySubscription;

        [Inject]
        public EntityService(IGameScopeFactory factory, EntityServiceConfig config)
        {
            _config = config;

            _pool = new EntityPool(factory.Get<IEntityFactory>());

            _runtime = new EntityRuntimeController(
                pool: _pool,
                spawnModel: new SpawnEntityModel(_config),
                cameraService: factory.Get<ICameraService>());

            _eatPreySubscription = factory
                .Get<ISubscriber<EatPreyMessage>>()
                .Subscribe(OnEatPreyMessage);
        }

        public void Start()
        {
            _pool.Prewarm(_config.SpawnData);
            SpawnAsync(_spawnCts.Token).Forget();
        }

        public void Tick()
        {
            _runtime.Tick();
        }

        public void FixedTick()
        {
            _runtime.FixedTick();
        }

        public void Dispose()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            _eatPreySubscription?.Dispose();
            _runtime.Dispose();
        }

        private void OnEatPreyMessage(EatPreyMessage message)
        {
            message.Killed.StartDeath();
        }

        private async UniTaskVoid SpawnAsync(CancellationToken token)
        {
            var spawnInterval = TimeSpan.FromSeconds(_config.SpawnIntervalSeconds);

            while (!token.IsCancellationRequested)
            {
                _runtime.TrySpawnOne();
                await UniTask.Delay(spawnInterval, cancellationToken: token);
            }
        }
    }
}