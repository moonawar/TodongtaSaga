using UnityEngine;

namespace TodongtoaSaga.Minigames.NamaPaman
{
    public class InGameLetter : MonoBehaviour {
        [SerializeField] private char representedLetter;
        // Called By Button
        public void ThisLetterFound() {
            GameManager.Instance.LetterFound(representedLetter);
            gameObject.SetActive(false);
        }
    }
}