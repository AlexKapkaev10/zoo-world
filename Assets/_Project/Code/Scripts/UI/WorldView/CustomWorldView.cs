using TMPro;
using DG.Tweening;
using System;
using Project.ScriptableObjects;
using UnityEngine;

namespace Project.UI
{
    public class CustomWorldView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textHeader;
        [SerializeField] private Transform _animatedRoot;
        [SerializeField] private CustomWorldViewConfig _config;

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
            
            _scaleTween = _animatedRoot
                .DOScale(Vector3.one, _config.ShowDuration)
                .From(0.0f)
                .SetEase(_config.ShowEase);
        }

        public void PlayHide(Action completeCallBack)
        {
            _scaleTween?.Kill();
            
            _scaleTween = _animatedRoot
                .DOScale(Vector3.zero, _config.HideDuration)
                .SetEase(_config.HideEase)
                .OnComplete(() => completeCallBack?.Invoke());
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