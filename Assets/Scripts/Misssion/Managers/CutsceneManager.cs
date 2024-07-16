using System;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using DG.Tweening;
public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private Image cutsceneImage;
    [SerializeField] private Image blackOverlay;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Button continueBtn;
    [SerializeField] private LineView lineView;

    private int currentImageIndex = 0;
    private Cutscene currentCutscene;
    private Tween currentFadeTween;

    private void Awake()
    {
        if (dialogueRunner == null)
        {
            Debug.LogWarning("DialogueRunner not found. Searching for one in the scene.");
            dialogueRunner = FindObjectOfType<DialogueRunner>();
        }
    }

    public void StartCutscene(Cutscene cutscene, Action onFinished = null)
    {
        dialogueRunner.Stop();
        dialogueRunner.onDialogueComplete.RemoveAllListeners();
        dialogueRunner.onDialogueComplete.AddListener(() => {
            FadeToBlack(() => {
                onFinished?.Invoke();
                dialogueRunner.onDialogueComplete.RemoveAllListeners();
            });
        });
        currentCutscene = cutscene;
        currentImageIndex = 0;

        cutsceneImage.sprite = cutscene.images[0].image;
        dialogueRunner.StartDialogue(cutscene.yarnTitle);
        FadeFromBlack();
    }

    [YarnCommand("prepareImage")]
    public void PrepareImage()
    {
        continueBtn.onClick = new Button.ButtonClickedEvent();
        continueBtn.onClick.AddListener(NextImage);
    }

    private void NextImage()
    {
        if (currentImageIndex < currentCutscene.images.Length - 1)
        {
            currentImageIndex++;
            continueBtn.enabled = false;
            FadeToBlack(() => {
                cutsceneImage.sprite = currentCutscene.images[currentImageIndex].image;
                lineView.UserRequestedViewAdvancement();
                continueBtn.onClick.RemoveAllListeners();
                continueBtn.onClick.AddListener(lineView.UserRequestedViewAdvancement);
                continueBtn.enabled = true;
                FadeFromBlack();
            });
        }
    }

    private Tween FadeToBlack(Action onComplete = null)
    {
        blackOverlay.color = new Color(0, 0, 0, 0);
        currentFadeTween = blackOverlay.DOFade(1f, fadeDuration).OnComplete(() => onComplete?.Invoke());
        return currentFadeTween;
    }

    private Tween FadeFromBlack(Action onComplete = null)
    {
        blackOverlay.color = new Color(0, 0, 0, 1);
        currentFadeTween = blackOverlay.DOFade(0f, fadeDuration).OnComplete(() => onComplete?.Invoke());
        return currentFadeTween;
    }

    public void OnSkipped()
    {
        // Remove the onComplete callback from the current fade tween
        if (currentFadeTween != null)
        {
            currentFadeTween.OnComplete(null);
            currentFadeTween.Kill(false);
        }

        // Kill all DOTween animations without calling their callbacks
        DOTween.KillAll(false);

        // Reset the black overlay to be fully transparent
        blackOverlay.color = new Color(0, 0, 0, 0);

        // Reset the continue button
        continueBtn.onClick.RemoveAllListeners();
        continueBtn.enabled = true;

        // Clear any remaining dialogue complete listeners
        dialogueRunner.onDialogueComplete.RemoveAllListeners();

        // Stop the dialogue
        dialogueRunner.Stop();

        // Reset the current image index
        currentImageIndex = 0;

        // Clear the current cutscene reference
        currentCutscene = null;
    }
}