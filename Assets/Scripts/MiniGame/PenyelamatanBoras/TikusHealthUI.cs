using UnityEngine;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PenyelamatanBoras
{
    public class TikusHealthUI : MonoBehaviour
    {
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;

        private Image[] healthImages;

        private void Awake()
        {
            // Get all Image components from child objects
            healthImages = GetComponentsInChildren<Image>();
            for (int i = 0; i < healthImages.Length; i++)
            {
                healthImages[i].sprite = activeSprite;
            }
        }

        public void UpdateHealth(int health)
        {
            healthImages[health].sprite = inactiveSprite;
        }
    }
}