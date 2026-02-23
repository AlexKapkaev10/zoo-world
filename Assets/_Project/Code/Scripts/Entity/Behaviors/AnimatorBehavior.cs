using System;
using DG.Tweening;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.Entities
{
    public sealed class AnimatorBehavior : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private ScaleAnimationConfig _config;

        private Tween _scaleTween;
        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = _targetTransform.localScale;
        }

        private void OnDisable()
        {
            KillScaleTween();
        }

        private void OnDestroy()
        {
            KillScaleTween();
        }

        public void Reset()
        {
            KillScaleTween();
            _targetTransform.localScale = _initialScale;
        }

        public void PlaySpawn()
        {
            KillScaleTween();
            
            _targetTransform.localScale = Vector3.zero;
            _scaleTween = _targetTransform
                .DOScale(_initialScale, _config.ShowDuration)
                .SetEase(_config.ShowEase);
        }

        public void PlayDeath(Action onComplete)
        {
            KillScaleTween();
            _scaleTween = _targetTransform
                .DOScale(Vector3.zero, _config.HideDuration)
                .SetEase(_config.HideEase)
                .OnComplete(() => onComplete?.Invoke());
        }

        private void KillScaleTween()
        {
            _scaleTween?.Kill();
            _scaleTween = null;
        }
    }
}
