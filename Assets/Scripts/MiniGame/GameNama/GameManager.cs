using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TodongtoaSaga.Minigames.NameGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private string characterName;
        private Dictionary<char, int> nameCharsCount;
        private int charsCount;

        [SerializeField] private UnityEvent OnEnd;
        [SerializeField] private UnityEvent OnLose;
        [SerializeField] private UnityEvent OnWin;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            GameStateManager.Instance.ToOpenUI();
            InitializeNameCharsCount();
        }

        private void InitializeNameCharsCount()
        {
            nameCharsCount = new Dictionary<char, int>();

            foreach (char c in characterName)
            {
                if (!nameCharsCount.ContainsKey(c))
                {
                    nameCharsCount[c] = 1;
                }
            }

            charsCount = nameCharsCount.Count;
        }

        public void TimesUp()
        {
            OnEnd?.Invoke();
            OnLose?.Invoke();
        }

        public void CharacterFound(char c)
        {
            if (!nameCharsCount.ContainsKey(c)) return;

            nameCharsCount[c]--;
            if (nameCharsCount[c] == 0)
            {
                charsCount--;
            }

            if (charsCount == 0)
            {
                OnEnd?.Invoke();
                OnWin?.Invoke();
            }
        }
    }

}