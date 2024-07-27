using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookMission : MonoBehaviour {
    [SerializeField] private Image checkmark;
    [SerializeField] private TextMeshProUGUI misisonDesc;

    public void Set(Mission mission, Sprite checkmarkSprite) {
        checkmark.sprite = checkmarkSprite;
        misisonDesc.text = mission.bookMissionDescription;
    }
}