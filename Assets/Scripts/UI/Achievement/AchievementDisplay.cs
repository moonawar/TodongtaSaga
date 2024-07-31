using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplay : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    public void Set(AchievementData data) {
        icon.sprite = data.icon;
        title.text = data.title;
        description.text = data.description;
    }
}
