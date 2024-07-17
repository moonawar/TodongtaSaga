using UnityEngine;

public class MenuButtonAPI : MonoBehaviour
{
    public void ToMainMenu() {
        if (SceneLoader.Instance != null) {
            SceneLoader.Instance.ToMainMenu();
        }
    }

    public void ToGameplayFromMainMenu() {
        if (SceneLoader.Instance != null) {
            SceneLoader.Instance.ToGameplayFromMainMenu();
        }
    }

    public void QuitGame() {
        if (SceneLoader.Instance != null) {
            SceneLoader.Instance.QuitGame();
        }
    }

    public void SkipTask() {
        if (MissionManager.Instance != null) {
            MissionManager.Instance.SkipCurrentTask();
        }
    }

    public void SkipMission() {
        if (MissionManager.Instance != null) {
            MissionManager.Instance.SkipCurrentMission();
        }
    }
}
