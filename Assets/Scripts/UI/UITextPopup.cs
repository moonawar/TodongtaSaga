using TMPro;
using UnityEngine;
using DG.Tweening;

public class UITextPopup : MonoBehaviour
{
    public static UITextPopup Instance;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float yOffSet = -100f;
    private Vector2 targetPos;
    private Sequence showSequence;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        targetPos = text.rectTransform.anchoredPosition;
        text.rectTransform.anchoredPosition += new Vector2(0, yOffSet);
    }

    public void Show(string message) {
        // Stop any running animations
        if (showSequence != null) {
            showSequence.Kill();
        }

        text.text = message;
        text.gameObject.SetActive(true);
        
        // Reset alpha to full opacity
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);

        // Animate in
        showSequence = DOTween.Sequence();
        showSequence.Append(text.rectTransform.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.OutCubic));

        // Wait for 2 seconds, then animate out
        showSequence.AppendInterval(2f);

        // Animate out
        showSequence.Append(text.DOFade(0f, 0.6f));

        // Deactivate the GameObject after the animation
        showSequence.OnComplete(() => {
            text.gameObject.SetActive(false);
            text.rectTransform.anchoredPosition = targetPos + new Vector2(0, yOffSet);
            showSequence = null;
        });
    }
}
