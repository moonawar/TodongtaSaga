using UnityEngine;
using UnityEngine.UI;

public class AmangUdaUIMeter : MonoBehaviour
{
    // [SerializeField] private Sprite[] angerSprites; // later;
    [SerializeField] private Sprite[] angerSprites;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void UpdateMeter(int angerMeter) {
        image.sprite = angerSprites[angerMeter];
    }
}
