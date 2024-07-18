using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    public enum GameState
    {
        Perintah,
        Main
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private ResultItem[] resultItems;
        private int currentIdx;
        [SerializeField] private PlayerItemGrabber playerItemGrabber;
        [SerializeField] private CountdownTimer countdownTimer;
        [SerializeField] private RadialClock radialClock;

        [Header("UI")]
        [SerializeField] private RectTransform gameplayUI;
        [SerializeField] private RectTransform perintahUI;
        [SerializeField] private TextMeshProUGUI perintahText;
        [SerializeField] private RectTransform perintahItemContainer;
        [SerializeField] private GameObject perintahItemPrefab;
        [SerializeField] private RectTransform grabbedItemContainer;

        [Header("Result")]
        [SerializeField] private RectTransform resultUI;
        [SerializeField] private GameObject resultUIPrefab;
        [SerializeField] private Image resultStatusImage;
        [SerializeField] private Sprite statusCorrect;
        [SerializeField] private Sprite statusIncorrect;

        [Header("Events")]
        [SerializeField] private UnityEvent OnEnd;
        [SerializeField] private UnityEvent OnLose;
        [SerializeField] private UnityEvent OnWin;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            GameStateManager.Instance.ToOpenUI(); // Tell GameStateManager to open UI exclusively
        }

        public void DisplayPerintah() 
        {
            GameStateManager.Instance.ToOpenUI(); // Tell GameStateManager to open UI on top

            gameplayUI.gameObject.SetActive(false);
            perintahUI.gameObject.SetActive(true);

            perintahText.text = $"Perintah {currentIdx + 1}/{resultItems.Length}";
            
            // Clear the container
            foreach (Transform child in perintahItemContainer) {
                Destroy(child.gameObject);
            }

            // Add the items
            foreach (var item in resultItems[currentIdx].recipe) {
                var itemObj = Instantiate(perintahItemPrefab, perintahItemContainer);
                itemObj.GetComponent<Image>().sprite = item.itemSprite;
            }

            radialClock.StartClock();
        }

        public void ToGameplay()
        {
            GameStateManager.Instance.ToGameplayOverride(); // To Gameplay, and override the UI

            gameplayUI.gameObject.SetActive(true);
            perintahUI.gameObject.SetActive(false);

            countdownTimer.StartTimer();
        }

        private bool CompareResult() {
            if (playerItemGrabber.GrabbedItems.Count != resultItems.Length) {
                return false;
            }

            for (int i = 0; i < playerItemGrabber.GrabbedItems.Count; i++) {
                if (playerItemGrabber.GrabbedItems[i].name != resultItems[currentIdx].name) {
                    InGameDebug.Instance.Log("Incorrect because " + playerItemGrabber.GrabbedItems[i].name + " != " 
                        + resultItems[currentIdx].name + " at index " + i);
                    return false;
                }
            }

            return true;
        }

        private void OnCorrect() {
            playerItemGrabber.GrabbedItems.Clear();
            // Destroy all items
            foreach (Transform child in grabbedItemContainer) {
                Destroy(child.gameObject);
            }

            // if succesful
            if (currentIdx == resultItems.Length - 1) {
                OnWin?.Invoke();
            } else {
                currentIdx++;
                countdownTimer.ResetTimer();
                DisplayPerintah();
            }
        }

        private void OnIncorrect() {
            playerItemGrabber.GrabbedItems.Clear();
            // Destroy all items
            foreach (Transform child in grabbedItemContainer) {
                Destroy(child.gameObject);
            }

            DisplayPerintah();
        }

        public void MergeAndSpawn(bool isCorrect, Action onComplete)
        {
            int completedMerges = 0;
            float fadeDuration = 0.5f;
            float mergeDuration = 1f;

            List<RectTransform> uiElementsToMerge = new List<RectTransform>();
            foreach (Transform child in grabbedItemContainer)
            {
                uiElementsToMerge.Add(child.GetComponent<RectTransform>());
            }

            RectTransform mergePoint = grabbedItemContainer;
            HorizontalLayoutGroup layoutGroup = grabbedItemContainer.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.enabled = false;

            foreach (var element in uiElementsToMerge)
            {
                // Fade out
                element.GetComponentInChildren<Image>().DOFade(0, fadeDuration);

                // Move to merge point
                element.DOMove(mergePoint.position, mergeDuration).OnComplete(() =>
                {
                    completedMerges++;

                    // Check if all elements have merged
                    if (completedMerges == uiElementsToMerge.Count)
                    {
                        grabbedItemContainer.gameObject.SetActive(false);
                        resultUI.gameObject.SetActive(true);

                        // Spawn new object
                        var resultPrefab = Instantiate(resultUIPrefab, resultUI);
                        resultPrefab.GetComponent<ResultItemUI>().Set(
                            resultItems[currentIdx].resultSprite, 
                            resultItems[currentIdx].resultName
                        );

                        // Set status image
                        resultStatusImage.sprite = isCorrect ? statusCorrect : statusIncorrect;

                        // Add delay before onComplete
                        DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
                        {
                            onComplete?.Invoke();
                            layoutGroup.enabled = true;

                            grabbedItemContainer.gameObject.SetActive(true);
                            resultUI.gameObject.SetActive(false);
                        });
                    }
                });
            }
        }

        public void TryMerge() {
            bool isCorrect = CompareResult();
            InGameDebug.Instance.Log("Is correct: " + isCorrect);

            if (isCorrect) {
                MergeAndSpawn(true, OnCorrect);
            } else {
                MergeAndSpawn(false, OnIncorrect);
            }
        }

        public void OnTimesUp()
        {
            OnEnd?.Invoke();
            OnLose?.Invoke();
        }
    }
}