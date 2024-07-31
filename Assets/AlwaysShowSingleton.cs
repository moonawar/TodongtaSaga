using UnityEngine;

public class AlwaysShowSingleton : MonoBehaviour
{
    private static AlwaysShowSingleton instance;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
