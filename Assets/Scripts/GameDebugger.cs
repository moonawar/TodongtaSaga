using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugger : MonoBehaviour
{
    [Range(0f, 8f)]
    [SerializeField] private float timeScale = 1f;
    private float storedTimeScale;

    private void Start()
    {
        storedTimeScale = timeScale;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            timeScale = 9f;
        } else if (Input.GetKeyUp(KeyCode.LeftShift)) {
            timeScale = 1f;
        }


        if (timeScale != storedTimeScale)
        {
            storedTimeScale = timeScale;
            Time.timeScale = timeScale;
        }
    }

    public void LoadGame() {
        SaveManager.Instance.LoadGame();
    }

    public void SaveGame() {
        SaveManager.Instance.SaveGame();
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(GameDebugger))]
public class GameDebuggerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameDebugger debugger = (GameDebugger)target;

        if (GUILayout.Button("Save Game"))
        {
            debugger.SaveGame();
        }

        if (GUILayout.Button("Load Game"))
        {
            debugger.LoadGame();
        }
    }
}
#endif