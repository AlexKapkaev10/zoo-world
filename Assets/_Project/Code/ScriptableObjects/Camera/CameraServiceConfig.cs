using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(CameraServiceConfig), menuName = "Config/Service/Camera")]
    public class CameraServiceConfig : ScriptableObject
    {
        [field: SerializeField] public Camera CameraPrefab { get; private set; }
        [field: SerializeField] public Vector3 CameraSpawnPosition { get; private set; }
        [field: SerializeField] public Vector3 CameraSpawnRotation { get; private set; }
    }
}