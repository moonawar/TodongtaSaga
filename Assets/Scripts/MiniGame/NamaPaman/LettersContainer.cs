using UnityEngine;

namespace TodongtoaSaga.Minigames.NamaPaman
{
    public class LettersContainer : MonoBehaviour {
        [SerializeField] private GameObject letterDisplayPrefab;
        [SerializeField] private GameObject spaceDisplayPrefab;
        public void InitString(string s) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            foreach (char c in s) {
                if (c.Equals(' ')) {
                    Instantiate(spaceDisplayPrefab, transform);
                    continue;
                }

                GameObject letterDisplay = Instantiate(letterDisplayPrefab, transform);
                letterDisplay.GetComponent<LetterDisplay>().SetLetter(c);
            }
        }

        public void DisplayLetterFound(char c) {
            foreach (Transform child in transform) {
                LetterDisplay letterDisplay = child.GetComponent<LetterDisplay>();
                if (letterDisplay.HasLetter(c)) {
                    letterDisplay.DisplayLetter();
                }
            }
        }
    }
}