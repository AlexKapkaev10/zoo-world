using Project.UI.MVP;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(InfoPresenterConfig), menuName = "Config/UI/MVP/Info Presenter")]
    public class InfoPresenterConfig : ScriptableObject
    {
        [field: SerializeField] public InfoView ViewPrefab { get; private set; }
    }
}