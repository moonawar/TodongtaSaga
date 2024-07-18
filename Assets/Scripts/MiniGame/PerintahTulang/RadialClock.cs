using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TodongtoaSaga.Minigames.PerintahTulang
{
    public class RadialClock : MonoBehaviour {
        [SerializeField] private float duration = 5f;
        [SerializeField] private Sprite[] clockSprites;

        [SerializeField] private UnityEvent onClockFinished;

        private void Awake() {
            GetComponent<Image>().sprite = clockSprites[0];
        }

        public void StartClock() {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown() {
            for (int i = 0; i < clockSprites.Length; i++) {
                GetComponent<Image>().sprite = clockSprites[i];
                yield return new WaitForSeconds(duration / clockSprites.Length);
            }

            onClockFinished?.Invoke();
        }
    }
}