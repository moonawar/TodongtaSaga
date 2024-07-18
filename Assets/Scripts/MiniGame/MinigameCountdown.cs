using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private int duration = 3;
    [SerializeField] private UnityEvent callback;

    public void StartCountdown() {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown() {
        for (int i = duration; i > 0; i--) {
            textMeshProUGUI.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        callback?.Invoke();
    }
}
