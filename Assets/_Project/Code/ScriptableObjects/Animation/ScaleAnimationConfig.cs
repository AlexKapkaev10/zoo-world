using DG.Tweening;
using UnityEngine;

namespace Project.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(ScaleAnimationConfig), menuName = "Config/Animation/Scale")]
    public class ScaleAnimationConfig : ScriptableObject
    {
        [field: SerializeField] public float ShowDuration {get; private set;} = 0.2f;
        [field: SerializeField] public float HideDuration {get; private set;} = 0.2f;
        [field: SerializeField] public Ease ShowEase {get; private set;} = Ease.OutBack;
        [field: SerializeField] public Ease HideEase {get; private set;} = Ease.InBack;
    }
}