using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace TodongtoaSaga.Minigames
{
    public class CountdownTimer : MonoBehaviour
    {
        [SerializeField] private float duration = 120f;
        [SerializeField] private TextMeshProUGUI timerDisplay;
        [SerializeField] private UnityEvent onCountdownFinished;

        private float remainingTime;
        private bool isRunning = false;

        void Awake()
        {
            ResetTimer();
        }

        void Update()
        {
            if (isRunning)
            {
                if (remainingTime > 0)
                {
                    remainingTime -= Time.deltaTime;
                    UpdateTimerDisplay();
                }
                else
                {
                    EndTimer();
                }
            }
        }

        public void StartTimer()
        {
            isRunning = true;
        }

        public void PauseTimer()
        {
            isRunning = false;
        }

        public void ResetTimer()
        {
            remainingTime = duration;
            isRunning = false;
            UpdateTimerDisplay();
        }

        private void EndTimer()
        {
            isRunning = false;
            remainingTime = 0;
            UpdateTimerDisplay();
            onCountdownFinished?.Invoke();
        }

        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            if (timerDisplay != null)
            {
                timerDisplay.text = timeString;
            }
            else
            {
                Debug.LogWarning("Timer Display (TextMeshProUGUI) is not assigned!");
            }
        }
    }
}