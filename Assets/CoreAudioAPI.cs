using UnityEngine;

public class CoreAudioAPI : MonoBehaviour
{
    public void PlaySFX(string name) {
        AudioManager.Instance.PlaySFX(name);
    }
}
