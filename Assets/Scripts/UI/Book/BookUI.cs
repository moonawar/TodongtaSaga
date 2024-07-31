using UnityEngine;

public class BookUI : MonoBehaviour
{
    [SerializeField] private RectTransform personInfoUI;
    [SerializeField] private RectTransform familyTreeUI;
    [SerializeField] private RectTransform achievementUI;

    public void OpenPersonInfo() {
        personInfoUI.gameObject.SetActive(true);
        familyTreeUI.gameObject.SetActive(false);
        achievementUI.gameObject.SetActive(false);
    }

    public void OpenFamilyTree() {
        personInfoUI.gameObject.SetActive(false);
        familyTreeUI.gameObject.SetActive(true);
        achievementUI.gameObject.SetActive(false);
    }

    public void OpenAchievement() {
        personInfoUI.gameObject.SetActive(false);
        familyTreeUI.gameObject.SetActive(false);
        achievementUI.gameObject.SetActive(true);
    }
}
