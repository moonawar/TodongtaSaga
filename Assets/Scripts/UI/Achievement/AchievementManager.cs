using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [Header("Achievement UI")]
    [SerializeField] private RectTransform[] hideOnAchievement;
    [SerializeField] private RectTransform achievementUI;
    [SerializeField] private AchievementDisplay display;

    [Header("Achievement Data")]
    public List<AchievementData> achievements;
    public static AchievementManager Instance;

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
    }

    public void UnlockAchievement(string id)
    {
        AchievementData achievement = achievements.Find(a => a.id == id);
        if (achievement != null)
        {
            achievement.unlocked = true;
            ShowAchievement(achievement);
        }
    }

    private void ShowAchievement(AchievementData achievement)
    {
        foreach (RectTransform rect in hideOnAchievement)
        {
            rect.gameObject.SetActive(false);
        }

        achievementUI.gameObject.SetActive(true);
        display.Set(achievement);
        AudioManager.Instance.PlaySFX("Gain");
    }

    public bool isUnlocked(string id)
    {
        return achievements.Find(a => a.id == id).unlocked;
    }

    public void RedisplayHiddenRects() {
        foreach (RectTransform rect in hideOnAchievement)
        {
            rect.gameObject.SetActive(true);
        }
    }
}
