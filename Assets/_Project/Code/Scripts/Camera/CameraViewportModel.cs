using System.Collections.Generic;
using Project.Entities;
using UnityEngine;

namespace Project.Services.CameraService
{
    public sealed class CameraViewportModel
    {
        private readonly Dictionary<int, ObservedEntityState> _observedByEntityId = new();
        private readonly List<int> _pendingRemoveIds = new();

        private readonly Camera _camera;
        private bool _isTicking;
        
        public CameraViewportModel(Camera camera)
        {
            _camera = camera;
        }

        public void AddViewportObserved(IEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            var id = entity.Id;

            _observedByEntityId[id] = new ObservedEntityState
            {
                Entity = entity,
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
            if (entity == null)
            {
                return;
            }

            RemoveViewportObserved(entity.Id);
        }

        private void RemoveViewportObserved(int entityId)
        {
            if (_isTicking)
            {
                _pendingRemoveIds.Add(entityId);
                return;
            }

            _observedByEntityId.Remove(entityId);
        }

        private void CheckExit()
        {
            foreach (var pair in _observedByEntityId)
            {
                var entityId = pair.Key;
                var state = pair.Value;

                var entity = state.Entity;

                if (entity == null)
                {
                    _pendingRemoveIds.Add(entityId);
                    continue;
                }

                var isInside = IsVisible(entity.GetPosition());

                if (state.WasInside && !isInside)
                {
                    entity.CameraViewportExit();
                }

                state.WasInside = isInside;
            }
        }

        private bool IsVisible(Vector3 observedPosition, float margin = 0.02f)
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
            if (_pendingRemoveIds.Count == 0)
            {
                return;
            }

            foreach (var id in _pendingRemoveIds)
            {
                _observedByEntityId.Remove(id);
            }

            _pendingRemoveIds.Clear();
        }
    }
}