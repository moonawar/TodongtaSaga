using UnityEngine;
using Yarn.Unity;
using DG.Tweening;
using System;
using TMPro;

public class NPC : Interactable
{
    public NPCData Data;

    [Header("Hint")]
    [SerializeField] private Transform interactHint;
    [SerializeField] private float hintOffset = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float bounceAmplitude = 0.1f;
    [SerializeField] private float bounceDuration = 0.5f;

    private bool inInteractRange = false;
    private bool canInteract = false;
    private SpriteRenderer hintSpriteRenderer;
    private Vector3 hintInitialPosition;
    private Tweener bounceAnimation;
    private Tween fadeAnimation;

    private Action onInteraction;
    private bool shouldDissapear = false;
    public bool ShouldDissapear { get => shouldDissapear; set {
        shouldDissapear = value;
        if (shouldDissapear) {
            inInteractRange = false;
        }
    }}

    private void Awake()
    {
        if (!interactHint.TryGetComponent(out hintSpriteRenderer))
        {
            Debug.LogError("SpriteRenderer not found on interactHint. Please add a SpriteRenderer component.");
        }

        hintInitialPosition = interactHint.localPosition;
    }

    private void Start()
    {
        interactHint.gameObject.SetActive(false);
    }

    public override void OnInteractableEnter()
    {   
        if (canInteract) return;
        fadeAnimation?.Kill();

        interactHint.gameObject.SetActive(true);
        inInteractRange = true;

        interactHint.localPosition = hintInitialPosition - new Vector3(0, hintOffset, 0);
        Color startColor = hintSpriteRenderer.color;
        startColor.a = 0f;
        hintSpriteRenderer.color = startColor;

        fadeAnimation = DOTween.Sequence()
            .Join(hintSpriteRenderer.DOFade(1f, fadeDuration))
            .Join(interactHint.DOLocalMove(hintInitialPosition, slideDuration).SetEase(Ease.OutBack))
            .OnComplete(() => {
                fadeAnimation = null;
                StartBounceAnimation();
            });
    }

    public override void OnInteractableExit()
    {
        if (canInteract) return;
        fadeAnimation?.Kill();

        inInteractRange = false;

        bounceAnimation?.Kill();

        fadeAnimation = DOTween.Sequence()
            .Join(hintSpriteRenderer.DOFade(0f, fadeDuration))
            .Join(interactHint.DOLocalMove(hintInitialPosition - new Vector3(0, hintOffset, 0), slideDuration).SetEase(Ease.InBack))
            .OnComplete(() => {
                interactHint.gameObject.SetActive(false); 
                fadeAnimation = null;
            });
    }

    private void StartBounceAnimation()
    {
        bounceAnimation = interactHint
            .DOLocalMoveY(hintInitialPosition.y + bounceAmplitude, bounceDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void AddInteractionListener(Action action)
    {
        canInteract = false;
        onInteraction += action;
    }

    public void RemoveInteractionListener(Action action)
    {
        onInteraction -= action;
    }

    public void ClearInteractionListeners()
    {
        canInteract = true;
        onInteraction = null;
    }

    public override void Interact()
    {
        if (!inInteractRange) return;
        onInteraction?.Invoke();
        OnInteractableExit();
        inInteractRange = false;
        ClearInteractionListeners();
    }

    public void Dissapear() {
        gameObject.SetActive(false);
        shouldDissapear = false;
    }

    private void OnBecameInvisible() {
        if (shouldDissapear) {
            Dissapear();
        }
    }
}