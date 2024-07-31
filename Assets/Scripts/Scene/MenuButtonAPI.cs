using UnityEngine;
using UnityEngine.UI;

public class MenuButtonAPI : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
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
            AudioManager.Instance.StopBGMCrossfade();
            MissionManager.Instance.SkipCurrentMission();
        }
    }

    public void DeleteSave() {
        if (SaveManager.Instance != null) {
            SaveManager.Instance.DeleteSave();
        }
    }

    public void SetMasterVolume() {
        if (AudioManager.Instance != null) {
            float volume = volumeSlider.value;
            AudioManager.Instance.UpdateMasterVolume(volume);
        }
    }
}
