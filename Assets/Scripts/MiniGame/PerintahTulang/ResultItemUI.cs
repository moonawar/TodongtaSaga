using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    public class ResultItemUI : MonoBehaviour {

        [SerializeField] private Image foodImage;
        [SerializeField] private TextMeshProUGUI foodNameText;

        [SerializeField] private Image drinkImage;
        [SerializeField] private TextMeshProUGUI drinkNameText;


        public void Set(Sprite foodSprite, string foodName, Sprite drinkSprite, string drinkName) {
            foodImage.sprite = foodSprite;
            foodNameText.text = foodName;

            drinkImage.sprite = drinkSprite;
            drinkNameText.text = drinkName;
        }
    }
}