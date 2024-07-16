using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour {
    public Dictionary<string, List<GameObject>> sceneObjects = new();

    public static SceneData Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public void AddObject(string sceneName, GameObject obj) {
        if (!sceneObjects.ContainsKey(sceneName)) {
            sceneObjects.Add(sceneName, new List<GameObject>());
        }
        sceneObjects[sceneName].Add(obj);
    }

    public void AddObject(GameObject obj) {
        AddObject(SceneManager.GetActiveScene().name, obj);
    }

    public void RemoveObject(string sceneName, GameObject obj) {
        if (sceneObjects.ContainsKey(sceneName)) {
            sceneObjects[sceneName].Remove(obj);
        }
    }

    public void RemoveObject(GameObject obj) {
        RemoveObject(SceneManager.GetActiveScene().name, obj);
    }
}