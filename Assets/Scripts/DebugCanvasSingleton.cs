using UnityEngine;

public class DebugCanvasSingleton : MonoBehaviour
{
    public static DebugCanvasSingleton Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
