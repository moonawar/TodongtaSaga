using UnityEngine;
using DG.Tweening;

public class ObjectColorBlink : MonoBehaviour
{
    public float TotalBlinkDuration { get => totalBlinkDuration; }

    [SerializeField] private float totalBlinkDuration = 1.2f;
    [SerializeField] private float blinkPeriod = 0.3f;
    [SerializeField] private Color blinkColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Sequence blinkSequence;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
        }
    }

    public void StartBlinking()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject.");
            return;
        }

        StopBlinking();

        blinkSequence = DOTween.Sequence();

        blinkSequence.Append(spriteRenderer.DOColor(blinkColor, blinkPeriod / 2))
                     .Append(spriteRenderer.DOColor(Color.white, blinkPeriod / 2))
                     .SetLoops(-1)
                     .SetEase(Ease.Linear);

        blinkSequence.OnComplete(StopBlinking);

        if (totalBlinkDuration > 0)
        {
            blinkSequence.SetDelay(0).SetLoops(-1).SetEase(Ease.Linear);
            DOVirtual.DelayedCall(totalBlinkDuration, StopBlinking);
        }
    }

    public void StopBlinking()
    {
        if (blinkSequence != null)
        {
            blinkSequence.Kill();
            blinkSequence = null;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void OnDisable()
    {
        StopBlinking();
    }
}