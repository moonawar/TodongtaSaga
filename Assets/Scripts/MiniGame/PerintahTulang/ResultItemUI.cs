using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    public class ResultItemUI : MonoBehaviour {

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI itemNameText;

        public void Set(Sprite sprite, string itemName) {
            image.sprite = sprite;
            itemNameText.text = itemName;
        }
    }
}