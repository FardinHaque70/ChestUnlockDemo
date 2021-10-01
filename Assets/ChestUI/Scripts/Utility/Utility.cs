using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
// using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

namespace Fardin
{
    public class Utility : MonoBehaviour
    {
        public static Utility instance;

        private Tween timeScaleTween;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
            }
        }

        #region Slow Mo

        public void SlowMo(float _targetTimeScale, float _duration, Ease _ease = Ease.Linear, Action OnComplete = null)
        {
            if (timeScaleTween != null && timeScaleTween.IsActive()) timeScaleTween.Kill();
            timeScaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, _targetTimeScale, _duration)
                .OnUpdate(() => { Time.fixedDeltaTime = 0.02f * Time.timeScale; })
                .SetEase(_ease)
                .SetUpdate(true)
                .OnComplete(() => { OnComplete?.Invoke(); });
        }

        public void ResetTimeScale()
        {
            if (timeScaleTween != null && timeScaleTween.IsActive()) timeScaleTween.Kill();
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        #endregion

        #region Reveal Text

        public void RevealText(TextMeshProUGUI _text, float _duration, Action _OnComplete)
        {
            int totalVisibleCharacter = _text.textInfo.characterCount;
            _text.maxVisibleCharacters = 0;
            int counter = 0;
            DOTween.To(() => counter, x => counter = x, totalVisibleCharacter, _duration)
                .SetEase(Ease.Linear)
                .OnUpdate(() => { _text.maxVisibleCharacters = counter; })
                .OnComplete(() => { _OnComplete?.Invoke(); });
        }

        public void RevealText(TextMeshPro _text, float _duration, Action _OnComplete)
        {
            int totalVisibleCharacter = _text.textInfo.characterCount;
            _text.maxVisibleCharacters = 0;
            int counter = 0;
            DOTween.To(() => counter, x => counter = x, totalVisibleCharacter, _duration)
                .SetEase(Ease.Linear)
                .OnUpdate(() => { _text.maxVisibleCharacters = counter; })
                .OnComplete(() => { _OnComplete?.Invoke(); })
                .SetUpdate(true);
        }

        #endregion

        #region Update Text

        public void UpdateText(TextMeshProUGUI _text, int _startCount, int _endCount, float _duration,
            Action OnStart = null,
            Action OnComplete = null, Action OnUpdate = null, float _delay = 0)
        {
            // StartCoroutine(_UpdateText(_text, _startCount, _endCount, _duration, OnStart, OnComplete, OnUpdate, _delay));
            _text.text = _startCount.ToString("N0");
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(_delay);
            seq.AppendCallback(() => { OnStart?.Invoke(); });
            float val = _startCount;
            DOTween.To(() => val, x => val = x, _endCount, _duration)
                .OnUpdate(() =>
                {
                    _text.text = val.ToString("N0");
                    OnUpdate?.Invoke();
                })
                .OnComplete(() =>
                {
                    _text.text = _endCount.ToString("N0");
                    OnComplete?.Invoke();
                });
        }

        #endregion

        #region Bezier Curve

        public static void CreatePathBetweenPoints(int numberOfPoints, ref Vector3[] points, Vector3 p0, Vector3 p1,
            Vector3 p2)
        {
            for (int i = 0; i < numberOfPoints + 1; i++)
            {
                float t = i / (float) numberOfPoints;
                points[i] = CalculateQuadraticeCurve(t, p0, p1, p2);
            }
        }

        private static Vector3 CalculateQuadraticeCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            //p1.x *= tightFactor;
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        #endregion
    }

    public static class ExtensionMethods
    {
        public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            System.Random rnd = new System.Random();
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}