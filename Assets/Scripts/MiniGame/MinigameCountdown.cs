using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private UnityEvent callback;

    public void StartCountdown() {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown() {
        textMeshProUGUI.text = "3";
        yield return new WaitForSeconds(1f);
        textMeshProUGUI.text = "2";
        yield return new WaitForSeconds(1f);
        textMeshProUGUI.text = "1";
        yield return new WaitForSeconds(1f);

        callback?.Invoke();
    }
}
