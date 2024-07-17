using TMPro;
using UnityEngine;
using DG.Tweening;

public class CatchStatusSpawner : MonoBehaviour
{
    [SerializeField] private TextMeshPro statusText; // Changed to TextMeshProUGUI for UI elements
    [SerializeField] private float duration = 2f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Ease movementEase = Ease.OutBack;
    [SerializeField] private Vector2 initialPosition = new Vector2(0f, 0f);

    private Vector2 endLocalPosition;
    private RectTransform rectTransform;
    private Sequence sequence;

    private void Awake()
    {
        rectTransform = statusText.GetComponent<RectTransform>();
        endLocalPosition = rectTransform.anchoredPosition; // Use anchoredPosition for RectTransform

        statusText.GetComponent<MeshRenderer>().sortingOrder = 10; // Set the sorting order of the text
        statusText.gameObject.SetActive(false);
    }

    public void SpawnStatusText(string status)
    {
        // Kill any ongoing tweens on the rectTransform and statusText to prevent interference
        sequence?.Kill();

        // Set the text and make the object active
        statusText.text = status;
        statusText.gameObject.SetActive(true);
        statusText.rectTransform.anchoredPosition = initialPosition;
        
        // Reset the text color to fully visible
        Color startColor = statusText.color;
        startColor.a = 1f;
        statusText.color = startColor;

        // Move and fade the text
        AnimateStatusText();
    }

    private void AnimateStatusText()
    {
        // Create a sequence for moving and fading the text
        sequence = DOTween.Sequence();

        // Move the text to the end local position
        sequence.Append(rectTransform.DOAnchorPos(endLocalPosition, duration).SetEase(movementEase));

        // Fade out the text during the last portion of the movement
        sequence.Insert(duration - fadeDuration, statusText.DOFade(0f, fadeDuration));

        // Deactivate the text object after the animation is complete
        sequence.OnComplete(() => statusText.gameObject.SetActive(false));
    }

    private void OnDestroy() {
        DOTween.KillAll();
    }
}
