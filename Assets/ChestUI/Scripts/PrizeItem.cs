using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrizeUI
{
    public class PrizeItem : MonoBehaviour
    {
        public Transform keyTargetPosition;
        [SerializeField] private int index;
        [SerializeField] private int value;
        [HideInInspector] public PrizeUIManager PrizeUIManager;
        [SerializeField] private Transform _chestTransform;
        [SerializeField] private Animator m_animator;
        [SerializeField] private Button m_button;
        [HorizontalLine()] [SerializeField] private GameObject lockedBG;
        [SerializeField] private GameObject unLockedBG;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image iconImage;
        [HorizontalLine()] [SerializeField] private ParticleSystem selectParticle;
        [SerializeField] private ParticleSystem appearParticle;
        [SerializeField] private ParticleSystem chestOpenParticle;
        [SerializeField] private ParticleSystem chestDropParticle;
        [SerializeField] private ParticleSystem chestDisapperaParticle;

        private void Start()
        {
            _chestTransform.localScale = Vector3.zero;
            transform.localScale = Vector3.zero;
        }

        public void Init(int _index, int _value)
        {
            index = _index;
            value = _value;
            valueText.text = "+" + value.ToString();
        }

        public void ChestAppear()
        {
            m_animator.SetTrigger("Appear");
            appearParticle.Play();
        }

        public void ChestIdle()
        {
            m_animator.SetTrigger("Idle");
        }

        public void ChestOpen()
        {
            m_animator.SetTrigger("Open");
            chestOpenParticle.Play();
            Invoke(nameof(ShowInfo), 1);
        }

        private void ShowInfo()
        {
            m_animator.enabled = false;
            chestOpenParticle.Stop();
            chestDisapperaParticle.Play();
            _chestTransform.DOScale(0, 0.3f).SetEase(Ease.InBack, 5);
            valueText.gameObject.SetActive(true);
            iconImage.gameObject.SetActive(true);
        }

        public void OnChestDrop()
        {
            chestDropParticle.Play();
        }

        public void OnClick()
        {
            PrizeUIManager.OnClickPrize(index);
        }

        public void OpenChest()
        {
            m_animator.SetTrigger("Clicked");
            lockedBG.SetActive(false);
            unLockedBG.SetActive(true);
            selectParticle.Play();
            m_button.interactable = false;
        }
    }
}