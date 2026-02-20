using System.Collections.Generic;
using Project.Entities;
using Project.Game;
using UnityEngine;

namespace Project.Models
{
    public sealed class CameraViewportModel
    {
        private readonly Dictionary<IEntity, ViewportObservedState> _observedEntityMap = new();
        private readonly Camera _camera;

        private const float Margin = 0.02f;

        public CameraViewportModel(Camera camera)
        {
            _camera = camera;
        }

        public void AddViewportObserved(IEntity entity)
        {
            if (entity == null || _observedEntityMap.ContainsKey(entity))
            {
                return;
            }

            _observedEntityMap[entity] = new ViewportObservedState
            {
                WasInside = IsVisible(entity.Transform.position)
            };
        }

        public void Tick()
        {
            CheckExit();
        }

        public void RemoveViewportObserved(IEntity entity)
        {
            if (entity == null)
            {
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
                    continue;
                }

                var isInside = IsVisible(entity.Transform.position, Margin);

                if (state.WasInside && !isInside)
                {
                    entity.CameraViewportExit();
                }

                state.WasInside = isInside;
            }
        }

        private bool IsVisible(Vector3 observedPosition, float margin = 0f)
        {
            if (_camera == null)
            {
                return false;
            }

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
    }
}