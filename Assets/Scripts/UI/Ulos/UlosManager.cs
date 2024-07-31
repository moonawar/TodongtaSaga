using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public enum UlosAction
{
    UnlockBoru,
    UnlockTubu,
    UnlockHula,
    DisappearAll,
    InitializeUlos
}

public class UlosManager : MonoBehaviour
{
    public static UlosManager Instance;

    [SerializeField] private RectTransform ulosUI;
    [SerializeField] private RectTransform ulosButton;
    [SerializeField] private RectTransform boru;
    [SerializeField] private RectTransform tubu;
    [SerializeField] private RectTransform hula;
    [SerializeField] private CanvasGroup badgesCanvasGroup;
    [SerializeField] private Button exitButton;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float blinkDuration = 0.3f;
    [SerializeField] private int blinkCount = 3;

    public bool UlosInitialized { 
        get => ulosButton.gameObject.activeSelf;
        set => ulosButton.gameObject.SetActive(value);
    }
    public bool[] Progress {
        get => new bool[] {boru.gameObject.activeSelf, tubu.gameObject.activeSelf, hula.gameObject.activeSelf};
        set {
            boru.gameObject.SetActive(value[0]);
            tubu.gameObject.SetActive(value[1]);
            hula.gameObject.SetActive(value[2]);
        }
    }

    private List<RectTransform> allBadges;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        allBadges = new List<RectTransform> { boru, tubu, hula };
    }

    public void ExecuteUlosAction(UlosAction action)
    {
        switch (action)
        {
            case UlosAction.UnlockBoru:
                UnlockUlos(boru);
                break;
            case UlosAction.UnlockTubu:
                UnlockUlos(tubu);
                break;
            case UlosAction.UnlockHula:
                UnlockUlos(hula);
                break;
            case UlosAction.DisappearAll:
                DisappearAll();
                break;
            case UlosAction.InitializeUlos:
                UlosInitialized = true;
                break;
        }
    }

    private void DisappearAll()
    {
        ulosUI.gameObject.SetActive(true);
        exitButton.interactable = false;
        Sequence disappearSequence = DOTween.Sequence();

        disappearSequence.Append(CreateBlinkSequence());

        disappearSequence.OnComplete(() => {
            foreach (var badge in allBadges)
            {
                badge.gameObject.SetActive(false);
            }
            exitButton.interactable = true;
            exitButton.onClick.AddListener(() => {
                MissionManager.Instance.ExecuteNextTask();
                exitButton.onClick.RemoveAllListeners();
            });
        });
    }

    private void UnlockUlos(RectTransform badge)
    {
        exitButton.interactable = false;
        badge.gameObject.SetActive(true);
        badgesCanvasGroup.alpha = 0f;

        badgesCanvasGroup.DOFade(1f, fadeDuration).OnComplete(() => {
            exitButton.interactable = true;
        });
    }

    private Sequence CreateBlinkSequence()
    {
        Sequence blinkSequence = DOTween.Sequence();

        for (int i = 0; i < blinkCount; i++)
        {
            blinkSequence.Append(badgesCanvasGroup.DOFade(0f, blinkDuration / 2));
            blinkSequence.Append(badgesCanvasGroup.DOFade(1f, blinkDuration / 2));
        }

        blinkSequence.Append(badgesCanvasGroup.DOFade(0f, blinkDuration));

        return blinkSequence;
    }

    private void OnDestroy()
    {
        DOTween.Kill(badgesCanvasGroup);
    }
}