using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MissionAnnouncer : MonoBehaviour
{
    [SerializeField] private RectTransform announcementPanel;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private float yOffset = 100f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private CanvasGroup canvasGroup;

    public static MissionAnnouncer Instance;

    private Vector2 from;
    private Vector2 to;
    private Queue<(string, string)> announcementQueue = new Queue<(string, string)>();
    private bool isDisplaying = false;

    private string lastMissionTitle;
    private string lastMissionDescription;

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
        to = announcementPanel.anchoredPosition;
        from = new Vector2(to.x, to.y - yOffset);
        announcementPanel.gameObject.SetActive(false);

        if (canvasGroup == null)
        {
            canvasGroup = announcementPanel.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void AnnounceMission(string missionTitle, string missionDescription)
    {
        // omit this.
        if (missionTitle == null || missionTitle.Length == 0 || missionDescription == null || missionDescription.Length == 0)
        {
            return;
        }

        announcementQueue.Enqueue((missionTitle, missionDescription));
        if (!isDisplaying)
        {
            StartCoroutine(DisplayAnnouncementsCoroutine());
        }

        lastMissionTitle = missionTitle;
        lastMissionDescription = missionDescription;
    }

    public void ReannounceMission()
    {
        AnnounceMission(lastMissionTitle, lastMissionDescription);
    }

    private IEnumerator DisplayAnnouncementsCoroutine()
    {
        isDisplaying = true;

        while (announcementQueue.Count > 0)
        {
            var (missionTitle, missionDescription) = announcementQueue.Dequeue();
            yield return StartCoroutine(ShowAnnouncement(missionTitle, missionDescription));
        }

        isDisplaying = false;
    }

    private IEnumerator ShowAnnouncement(string missionTitle, string missionDescription)
    {
        title.text = missionTitle;
        description.text = missionDescription;

        yield return StartCoroutine(AnimateIn());
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(AnimateOut());
    }

    private IEnumerator AnimateIn()
    {
        announcementPanel.anchoredPosition = from;
        canvasGroup.alpha = 0f;
        announcementPanel.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(announcementPanel.DOAnchorPosY(to.y, animationDuration).SetEase(Ease.OutBack));
        sequence.Join(canvasGroup.DOFade(1f, animationDuration).SetEase(Ease.InOutSine));

        yield return sequence.WaitForCompletion();
    }

    private IEnumerator AnimateOut()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(announcementPanel.DOAnchorPosY(from.y, animationDuration).SetEase(Ease.InBack));
        sequence.Join(canvasGroup.DOFade(0f, animationDuration).SetEase(Ease.InOutSine));

        yield return sequence.WaitForCompletion();
        announcementPanel.gameObject.SetActive(false);
    }
}