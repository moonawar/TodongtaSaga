using UnityEngine;

public class CoreSingleton : MonoBehaviour
{
    public static CoreSingleton Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
