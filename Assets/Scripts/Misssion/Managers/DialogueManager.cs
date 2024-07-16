using System;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {
    [SerializeField] private DialogueRunner dialogueRunner;

    private void Awake() {
        if (dialogueRunner == null) {
            Debug.LogWarning("DialogueRunner not found. Searching for one in the scene.");
        }
    }

    public void StartDialogue(string dialogueName, Action onFinished = null) {

        dialogueRunner.Stop();
        dialogueRunner.onDialogueComplete.RemoveAllListeners();
        dialogueRunner.onDialogueComplete.AddListener(() => {
            onFinished?.Invoke();
            dialogueRunner.onDialogueComplete.RemoveAllListeners();
        });

        dialogueRunner.StartDialogue(dialogueName);
    }

    public void OnSkipped() {
        dialogueRunner.onDialogueComplete.RemoveAllListeners();
        dialogueRunner.Stop();
    }
}