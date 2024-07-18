using TMPro;
using UnityEngine;

namespace TodongtoaSaga.Minigames.NameGame {
    public class CharacterDisplay : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI characterText;
        public char Character { get; private set; }
        public void SetCharacter(char c) {
            Character = c;
            characterText.text = c.ToString();
            characterText.gameObject.SetActive(false);
        }

        public bool ContainCharacter(char c) {
            return Character == c;
        }

        public void DisplayCharacter() {
            characterText.gameObject.SetActive(true);
        }
    }
}