using DG.Tweening;
using UnityEngine;

public class UlosUIAnimation : MonoBehaviour
{
    [SerializeField] private float offsetY = 70f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private float duration = 0.5f;
    private RectTransform rectTransform;
    private Tween tween;
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        rectTransform.anchoredPosition = new Vector2(0, offsetY);
        tween = rectTransform.DOAnchorPosY(0, duration).SetEase(ease);
    }

    private void OnDisable() {
        tween.Kill();
    }
}
