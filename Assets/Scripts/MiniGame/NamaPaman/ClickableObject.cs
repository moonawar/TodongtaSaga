using UnityEngine;

namespace TodongtoaSaga.Minigames.NamaPaman
{
    public class ClickableObject : MonoBehaviour
    {
        [SerializeField] private GameObject afterClick;

        // Called by Button
        public void OnClick() {
            gameObject.SetActive(false);
            afterClick.SetActive(true);
        }
    }
}