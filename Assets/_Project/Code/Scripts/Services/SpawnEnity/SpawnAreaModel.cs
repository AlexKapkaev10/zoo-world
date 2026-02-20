using UnityEngine;

namespace Project.Services.SpawnEntity
{
    public sealed class SpawnAreaModel
    {
        private readonly Vector3 _center;
        private readonly Vector2 _range;
        private readonly float _groundY;

        public SpawnAreaModel(Vector3 center, Vector2 range, float groundY)
        {
            _center = center;
            _range = range;
            _groundY = groundY;
        }

        public Vector3 GetRandomPosition()
        {
            float randomX = Random.Range(_center.x - _range.x, _center.x + _range.x);
            float randomZ = Random.Range(_center.z - _range.y, _center.z + _range.y);
            return new Vector3(randomX, _groundY, randomZ);
        }

        public Quaternion GetRandomBodyRotation()
        {
            float yaw = Random.Range(0f, 360f);
            return Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
