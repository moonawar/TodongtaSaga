using UnityEngine;
using DG.Tweening;

public class SplashScreenLogoAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float scaleFrom = 1.2f;
    [SerializeField] private float tiltAngle = 5f;
    [SerializeField] private float tiltDuration = 1f;
    [SerializeField] private RectTransform[] openElements;
    [SerializeField] private RectTransform[] closeElements;
    [SerializeField] private float delayAfterTilt = 5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Sequence mainSequence;

    private void Start()
    {
        if (targetUI == null)
        {
            targetUI = GetComponent<RectTransform>();
        }

        targetUI.localScale = Vector3.one * scaleFrom;

        // Set initial states
        SetElementsActive(openElements, false);
        SetElementsActive(closeElements, true);

        // Create main sequence
        mainSequence = DOTween.Sequence();

        // Initial scale animation
        mainSequence.Append(targetUI.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));

        // Start the continuous tilt animation after scaling
        mainSequence.AppendCallback(StartTiltAnimation);

        mainSequence.AppendInterval(delayAfterTilt / 2);
        mainSequence.AppendCallback(() => {
            AudioManager.Instance.PlayBGMOverwrite("Game");
        });

        // Add delay
        mainSequence.AppendInterval(delayAfterTilt / 2);

        // Fade out close elements and activate open elements
        mainSequence.AppendCallback(() => {
            FadeOutCloseElements();
            SetElementsActive(openElements, true);
        });
    }

    private void StartTiltAnimation()
    {
        Sequence tiltSequence = DOTween.Sequence();

        tiltSequence.Append(targetUI.DORotate(new Vector3(0, 0, -tiltAngle), tiltDuration / 2).SetEase(Ease.Linear));
        tiltSequence.Append(targetUI.DORotate(new Vector3(0, 0, tiltAngle), tiltDuration).SetEase(Ease.Linear));
        tiltSequence.Append(targetUI.DORotate(Vector3.zero, tiltDuration / 2).SetEase(Ease.Linear));

        tiltSequence.SetLoops(-1, LoopType.Restart);
    }

    private void FadeOutCloseElements()
    {
        foreach (RectTransform element in closeElements)
        {
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = element.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.DOFade(0, fadeOutDuration).OnComplete(() => element.gameObject.SetActive(false));
        }
    }

    private void SetElementsActive(RectTransform[] elements, bool active)
    {
        foreach (RectTransform element in elements)
        {
            element.gameObject.SetActive(active);
        }
    }

    private void OnDestroy()
    {
        mainSequence.Kill();
        targetUI.DOKill();
        
        foreach (RectTransform element in closeElements)
        {
            DOTween.Kill(element);
        }
    }
}