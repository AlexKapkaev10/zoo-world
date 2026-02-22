using Project.Entities;
using Project.ScriptableObjects;
using UnityEngine;
using VContainer;

namespace Project.Services.CameraService
{
    public sealed class CameraService : ICameraService
    {
        private readonly CameraServiceConfig _config;
        
        private Camera _camera;
        private CameraViewportModel _viewportModel;

        [Inject]
        public CameraService(CameraServiceConfig config)
        {
            _config = config;
        }
        
        public void Initialize()
        {
            CreateCamera();
            _viewportModel = new CameraViewportModel(_camera);
        }

        public void Tick()
        {
            _viewportModel.Tick();
        }

        public void AddViewportObserved(IEntity entity)
        {
            _viewportModel.AddViewportObserved(entity);
        }

        public void RemoveViewportObserved(IEntity entity)
        {
            _viewportModel.RemoveViewportObserved(entity);
        }

        private void CreateCamera()
        {
            _camera = Object.Instantiate(_config.CameraPrefab);
            _camera.transform.position = _config.CameraSpawnPosition;
            _camera.transform.localEulerAngles = _config.CameraSpawnRotation;
        }
    }
}