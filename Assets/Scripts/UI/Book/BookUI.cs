using UnityEngine;

public class BookUI : MonoBehaviour
{
    [SerializeField] private RectTransform personInfoUI;
    [SerializeField] private RectTransform familyTreeUI;

    public void OpenPersonInfo() {
        personInfoUI.gameObject.SetActive(true);
        familyTreeUI.gameObject.SetActive(false);
    }

    public void OpenFamilyTree() {
        personInfoUI.gameObject.SetActive(false);
        familyTreeUI.gameObject.SetActive(true);
    }
}
