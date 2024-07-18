using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class RecipeSubmitPoint : Interactable {
    [Header("Hint")]
    [SerializeField] private Transform interactHint;
    [SerializeField] private float bounceAmplitude = 0.1f;
    [SerializeField] private float bounceDuration = 0.5f;

    private Vector3 hintInitialPosition;

    [SerializeField] private UnityEvent onInteraction;

    private void Awake()
    {
        hintInitialPosition = interactHint.localPosition;
    }

    private void Start()
    {
        StartBounceAnimation();
    }

    public override void OnInteractableEnter() { }
    public override void OnInteractableExit() { }

    private void StartBounceAnimation()
    {
        interactHint
            .DOLocalMoveY(hintInitialPosition.y + bounceAmplitude, bounceDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public override void Interact()
    {
        onInteraction?.Invoke();
    }
}