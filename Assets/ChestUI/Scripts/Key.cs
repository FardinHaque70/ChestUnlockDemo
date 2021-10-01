using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PrizeUI
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private Transform m_keyTransform;

        private Vector3 keyInitialScale;

        private void Start()
        {
            keyInitialScale = m_keyTransform.localScale;
            m_keyTransform.localScale = Vector3.zero;
        }

        public void ShowKey(float delay = 0)
        {
            m_keyTransform.DOScale(keyInitialScale, 0.3f).SetDelay(delay).SetEase(Ease.OutBack, 3);
        }

        public void FlyToLock(Transform targetPosition, Action OnComplete)
        {
            Vector3[] points = new Vector3[3];
            points[0] = m_keyTransform.transform.position;
            points[1] = new Vector3(m_keyTransform.position.x, targetPosition.position.y,
                m_keyTransform.position.z - 2f);
            points[2] = targetPosition.position;
            Sequence seq = DOTween.Sequence();
            seq.Append(m_keyTransform.DOScale(m_keyTransform.localScale * 1.25f, 0.15f).SetEase(Ease.OutBack));
            seq.AppendInterval(0.1f);
            seq.Join(m_keyTransform.DOPath(points, 0.45f, PathType.CatmullRom, PathMode.Full3D)
                .SetLookAt(1f)
                .SetEase(Ease.Linear));
            seq.Append(m_keyTransform.DORotate(targetPosition.eulerAngles, 0.3f));
            seq.Append(m_keyTransform.DOScale(m_keyTransform.localScale * 0.7f, 0.2f));
            seq.Join(m_keyTransform.DOMoveZ(0.1f, 0.2f).SetRelative(true));
            seq.AppendInterval(0.1f);
            seq.Append(m_keyTransform.DORotate(new Vector3(0, 0, -90), 0.2f).SetRelative(true).OnComplete(() =>
            {
                OnComplete?.Invoke();
                m_keyTransform.gameObject.SetActive(false);
            }));
        }
    }
}