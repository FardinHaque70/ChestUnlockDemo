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
    public class PrizeUIManager : MonoBehaviour
    {
        public PrizeManager PrizeManager;
        [SerializeField] private PrizeItem[] m_prizeItems;

        [HorizontalLine()] [Header("Key")] [SerializeField]
        private GameObject m_keyPrefab;

        [SerializeField] private RectTransform m_keyContainer;
        [SerializeField] private Button m_getKeyButton;
        [SerializeField] private CanvasGroup m_noThanksCG;
        [HorizontalLine()] [SerializeField] private Animator m_animator;
        [SerializeField] private RectTransform m_holder;
        [SerializeField] private RectTransform m_keyHolder;
        [SerializeField] private RectTransform m_header;
        [SerializeField] private RectTransform m_board;
        [SerializeField] private CanvasGroup m_boardCanvasGroup;
        [SerializeField] private TextMeshProUGUI m_selectChestText;
        [SerializeField] private TextMeshProUGUI m_prizeMachineText;
        [SerializeField] private Image m_fadeImage;

        private Key[] keys;
        private int keyCount = 0;
        private int keyTraveled = 0;
        private int unlockedCount = 0;

        private void Start()
        {
            InitKeys();
            HideNoThanksButton();
            StartCoroutine(ShowUI());
            m_boardCanvasGroup.blocksRaycasts = false;
        }

        private void InitKeys()
        {
            if (keys != null && keys.Length > 0)
                for (int i = 0; i < keys.Length; i++)
                {
                    Destroy(keys[i].gameObject);
                }

            keys = new Key[3];
            for (int i = 0; i < 3; i++)
            {
                keys[i] = Instantiate(m_keyPrefab, m_keyContainer).GetComponent<Key>();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_keyContainer);
            keyCount = 0;
            keyTraveled = 0;
            m_getKeyButton.interactable = false;
            m_getKeyButton.transform.localScale = Vector3.zero;
        }

        public void OnClickPrize(int index)
        {
            if (keyCount < 3)
            {
                m_prizeItems[index].OpenChest();
                keys[keyCount].FlyToLock(m_prizeItems[index].keyTargetPosition, () =>
                {
                    m_prizeItems[index].ChestOpen();
                    m_holder.DOComplete();
                    m_holder.DOShakePosition(0.1f, 15f, 50);
                    keyTraveled++;

                    if (keyTraveled == 3)
                    {
                        if (unlockedCount < 8)
                        {
                            ShowGetMoreKeyUI();
                        }
                        else
                        {
                            // NO MORE CHESTS ARE LEFT TO UNLOCK
                            m_keyHolder.gameObject.SetActive(false);
                            m_selectChestText.alpha = 0;
                        }
                    }
                });
                PrizeManager.OnPrizePickedEvent?.Invoke(index, PrizeManager.PrizeData.Prizes[index].value);
                keyCount++;
                unlockedCount++;
            }
        }

        private IEnumerator ShowUI()
        {
            PrizeManager.PrizeData.ShuffleList();
            m_keyHolder.sizeDelta = new Vector2(0, 150);
            m_selectChestText.alpha = 0;
            m_prizeMachineText.transform.localScale = Vector3.zero;
            m_header.transform.localScale = Vector3.zero;
            m_board.localScale = Vector3.zero;
            m_boardCanvasGroup.alpha = 0;
            m_boardCanvasGroup.blocksRaycasts = false;

            m_fadeImage.color = new Color(1, 1, 1, 1);
            m_fadeImage.DOFade(0, 2f);
            m_animator.SetTrigger("Init");
            yield return new WaitForSeconds(0.1f);
            m_board.DOScale(1, 0.3f).SetEase(Ease.OutBack, 2f);
            m_boardCanvasGroup.DOFade(1, 0.3f);
            yield return new WaitForSeconds(0.4f);
            m_prizeMachineText.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack, 2);
            // yield return new WaitForSeconds(0.1f);
            m_header.DOScale(1, 0.3f).SetEase(Ease.OutBack, 2f);
            for (int i = 0; i < m_prizeItems.Length; i++)
            {
                m_prizeItems[i].Init(i, PrizeManager.PrizeData.Prizes[i].value);
                m_prizeItems[i].PrizeUIManager = this;
                m_prizeItems[i].transform.DOScale(1, 0.3f).SetEase(Ease.OutBack, 2);
                yield return new WaitForSeconds(0.05f);
            }

            for (int i = 0; i < m_prizeItems.Length; i++)
            {
                m_prizeItems[i].ChestAppear();
                yield return new WaitForSeconds(0.05f);
            }


            yield return new WaitForSeconds(0.1f * 9);
            for (int i = 0; i < m_prizeItems.Length; i++)
            {
                m_prizeItems[i].ChestIdle();
            }

            ShowKeyUI();
            yield return new WaitForSeconds(0.5f);
            m_boardCanvasGroup.blocksRaycasts = true;
        }

        private void ShowKeyUI()
        {
            m_keyHolder.gameObject.SetActive(true);
            m_keyHolder.sizeDelta = new Vector2(0, 150);
            m_selectChestText.alpha = 0;
            m_selectChestText.rectTransform.anchoredPosition =
                new Vector2(0, m_selectChestText.rectTransform.anchoredPosition.y + 100);

            m_keyHolder.DOSizeDelta(new Vector2(350, 150), 0.3f).SetEase(Ease.OutBack, 2).OnComplete(() =>
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].ShowKey(0.1f * i);
                }
            });
            m_selectChestText.DOFade(1, 0.2f).SetDelay(0.5f);
            m_selectChestText.rectTransform.DOAnchorPosY(m_selectChestText.rectTransform.anchoredPosition.y - 100, 0.3f)
                .SetEase(Ease.OutBack, 2).SetDelay(0.5f);
        }

        private void ShowGetMoreKeyUI()
        {
            m_keyHolder.gameObject.SetActive(false);
            m_selectChestText.alpha = 0;
            m_getKeyButton.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack, 3).OnComplete(() =>
            {
                m_getKeyButton.interactable = true;
            });
            ShowNoThanksButton(3);
        }

        private void ShowNoThanksButton(float delay = 0)
        {
            m_noThanksCG.DOFade(1, 1f).SetDelay(delay).OnComplete(() => { m_noThanksCG.blocksRaycasts = true; });
        }

        private void HideNoThanksButton()
        {
            m_noThanksCG.DOKill();
            m_noThanksCG.alpha = 0;
            m_noThanksCG.blocksRaycasts = false;
        }

        public void OnClickGetMoreKey()
        {
            InitKeys();
            ShowKeyUI();
            HideNoThanksButton();
            m_getKeyButton.interactable = false;
            m_getKeyButton.transform.localScale = Vector3.zero;
            PrizeManager.OnGetMoreKeyEvent?.Invoke();
        }

        public void OnClickNoThanks()
        {
            PrizeManager.OnNoThanksEvent?.Invoke();
        }
    }
}