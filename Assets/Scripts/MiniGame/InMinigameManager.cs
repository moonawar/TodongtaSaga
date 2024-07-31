using UnityEngine;
using DG.Tweening;
using System;

public class InMinigameManager : MonoBehaviour
{
    public static InMinigameManager Instance;

    [SerializeField] private RectTransform winningScreen;
    [SerializeField] private RectTransform losingScreen;
    [SerializeField] private float zoomOutDuration = 2f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float targetOrthoSize = 10f;
    [SerializeField] private float slowMoDuration = 2f; // Duration of slow-mo effect
    [SerializeField] private float endSlowMoTimeScale = 0.1f; // Final time scale (0.1 = 10% speed)

    private Camera mainCamera;
    public bool GameEnded { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        mainCamera = Camera.main;
    }

    private void Start() {
        AudioManager.Instance.PlayBGMOverwrite("Minigame");
    }

    public void WinMinigame() {
        WinMinigame(null);
    }

    public void WinMinigame(Action onZoomEndedOverride = null)
    {
        GameEnded = true;
        StartEndSequence(winningScreen, onZoomEndedOverride);
    }

    public void LoseMinigame() {
        LoseMinigame(null);
    }

    public void LoseMinigame(Action onZoomEndedOverride = null)
    {
        GameEnded = true;
        StartEndSequence(losingScreen, onZoomEndedOverride);
    }

    private void StartEndSequence(RectTransform endScreen, Action onZoomEndedOverride = null)
    {
        // Start zoom and slow-mo
        AnimateCameraZoomOut();
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, endSlowMoTimeScale, slowMoDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                if (onZoomEndedOverride != null) {
                    onZoomEndedOverride();
                    return;
                }

                // After zoom and slow-mo, fade in the end screen and end the game
                FadeInScreen(endScreen);
                AudioManager.Instance.StopBGMCrossfade();
                if (endScreen == losingScreen)
                {
                    AudioManager.Instance.PlaySFX("GameOver");
                }
                GameStateManager.Instance.ToOpenUIOverride(); // Tell GameStateManager to open UI on top
                // Reset time scale
                Time.timeScale = 1f;
            });
    }

    private void AnimateCameraZoomOut()
    {
        if (mainCamera != null && mainCamera.orthographic)
        {
            mainCamera.DOOrthoSize(targetOrthoSize, zoomOutDuration).SetEase(Ease.OutSine);
        }
    }

    private void FadeInScreen(RectTransform screen)
    {
        if (!screen.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = screen.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        screen.gameObject.SetActive(true);
        canvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad);
    }

    public void WinAndBackToMap() {
        // SceneLoader.Instance.LoadScene("Gameplay", MissionManager.Instance.ExecuteNextTask);
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