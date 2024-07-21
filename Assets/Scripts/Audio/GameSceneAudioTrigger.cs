using UnityEngine;

public class GameSceneAudioTrigger : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayBGMOverwrite("Game");
    }
}
