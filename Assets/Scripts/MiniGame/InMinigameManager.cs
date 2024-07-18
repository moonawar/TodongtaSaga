using UnityEngine;
using DG.Tweening;

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

    public void WinMinigame()
    {
        StartEndSequence(winningScreen);
    }

    public void LoseMinigame()
    {
        StartEndSequence(losingScreen);
    }

    private void StartEndSequence(RectTransform endScreen)
    {
        // Start zoom and slow-mo
        AnimateCameraZoomOut();
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, endSlowMoTimeScale, slowMoDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                // After zoom and slow-mo, fade in the end screen and end the game
                FadeInScreen(endScreen);
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