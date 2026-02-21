using UnityEngine;

namespace Project.Entities
{
    public sealed class SpawnAreaModel
    {
        private readonly Vector3 _center;
        private readonly Vector2 _range;

        public SpawnAreaModel(Vector3 center, Vector2 range)
        {
            _center = center;
            _range = range;
        }

        public Vector3 GetRandomPosition(float minY, float maxY)
        {
            float randomX = Random.Range(_center.x - _range.x, _center.x + _range.x);
            float randomZ = Random.Range(_center.z - _range.y, _center.z + _range.y);
            float randomY = Random.Range(minY, maxY);
            
            return new Vector3(randomX, randomY, randomZ);
        }

        public Quaternion GetRandomBodyRotation()
        {
            return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
    }
}
