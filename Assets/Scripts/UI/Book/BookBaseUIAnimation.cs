using UnityEngine;
using DG.Tweening;

public class BookBaseUIAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float yOffset = -100f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easing = Ease.OutBack;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        // Set the initial position below the screen
        rectTransform.anchoredPosition = originalPosition + new Vector2(0, yOffset);

        // Animate to the original position
        rectTransform.DOAnchorPos(originalPosition, duration)
            .SetEase(easing)
            .SetUpdate(true); // This ensures the animation runs even if the game is paused
    }

    private void OnDisable()
    {
        // Kill the tween when the object is disabled to prevent any conflicts
        rectTransform.DOKill();
    }
}