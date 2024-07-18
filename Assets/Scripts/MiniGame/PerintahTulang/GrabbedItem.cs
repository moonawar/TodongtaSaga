using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    public class GrabbedItem : MonoBehaviour {

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI itemNameText;

        private PlayerItemGrabber playerItemGrabber;
        public void Init(PlayerItemGrabber playerItemGrabber) {
            this.playerItemGrabber = playerItemGrabber;
        }

        public void Set(Sprite sprite, string itemName) {
            image.sprite = sprite;
            itemNameText.text = itemName;
        }

        public void Remove() {
            playerItemGrabber.RemoveItem(transform.GetSiblingIndex());
            Destroy(gameObject);
        }
    }
}