using UnityEngine;
using UnityEngine.UI;

public class GamePauseMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Awake() {
        if (AudioManager.Instance != null) {
            volumeSlider.value = AudioManager.Instance.MasterVolume;
        }
    
    }

    public void UpdateMasterVolume()
    {
        if (AudioManager.Instance != null)
        {
            float volume = volumeSlider.value;
            AudioManager.Instance.UpdateMasterVolume(volume);
        }
    }
}
