using TMPro;
using UnityEngine;

namespace TodongtoaSaga.Minigames.NamaPaman 
{
    public class LetterDisplay : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI letterText;
        public char Letter { get; private set; }
        public void SetLetter(char c) {
            Letter = c;
            letterText.text = c.ToString();
            letterText.gameObject.SetActive(false);
        }

        public bool HasLetter(char c) {
            return Letter.Equals(c);
        }

        public void DisplayLetter() {
            letterText.gameObject.SetActive(true);
        }
    }
}