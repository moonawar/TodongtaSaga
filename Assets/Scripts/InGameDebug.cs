using UnityEngine;
using TMPro;

public class InGameDebug : MonoBehaviour
{
    public static InGameDebug Instance;
    [SerializeField] private RectTransform scrollContent;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private RectTransform parent;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        ToggleDebugUI(false);
        Debug.unityLogger.logHandler = new InGameLogHandler();
    }

    public void Log(string message) {
        GameObject text = Instantiate(textPrefab, scrollContent);
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        text.GetComponent<TextMeshProUGUI>().text = $"[{timestamp}] {message}";
    }

    public void LogError(string message) {
        GameObject text = Instantiate(textPrefab, scrollContent);
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        text.GetComponent<TextMeshProUGUI>().text = $"[{timestamp}] <color=red>{message}</color>";
    }

    public void LogWarning(string message) {
        GameObject text = Instantiate(textPrefab, scrollContent);
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        text.GetComponent<TextMeshProUGUI>().text = $"[{timestamp}] <color=yellow>{message}</color>";
    }

    public void ClearLog() {
        foreach (Transform child in scrollContent) {
            Destroy(child.gameObject);
        }
    }

    public void ToggleDebugUI(bool open) {
        parent.gameObject.SetActive(open);
    }

    public void SkipTask() {
        MissionManager.Instance.SkipCurrentTask();
    }

    public void SkipMission() {
        MissionManager.Instance.SkipCurrentMission();
    }
}
