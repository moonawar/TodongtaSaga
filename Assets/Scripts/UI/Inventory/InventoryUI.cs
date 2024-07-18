using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform foodUI;
    [SerializeField] private RectTransform clothUI;
    [SerializeField] private RectTransform objectUI;

    public void OpenFoodUI()
    {
        foodUI.gameObject.SetActive(true);
        clothUI.gameObject.SetActive(false);
        objectUI.gameObject.SetActive(false);
    }

    public void OpenClothUI()
    {
        foodUI.gameObject.SetActive(false);
        clothUI.gameObject.SetActive(true);
        objectUI.gameObject.SetActive(false);
    }

    public void OpenObjectUI()
    {
        foodUI.gameObject.SetActive(false);
        clothUI.gameObject.SetActive(false);
        objectUI.gameObject.SetActive(true);
    }
}
