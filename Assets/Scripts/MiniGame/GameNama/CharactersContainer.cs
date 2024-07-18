using UnityEngine;

namespace TodongtoaSaga.Minigames.NameGame
{
    public class CharactersContainer : MonoBehaviour {
        [SerializeField] private GameObject characterDisplayPrefab;
        public void InitString(string s) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            foreach (char c in s) {
                GameObject characterDisplay = Instantiate(characterDisplayPrefab, transform);
                characterDisplay.GetComponent<CharacterDisplay>().SetCharacter(c);
            }
        }

        public void DisplayCharacterFound(char c) {
            foreach (Transform child in transform) {
                CharacterDisplay characterDisplay = child.GetComponent<CharacterDisplay>();
                if (characterDisplay.ContainCharacter(c)) {
                    characterDisplay.DisplayCharacter();
                    break;
                }
            }
        }
    }
}