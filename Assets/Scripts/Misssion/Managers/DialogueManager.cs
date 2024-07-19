using System;
using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour {
    [SerializeField] private DialogueRunner dialogueRunner;

    private void Awake() {
        if (dialogueRunner == null) {
            Debug.LogError("DialogueRunner not found. Searching for one in the scene.");
        }
    }

    public void StartDialogue(string dialogueName, Action onFinished = null) {
        dialogueRunner.Stop();

        dialogueRunner.onDialogueComplete.RemoveAllListeners();
        dialogueRunner.onDialogueComplete.AddListener(() => {
            onFinished?.Invoke();
            dialogueRunner.onDialogueComplete.RemoveAllListeners();
        });

        Debug.Log("Starting dialogue: " + dialogueName);

        dialogueRunner.StartDialogue(dialogueName);
    }

    public void CleanDialogue() {
        dialogueRunner.onDialogueComplete.RemoveAllListeners();
    }
}