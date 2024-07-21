using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class RecipePickupPoint : Interactable {
    [Header("Hint")]
    [SerializeField] private Transform interactHint;
    [SerializeField] private float bounceAmplitude = 0.1f;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private Sprite nonInteractableSprite;
    [SerializeField] private Sprite interactableSprite;
    private SpriteRenderer spriteRenderer;

    private Vector3 hintInitialPosition;

    [SerializeField] private UnityEvent onInteraction;

    private bool interactable = false;

    private void Awake()
    {
        hintInitialPosition = interactHint.localPosition;
        spriteRenderer = interactHint.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartBounceAnimation();
        interactable = false;
        spriteRenderer.sprite = nonInteractableSprite;
    }

    public override void OnInteractableEnter() { 
        interactable = true;
        spriteRenderer.sprite = interactableSprite;
    }
    public override void OnInteractableExit() {
        interactable = false;
        spriteRenderer.sprite = nonInteractableSprite;
    }

    private void StartBounceAnimation()
    {
        interactHint
            .DOLocalMoveY(hintInitialPosition.y + bounceAmplitude, bounceDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public override void Interact()
    {
        if (interactable) onInteraction?.Invoke();
    }
}