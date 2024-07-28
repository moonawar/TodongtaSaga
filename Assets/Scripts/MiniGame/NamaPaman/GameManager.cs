using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace TodongtoaSaga.Minigames.NamaPaman
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [Header("Data")]
        [HorizontalLine(color: EColor.Gray)]
        [SerializeField] private string characterName;
        private Dictionary<char, int> nameLettersCount;
        private int lettersCount;

        [Header("Character UI")]
        [HorizontalLine(color: EColor.Gray)]
        [SerializeField] private RectTransform lettersParent;
        [SerializeField] private LettersContainer lettersContainer;
        [SerializeField] private CountdownTimer countdownTimer;

        [Header("Ending")]
        [HorizontalLine(color: EColor.Gray)]
        [SerializeField] private RectTransform winningScreen;
        [SerializeField] private RectTransform losingScreen;
        public bool GameEnded { get; private set; } = false;    

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            GameStateManager.Instance.ToOpenUI();
            characterName = characterName.ToUpper();
            InitializeNameLettersCount();
        }    

        private void Start() {
            AudioManager.Instance.PlayBGMOverwrite("Minigame");
        }

        public void StartGame() {
            countdownTimer.StartTimer();
            lettersContainer.InitString(characterName);
        }

        private void InitializeNameLettersCount()
        {
            nameLettersCount = new Dictionary<char, int>();

            foreach (char letter in characterName)
            {
                if (letter.Equals(' ')) continue;
                if (!nameLettersCount.ContainsKey(letter))
                {
                    nameLettersCount.Add(letter, 1);
                }
            }

            lettersCount = nameLettersCount.Count;
        }

        public void TimesUp()
        {
            // Lose
            LoseMinigame();
        }

        public void LetterFound(char c)
        {
            nameLettersCount[c]--;

            if (nameLettersCount[c] == 0)
            {
                lettersCount--;
                lettersContainer.DisplayLetterFound(c);
            }

            if (lettersCount == 0)
            {
                // Win
                WinMinigame();
            }
        }

        public void WinMinigame()
        {
            GameEnded = true;
            countdownTimer.PauseTimer();
            StartEndSequence(winningScreen);
        }

        public void LoseMinigame()
        {
            GameEnded = true;
            countdownTimer.PauseTimer();
            StartEndSequence(losingScreen);
        }

        private void StartEndSequence(RectTransform endScreen)
        {
            // Store the current world position
            Vector3 worldPosition = lettersParent.position;

            // Change the anchor points
            lettersParent.anchorMin = new Vector2(0.5f, 0.5f);
            lettersParent.anchorMax = new Vector2(0.5f, 0.5f);

            // Set the position back to the original world position
            lettersParent.position = worldPosition;

            // Animate to the center
            lettersParent.DOAnchorPos(Vector2.zero, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    AudioManager.Instance.StopBGMCrossfade();
                    GameStateManager.Instance.ToOpenUIOverride(); // Tell GameStateManager to open UI on top
                    FadeInScreen(endScreen);
                });
            });
        }

        private void FadeInScreen(RectTransform screen)
        {
            if (!screen.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = screen.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            screen.gameObject.SetActive(true);
            canvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad);
        }

        public void WinAndBackToMap() {
            MissionManager.Instance.ExecuteNextTask();
        }

        public void LoseAndBackToMap() {
            SaveManager.Instance.LoadGame();
        }

        public void Restart() {
            SceneLoader.Instance.RestartScene();
        }    
        
        public void ToMinigamePlaying()
        {
            GameStateManager.Instance.ToGameplayOverride();
        }

        private void OnDestroy() {
            DOTween.KillAll();
        }
    }

}