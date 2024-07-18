using UnityEngine;
using UnityEngine.Events;

namespace TodongtoaSaga.Minigames.PenyelamatanBoras
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private Tikus tikus;

        [Header("UI")]
        [SerializeField] private AmangUdaUIMeter angerMeterUI;
        [SerializeField] private TikusHealthUI tikusHealthUI;

        [SerializeField] private UnityEvent OnEnd;
        [SerializeField] private UnityEvent OnLose;
        [SerializeField] private UnityEvent OnWin;

        public int TikusHealth { get; private set; } = 3;
        public int AngerMeter { get; private set; } = 0;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }

            GameStateManager.Instance.ToOpenUI();
        }

        public void IncrementAngerMeter()
        {
            if (AngerMeter >= 3) return;

            AngerMeter++;
            angerMeterUI.UpdateMeter(AngerMeter);
            if (AngerMeter >= 3)
            {
                OnEnd?.Invoke();
                OnLose?.Invoke();
            }
        }

        public void DecrementTikusHealth()
        {
            if (TikusHealth <= 0) return;

            TikusHealth--;
            tikusHealthUI.UpdateHealth(TikusHealth);
            if (TikusHealth <= 0)
            {
                tikus.Die();

                OnEnd?.Invoke();
                OnWin?.Invoke();
            }
        }
    }
}