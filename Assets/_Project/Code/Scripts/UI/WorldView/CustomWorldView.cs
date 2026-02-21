using TMPro;
using DG.Tweening;
using System;
using UnityEngine;

namespace Project.UI
{
    public class CustomWorldView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textHeader;
        [SerializeField] private Transform _animatedRoot;
        [SerializeField] private float _showDuration = 0.2f;
        [SerializeField] private float _hideDuration = 0.2f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private Ease _hideEase = Ease.InBack;

        private Tween _scaleTween;

        private void Awake()
        {
            _animatedRoot.localScale = Vector3.zero;
        }

        public void SetHeader(string header)
        {
            _textHeader.SetText(header);
        }

        public void PlayShow(string header)
        {
            SetHeader(header);
            _scaleTween?.Kill();
            _animatedRoot.localScale = Vector3.zero;
            _scaleTween = _animatedRoot
                .DOScale(Vector3.one, _showDuration)
                .SetEase(_showEase);
        }

        public void PlayHide(Action onComplete)
        {
            _scaleTween?.Kill();
            _scaleTween = _animatedRoot
                .DOScale(Vector3.zero, _hideDuration)
                .SetEase(_hideEase)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void CancelHide()
        {
            _scaleTween?.Kill();
            _animatedRoot.localScale = Vector3.one;
        }

        private void OnDestroy()
        {
            _scaleTween?.Kill();
        }
    }
}