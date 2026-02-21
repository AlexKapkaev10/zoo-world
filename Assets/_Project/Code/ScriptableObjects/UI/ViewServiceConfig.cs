using Project.UI;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(ViewServiceConfig), menuName = "Config/UI/View Service")]
    public class ViewServiceConfig : ScriptableObject
    {
        [field: SerializeField] public CustomWorldView WorldViewPrefab { get; private set; }
        [field: SerializeField] public string WorldViewHeader { get; private set; } = "tasty";
        [field: SerializeField] public float WorldViewLifetimeSeconds { get; private set; } = 2f;
    }
}