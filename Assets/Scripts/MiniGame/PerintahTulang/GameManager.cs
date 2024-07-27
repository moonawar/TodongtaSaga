using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using TodongtoaSaga.Minigames.PenyelamatanBoras;
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

    [Serializable]
    public class Level
    {
        public ResultItem[] items;
        public int timeLimit;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private Level[] levels;
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
        [SerializeField] private RectTransform grabPanel;

        [Header("Result")]
        [SerializeField] private RectTransform resultUI;
        [SerializeField] private GameObject resultUIPrefab;
        [SerializeField] private Image resultStatusImage;
        [SerializeField] private Sprite statusCorrect;
        [SerializeField] private Sprite statusIncorrect;
        [SerializeField] private StatusSpawner statusTextSpawner;

        [Header("Events")]
        [SerializeField] private UnityEvent OnEnd;
        [SerializeField] private UnityEvent OnLose;
        [SerializeField] private UnityEvent OnWin;

        private ResultItem currentTarget;
        private Vector2 grabPanelOriginalPos;
        private GameObject resultPrefab;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            GameStateManager.Instance.ToOpenUI(); // Tell GameStateManager to open UI exclusively
            grabPanelOriginalPos = grabPanel.anchoredPosition;
        }

        public void DisplayPerintah() 
        {
            GameStateManager.Instance.ToOpenUI(); // Tell GameStateManager to open UI on top

            gameplayUI.gameObject.SetActive(false);
            perintahUI.gameObject.SetActive(true);

            perintahText.text = $"Perintah {currentIdx + 1}/{levels.Length}";
            
            // Clear the container
            foreach (Transform child in perintahItemContainer) {
                child.gameObject.SetActive(false);
            }

            // Add the items
            Level currentLevel = levels[currentIdx];
            currentTarget = currentLevel.items[UnityEngine.Random.Range(0, currentLevel.items.Length)];

            for (int i = 0; i < currentTarget.recipe.Length; i++) {
                var itemObj = i < perintahItemContainer.childCount ? 
                    perintahItemContainer.GetChild(i).gameObject : 
                    Instantiate(perintahItemPrefab, perintahItemContainer);

                itemObj.GetComponent<Image>().sprite = currentTarget.recipe[i].itemSprite;
                itemObj.GetComponentInChildren<TextMeshProUGUI>().text = currentTarget.recipe[i].name;
                itemObj.SetActive(true);
            }
        
            // foreach (var item in currentTarget.recipe) {
            //     var itemObj = Instantiate(perintahItemPrefab, perintahItemContainer);
            //     itemObj.GetComponent<Image>().sprite = item.itemSprite;
            //     itemObj.GetComponentInChildren<TextMeshProUGUI>().text = item.name;
            // }

            countdownTimer.SetDuration(currentLevel.timeLimit);
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
            if (playerItemGrabber.GrabbedItems.Count != currentTarget.recipe.Length) {
                Debug.Log("Incorrect because grabbed items count " + playerItemGrabber.GrabbedItems.Count + " != " + levels.Length);
                return false;
            }

            for (int i = 0; i < playerItemGrabber.GrabbedItems.Count; i++) {
                if (playerItemGrabber.GrabbedItems[i].name != currentTarget.recipe[i].name) {
                    Debug.Log("Incorrect because " + playerItemGrabber.GrabbedItems[i].name + " != " 
                        + currentTarget.name + " at index " + i);
                    return false;
                }
            }

            return true;
        }

        private void OnCorrect() {
            Destroy(resultPrefab);
            grabPanel.anchoredPosition = grabPanelOriginalPos;
            playerItemGrabber.GrabbedItems.Clear();
            // Destroy all items
            foreach (Transform child in grabbedItemContainer) {
                Destroy(child.gameObject);
            }                            

            grabbedItemContainer.gameObject.SetActive(true);
            resultUI.gameObject.SetActive(false);

            // if succesful
            if (currentIdx == levels.Length - 1) {
                OnWin?.Invoke();
            } else {
                currentIdx++;
                countdownTimer.ResetTimer();
                DisplayPerintah();
            }
        }

        // private bool[] PointOutMistakes() {
        //     int minLength = Mathf.Min(playerItemGrabber.GrabbedItems.Count, currentTarget.recipe.Length);

        //     bool[] mistakes = new bool[minLength];
        //     for (int i = 0; i < minLength; i++) {
        //         if (playerItemGrabber.GrabbedItems[i].name != currentTarget.recipe[i].name) {
        //             mistakes[i] = true;
        //         } else {
        //             mistakes[i] = false;
        //         }
        //     }

        //     return mistakes;
        // }

        private void OnIncorrect() {
            playerItemGrabber.GrabbedItems.Clear();
            Lose();
        }

        public void MergeAndSpawn(bool isCorrect, Action onComplete)
        {
            int completedMerges = 0;
            float fadeDuration = 0.8f;
            float mergeDuration = 0.85f;
            float panelMoveDuration = 1f;

            List<RectTransform> uiElementsToMerge = new();
            foreach (Transform child in grabbedItemContainer)
            {
                uiElementsToMerge.Add(child.GetComponent<RectTransform>());
            }

            RectTransform mergePoint = grabbedItemContainer;
            HorizontalLayoutGroup layoutGroup = grabbedItemContainer.GetComponent<HorizontalLayoutGroup>();
            layoutGroup.enabled = false;


            // Move grabPanel to the center first
            grabPanel.DOAnchorPosY(0, panelMoveDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                foreach (var element in uiElementsToMerge)
                {
                    // Fade out
                    element.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration);

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
                            resultPrefab = Instantiate(resultUIPrefab, resultUI);
                            resultPrefab.GetComponent<ResultItemUI>().Set(
                                currentTarget.foodSprite, 
                                currentTarget.foodName,
                                currentTarget.drinkSprite,
                                currentTarget.drinkName
                            );

                            // Set status image
                            resultStatusImage.sprite = isCorrect ? statusCorrect : statusIncorrect;

                            // Add delay before onComplete
                            DOTween.Sequence().AppendInterval(3f).OnComplete(() =>
                            {
                                layoutGroup.enabled = true;
                                onComplete?.Invoke();
                            });
                        }
                    });
                }
            });
        }

        public void TryMerge() {
            if (playerItemGrabber.GrabbedItems.Count == 0) {
                statusTextSpawner.SpawnStatusText("Nampan Kosong!");
                return;
            }

            countdownTimer.PauseTimer();

            bool isCorrect = CompareResult();
            Debug.Log("Is correct: " + isCorrect);

            if (isCorrect) {
                MergeAndSpawn(true, OnCorrect);
            } else {
                MergeAndSpawn(false, OnIncorrect);
            }
        }

        public void Lose()
        {
            OnEnd?.Invoke();
            OnLose?.Invoke();
        }
    }
}