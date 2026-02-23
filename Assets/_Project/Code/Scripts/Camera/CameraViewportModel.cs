using System.Collections.Generic;
using Project.Entities;
using UnityEngine;

namespace Project.Services.CameraService
{
    public sealed class CameraViewportModel
    {
        private readonly Dictionary<IEntity, CameraViewportObservedData> _observedEntityMap = new();
        private readonly List<IEntity> _pendingRemove = new();
        private readonly Camera _camera;
        private bool _isTicking;

        private const float Margin = 0.02f;

        public CameraViewportModel(Camera camera)
        {
            _camera = camera;
        }

        public void AddViewportObserved(IEntity entity)
        {
            _observedEntityMap[entity] = new CameraViewportObservedData
            {
                WasInside = IsVisible(entity.GetPosition())
            };
        }

        public void Tick()
        {
            _isTicking = true;
            CheckExit();
            _isTicking = false;
            FlushPendingRemove();
        }

        public void RemoveViewportObserved(IEntity entity)
        {
            if (_isTicking)
            {
                _pendingRemove.Add(entity);
                return;
            }

            _observedEntityMap.Remove(entity);
        }

        private void CheckExit()
        {
            foreach (var keyValuePair in _observedEntityMap)
            {
                var entity = keyValuePair.Key;
                var state = keyValuePair.Value;

                if (entity == null)
                {
                    _pendingRemove.Add(entity);
                    continue;
                }

                var isInside = IsVisible(entity.GetPosition(), Margin);

                if (state.WasInside && !isInside)
                {
                    entity.CameraViewportExit();
                }

                state.WasInside = isInside;
            }
        }

        private bool IsVisible(Vector3 observedPosition, float margin = 0f)
        {
            var point = _camera.WorldToViewportPoint(observedPosition);

            if (point.z <= 0f)
            {
                return false;
            }

            return IsPointInsideFrustum(margin, point);
        }

        private bool IsPointInsideFrustum(float margin, Vector3 viewportPoint)
        {
            return viewportPoint.x >= -margin &&
                   viewportPoint.x <= 1f + margin &&
                   viewportPoint.y >= -margin &&
                   viewportPoint.y <= 1f + margin;
        }

        private void FlushPendingRemove()
        {
            if (_pendingRemove.Count == 0)
            {
                return;
            }

            foreach (var entity in _pendingRemove)
            {
                _observedEntityMap.Remove(entity);
            }

            _pendingRemove.Clear();
        }
    }
}