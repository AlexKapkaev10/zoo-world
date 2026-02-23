using Project.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Entities
{
    public sealed class ViewportExitHandler
    {
        private readonly Transform _bodyTransform;
        private ArchetypeData _data;

        public ViewportExitHandler(Transform bodyTransform)
        {
            _bodyTransform = bodyTransform;
        }

        public void SetData(ArchetypeData data)
        {
            _data = data;
        }

        public void HandleExit()
        {
            var backAngle =
                _bodyTransform.eulerAngles.y
                + _data.TurnBackAngle
                + Random.Range(-_data.TurnRandomDelta, _data.TurnRandomDelta);

            _bodyTransform.rotation = Quaternion.Euler(0f, backAngle, 0f);
        }
    }
}
