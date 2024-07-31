using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PenyelamatanBoras
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private Tikus tikus;

        [Header("UI")]
        [HorizontalLine(color: EColor.Gray)]
        [SerializeField] private AmangUdaUIMeter angerMeterUI;
        [SerializeField] private TikusHealthUI tikusHealthUI;
        [SerializeField] private RectTransform winningScreen;
        [SerializeField] private RectTransform losingScreen;

        [Header("Cutscene")]
        [HorizontalLine(color: EColor.Gray)]
        [SerializeField] private Cutscene winningCutscene;
        [SerializeField] private Cutscene losingCutscene;

        private Image blackOverlay;
        private CanvasGroup cutsceneCanvasGroup;
        private readonly string[] angerSounds = { "Hmm1", "Hmm2" };

        public int TikusHealth { get; private set; } = 3;
        public int AngerMeter { get; private set; } = 0;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            GameStateManager.Instance.ToOpenUI();
        }

        private void Start() {
            blackOverlay = CutsceneManager.Instance.BlackOverlay;
            cutsceneCanvasGroup = CutsceneManager.Instance.CutsceneCanvasGroup;
        }

        public void IncrementAngerMeter()
        {   
            if (InMinigameManager.Instance.GameEnded) return;
            if (AngerMeter >= 3) return;

            string sound = angerSounds[UnityEngine.Random.Range(0, angerSounds.Length)];
            AudioManager.Instance.PlaySFX(sound);
            
            AngerMeter++;
            angerMeterUI.UpdateMeter(AngerMeter);
            if (AngerMeter >= 3)
            {               
                InMinigameManager.Instance.LoseMinigame(
                () => {
                    // OnZoomEnded
                    Time.timeScale = 1f;
                    AudioManager.Instance.StopBGMCrossfade();
                    cutsceneCanvasGroup.alpha = 0;
                    FadeToBlack(() => {
                        cutsceneCanvasGroup.alpha = 1;
                        CutsceneManager.Instance.StartCutscene(losingCutscene, () => {
                            GameStateManager.Instance.ToOpenUI();
                            losingScreen.gameObject.SetActive(true);
                            AudioManager.Instance.PlaySFX("GameOver");
                            FadeFromBlack();
                        });
                    });
                });
            }
        }

        public void DecrementTikusHealth()
        {
            if (InMinigameManager.Instance.GameEnded) return;
            if (TikusHealth <= 0) return;

            TikusHealth--;
            tikusHealthUI.UpdateHealth(TikusHealth);
            if (TikusHealth <= 0)
            {
                tikus.Die();

                InMinigameManager.Instance.WinMinigame(
                () => {
                    // OnZoomEnded
                    Time.timeScale = 1f;
                    AudioManager.Instance.StopBGMCrossfade();
                    cutsceneCanvasGroup.alpha = 0;
                    FadeToBlack(() => {
                        cutsceneCanvasGroup.alpha = 1;
                        CutsceneManager.Instance.StartCutscene(winningCutscene, () => {
                            GameStateManager.Instance.ToOpenUI();
                            winningScreen.gameObject.SetActive(true);
                            FadeFromBlack();
                        });
                    });
                });
            }
        }

        private void FadeToBlack(Action onComplete = null)
        {
            blackOverlay.color = new Color(0, 0, 0, 0);
            blackOverlay.DOFade(1f, 0.8f).OnComplete(() => onComplete?.Invoke());
        }
        private void FadeFromBlack(Action onComplete = null)
        {
            blackOverlay.color = new Color(0, 0, 0, 1);
            blackOverlay.DOFade(0f, 0.8f).OnComplete(() => onComplete?.Invoke());
        }
    }
}