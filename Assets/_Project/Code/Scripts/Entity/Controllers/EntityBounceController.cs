using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class EntityBounceController
    {
        private readonly Transform _bodyTransform;
        private readonly PhysicsBehavior _physicsBehavior;
        private readonly EntityComponentsController _componentsController;

        private ArchetypeData _data;

        public EntityBounceController(
            Transform bodyTransform,
            PhysicsBehavior physicsBehavior,
            EntityComponentsController componentsController)
        {
            _bodyTransform = bodyTransform;
            _physicsBehavior = physicsBehavior;
            _componentsController = componentsController;
        }

        public void SetData(ArchetypeData data)
        {
            _data = data;
        }

        public void Apply(Vector3 direction)
        {
            var directionNormalized = direction.normalized;
            var lookDirection = new Vector3(directionNormalized.x, 0f, directionNormalized.z);
            _bodyTransform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            var bounceDirection = new Vector3(
                directionNormalized.x,
                _data.BounceUpValue,
                directionNormalized.z);

            _physicsBehavior.AddBounceImpulse(bounceDirection, _data.BounceForce);
            _componentsController.BounceApply();
        }
    }
}
